using Winreels.Core;

namespace Winreels.Feature;

/// <summary>
/// This class is used to send and receive files to/from a server.
/// Upload files using the upload command and download them using the download command.
/// </summary>
public class FileFeature
{
    public const uint MAX_SIZE = 25000000;
    public const uint IO_RATE = 1000;

    private readonly Dictionary<string, Action<int, int, int, string>> OnDownload = [];
    private readonly Dictionary<string, Action<int>> OnUpload = [];
    
    private LoggerFragment? logger;
    private ServerFragment? server;
    private ClientFragment? client;
    private string? path;
    
    // Links a LoggerFragment with this FileFeature
    public FileFeature WithLogger(LoggerFragment logger)
    {
        this.logger = logger;
        return this;
    }

    // Links a ServerFragment with this FileFeature
    public FileFeature WithServer(string name, ServerFragment server)
    {
        this.server = server;
        server.OnReceived += ParseCommand;
        string path = $"{name}";
        this.path = path;
        Directory.CreateDirectory(path);
        return this;
    }

    // Links a ClientFragment with this FIleFeature
    public FileFeature WithClient(ClientFragment client)
    {
        this.client = client;
        client.OnReceived += ParseCommand;
        return this;
    }

    /// <summary>
    /// Attempts to upload a file to the server on the client side.
    /// The OnUpload handler int parameter is the response code:
    ///     1 - Server exception
    ///     0 - Success
    /// </summary>
    public void Upload(string name, byte[] data, Action<int> OnUpload)
    {
        if (client == null || this.OnUpload == null || this.OnUpload.ContainsKey(name))
            return;

        this.OnUpload[name] = OnUpload;

        uint chunkCount = (uint)Math.Ceiling((double)data.Length / IO_RATE);
        int index = 0;

        for (uint i = 0; i < chunkCount; i++)
        {
            int remaining = data.Length - index;
            int size = Math.Min((int)IO_RATE, remaining);
            byte[] current = new byte[size];

            Array.Copy(data, index, current, 0, size);
            string encoded = Convert.ToBase64String(current);

            client.Execute("upload", [name, i.ToString(), chunkCount.ToString(), encoded]);
            index += size;
            Thread.Sleep(2);
        }
    }

    /// <summary>
    /// Attempts to download a file from the server on the client side.
    /// The OnDownload handler has the following arguments:
    ///     result:
    ///         1 - Server exception
    ///         2 - File doesn't exist
    ///         0 - Success
    ///     chunkIndex
    ///     chunkCount
    ///     data
    /// </summary>
    public void Download(string name, Action<int, int, int, string> OnDownload)
    {
        if (client == null || this.OnDownload == null || this.OnDownload.ContainsKey(name))
            return;

        this.OnDownload[name] = OnDownload;
        client.Execute("download", [name]);
    }

    // Uploads a file on the server side.
    private void Upload(int id, string[] args)
    {
        if (server == null)
            return;
        
        try {
            string fileName = args[0];
            uint chunkId = uint.Parse(args[1]);
            uint chunkAmount = uint.Parse(args[2]);
            byte[] data = Convert.FromBase64String(args[3]);

            string newPath = Path.Combine(path, fileName);

            if (chunkId <= 0)
            {
                logger?.Log(LogLevel.INFO, $"Started processing a file named: {fileName}.");
                File.WriteAllBytes(newPath, data);
            }
            else if (chunkId < chunkAmount - 1)
            {
                logger?.Log(LogLevel.INFO, $"Received a new chunk for file, chunkId: {fileName}, {chunkId}.");
                File.AppendAllBytes(newPath, data);
            }
            else
            {
                logger?.Log(LogLevel.INFO, $"Finished receiving a file: {fileName}.");
                File.AppendAllBytes(newPath, data);
                server.Execute("upload_response", [fileName, "0"], id);
            }
        }
        catch (Exception ex)
        {
            logger?.Log(LogLevel.ERROR, $"Data for file upload was incorrect for user, exception: {id}, {ex}.");
            server.Execute("upload_response", [args[0], "1"], id);
        }
    }

    // Downloads a file on the server side.
    private void Download(int id, string[] args)
    {
        if (server == null)
            return;
        
        try
        {
            string fileName = args[0];
            string newPath = Path.Combine(path, fileName);

            if (!File.Exists(newPath))
            {
                logger?.Log(LogLevel.ERROR, $"Tried to download a file that didn't exist for user, file: {id}, {fileName}.");
                server.Execute("download_response", [fileName, "2"], id);
                return;
            }

            byte[] data = File.ReadAllBytes(newPath);
            uint chunkCount = (uint)Math.Ceiling((double)data.Length / IO_RATE);
            int index = 0;

            for (uint i = 0; i < chunkCount; i++)
            {
                int remaining = data.Length - index;
                int size = Math.Min((int)IO_RATE, remaining);
                byte[] current = new byte[size];

                Array.Copy(data, index, current, 0, size);
                string encoded = Convert.ToBase64String(current);

                server.Execute("download_response", [fileName, "0", i.ToString(), chunkCount.ToString(), encoded], id);
                index += size;
                Thread.Sleep(2);
            }
        }
        catch (Exception ex)
        {
            logger?.Log(LogLevel.ERROR, $"Data for file download was incorrect for user, exception: {id}, {ex}.");
            server.Execute("download_response", [args[0], "1"], id);
        }
    }

    // Handles a download response on the client side.
    private void DownloadHandler(string[] args)
    {
        string fileName = args[0];
        int code = int.Parse(args[1]);
        int chunkId = int.Parse(args[2]);
        int chunkAmount = int.Parse(args[3]);
        byte[] data = Convert.FromBase64String(args[4]);

        string newPath = Path.Combine(Path.GetTempPath(), fileName);
        
        if (chunkId <= 0)
        {
            logger?.Log(LogLevel.INFO, $"Started processing a file named: {fileName}.");
            File.WriteAllBytes(newPath, data);
        }
        else if (chunkId < chunkAmount - 1)
        {
            logger?.Log(LogLevel.INFO, $"Received a new chunk for file, chunkId: {fileName}, {chunkId}.");
            File.AppendAllBytes(newPath, data);
        }
        else
        {
            logger?.Log(LogLevel.INFO, $"Finished receiving a file: {fileName}.");
            File.AppendAllBytes(newPath, data);
            OnDownload[fileName].Invoke(code, chunkId, chunkAmount, newPath);
            // File.Delete(newPath);
            OnDownload.Remove(fileName);
        }
    }

    // Parses commands on the server side.
    private void ParseCommand(int id, string cmd, string[] args)
    {
        switch (cmd)
        {
            case "upload":
                if (args.Length != 4)
                    return;

                Upload(id, args);
                break;
            case "download":
                if (args.Length != 1)
                    return;

                Download(id, args);
                break;
        }
    }

    // Parses commands on the client side.
    private void ParseCommand(string cmd, string[] args)
    {
        switch (cmd)
        {
            case "upload_response":
                OnUpload[args[0]].Invoke(int.Parse(args[1]));
                OnUpload.Remove(args[0]);
                break;
            case "download_response":
                DownloadHandler(args);
                break;
        }
    }
}