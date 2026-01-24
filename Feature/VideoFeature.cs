namespace WinReels.Feature;

using Winreels;
using Winreels.Core;
using Winreels.Feature;

public class VideoFeature : DataFeature
{
    public const uint FILE_LENGTH = 64;
    public const uint DESCRIPTION_LENGTH = 256;
    public const uint TAGS_LENGTH = 256;

    public readonly string NAME = "videos";
    public readonly DatabaseFormat FORMAT = new DatabaseFormat("videos", "id", "int")
        .AddArgument("file", $"char({FILE_LENGTH})")
        .AddArgument("description", $"char({DESCRIPTION_LENGTH})")
        .AddArgument("tags", $"char({TAGS_LENGTH})");

    private FileFeature fileFeature;
    private LoggerFragment? logger;

    public Action<int>? OnUpload;
    public Action<int, string>? OnDownload;

    // Creates a new VideoFeature on the server side.
    public VideoFeature(ServerFragment server)
    {
        this.fileFeature = new FileFeature().WithServer("videos", server);
        this.WithServer(server, NAME, FORMAT);
        this.AddCommandServer("upload_video", Upload);
        this.AddCommandServer("download_video", Download);
        this.AddRule("file", input =>
        {
            string? file = input.ToString();
            if (file != null && file.Length > 0 && file.Length < FILE_LENGTH)
                return 0;
            return 1;
        });
        this.AddRule("description", input =>
        {
            string? desc = input.ToString();
            if (desc != null && desc.Length > 0 && desc.Length < DESCRIPTION_LENGTH)
                return 0;
            return 2;
        });
        this.AddRule("tags", input =>
        {
            string? tags = input.ToString();
            if (tags != null && tags.Length > 0 && tags.Length < TAGS_LENGTH)
                return 0;
            return 3;
        });
    }

    // Creates a new VideoFeature on the client side.
    public VideoFeature(ClientFragment client)
    {
        this.fileFeature = new FileFeature().WithClient(client);
        this.WithClient(client);
        this.AddCommandClient("upload_video_response", args =>
        {
            int status = int.Parse(args[0]);
            OnUpload?.Invoke(status);
        });
        this.AddCommandClient("download_video_response", args =>
        {
            int status = int.Parse(args[0]);
            string file = args[1];
            string description = args[2];
            string tags = args[3];

            if (status != 0)
            {
                OnDownload?.Invoke(status, string.Empty);
                return;
            }
            fileFeature.Download(file, (status, chunkIndex, chunkCount, path) =>
            {
                if (status != 0)
                {
                    OnDownload?.Invoke(1, string.Empty);
                    return;
                }
                OnDownload?.Invoke(0, path);
            });
        });
    }

    // Links a LoggerFragment with this VideoFeature.
    public VideoFeature WithLogger(LoggerFragment logger)
    {
        this.fileFeature.WithLogger(logger);
        this.logger = logger;
        return this;
    }

    /// <summary>
    /// Attempts to upload a video on the client side.
    /// The OnUpload handler int parameter is the response code:
    ///     1 - File invalid
    ///     2 - Description invalid
    ///     3 - Tags invalid
    ///     4 - Server error
    ///     0 - Success
    /// </summary>
    public void Upload(string file, string description, string tags, Action<int> OnUpload)
    {
        if (client == null)
            return;
        
        string name = Path.GetFileName(file);
        byte[] data = File.ReadAllBytes(file);
        fileFeature.Upload(name, data, (status) =>
        {
            if (status == 0)
            {
                this.OnUpload = OnUpload;

                client.Execute("upload_video", [file, description, tags]);
                return;
            }
            OnUpload?.Invoke(4);
        });
    }

    /// <summary>
    /// Attempts to download a random video on the client side.
    /// The OnDownload handler int parameter is the response code:
    ///     1 - Server error
    ///     0 - Success
    /// </summary>
    public void Download(Action<int, string> OnDownload)
    {
        if (client == null)
            return;
        
        this.OnDownload = OnDownload;

        client.Execute("download_video", []);
    }

    // Attempts to upload a video on the server side.
    private void Upload(int id, string[] args)
    {
        if (server == null || database == null)
            return;

        string file = args[0];
        string description = args[1];
        string tags = args[2];
        string name = Path.GetFileName(file);
        int result = this.Put(["file", "description", "tags"], [name, description, tags]);
        server.Execute("upload_video_response", [result.ToString()], id);
        logger?.Log(LogLevel.INFO, $"Status code for video upload for id: {id} is {result}.");
    }

    // Attempts to download a video on the server side.
    private void Download(int id, string[] args)
    {
        if (server == null || database == null)
            return;

        string sql = $"SELECT * FROM {NAME} ORDER BY RANDOM() LIMIT 1";
        List<Dictionary<string, object>> result = database.Query(sql);
        if (result.Count <= 0)
        {
            logger?.Log(LogLevel.ERROR, $"The video database was empty for id: {id}.");
            server.Execute("download_video_response", ["1"], id);
            return;
        }

        Dictionary<string, object> obj = result[0];
        string file = obj["file"].ToString() ?? "";
        string description = obj["description"].ToString() ?? "";
        string tags = obj["tags"].ToString() ?? "";
        server.Execute("download_video_response", ["0", file, description, tags], id);
    }
}