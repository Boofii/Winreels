namespace Winreels.Feature;

using System.Windows.Forms.VisualStyles;
using Winreels;
using Winreels.Core;

public class VideoFeature : DataFeature
{
    public const uint FILE_LENGTH = 64;
    public const uint DESCRIPTION_LENGTH = 256;
    public const uint TAGS_LENGTH = 256;
    public const uint LIKES_LENGTH = 256;

    public readonly string NAME = "videos";
    public readonly DatabaseFormat FORMAT = new DatabaseFormat("videos", "id", "int")
        .AddArgument("file", $"char({FILE_LENGTH})")
        .AddArgument("description", $"char({DESCRIPTION_LENGTH})")
        .AddArgument("tags", $"char({TAGS_LENGTH})")
        .AddArgument("likes", $"char({LIKES_LENGTH})");

    private FileFeature fileFeature;
    private LoggerFragment? logger;

    public Action<int>? OnUpload;
    public Action<int, string, string, string[], string[]>? OnDownload;
    public Action<int, string[]>? OnGetLikes;
    public Action<int, string[]>? OnSetLikes;

    // Creates a new VideoFeature on the server side.
    public VideoFeature(ServerFragment server)
    {
        this.fileFeature = new FileFeature().WithServer("videos", server);
        this.WithServer(server, NAME, FORMAT);
        this.AddCommandServer("upload_video", Upload);
        this.AddCommandServer("download_video", Download);
        this.AddCommandServer("get_likes", GetLikes);
        this.AddCommandServer("set_likes", SetLikes);
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
            if (status != 0)
            {
                OnDownload?.Invoke(status, string.Empty, string.Empty, [], []);
                return;
            }

            string file = args[1];
            string description = args[2];
            string[] tags = args[3].Split(' ');
            string[] likes = args[4].Split(',');

            fileFeature.Download(file, (status, chunkIndex, chunkCount, path) =>
            {
                if (status != 0)
                {
                    OnDownload?.Invoke(status, string.Empty, description, tags, likes);
                    return;
                }
                OnDownload?.Invoke(status, path, description, tags, likes);
            });
        });
        this.AddCommandClient("get_likes_response", args =>
        {
            int status = int.Parse(args[0]);
            if (status != 0)
            {
                OnGetLikes?.Invoke(status, []);
                return;
            }

            string raw = args[1];
            string[] likes = raw.Split(',');
            OnGetLikes?.Invoke(status, likes);
        });
        this.AddCommandClient("set_likes_response", args =>
        {
            int status = int.Parse(args[0]);
            string[] likes = args[1].Split(',');
            OnSetLikes?.Invoke(status, likes);
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
    public void Download(Action<int, string, string, string[], string[]> OnDownload)
    {
        if (client == null)
            return;
        
        this.OnDownload = OnDownload;

        client.Execute("download_video", []);
    }

    /// <summary>
    /// Gets all the users who liked a certain video on the client side.
    /// The OnGetLikes handler int parameter is the response code:
    ///     1 - Server error
    ///     0 - Success
    /// </summary>
    public void GetLikes(string path, Action<int, string[]> OnGetLikes)
    {
        if (client == null)
            return;

        this.OnGetLikes = OnGetLikes;

        client.Execute("get_likes", [path]);
    }

    /// <summary>
    /// Sets a like for a certain video on the client side.
    /// If the video is already likes, the video is unliked.
    /// The OnSetLikes handler int parameter is the response code:
    ///     1 - Server error
    ///     0 - Success
    /// </summary>
    public void SetLikes(string path, Action<int, string[]> OnSetLikes)
    {
        if (client == null)
            return;

        this.OnSetLikes = OnSetLikes;

        client.Execute("set_likes", [path]);
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
        int result = this.Put(["file", "description", "tags", "likes"], [name, description, tags, ""]);
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
        string likes = obj["likes"].ToString() ?? "";
        server.Execute("download_video_response", ["0", file, description, tags, likes], id);
    }

    // Attempts to get all the users who liked a video on the server side.
    private void GetLikes(int id, string[] args)
    {
        if (server == null || database == null)
            return;

        string path = Path.GetFileName(args[0]) ?? "";
        if (!this.Exists(["file"], [path]))
        {
            server.Execute("get_likes_response", ["1", string.Empty], id);
            return;
        }

        string sql = $"SELECT * FROM {NAME} WHERE file = @path";
        object raw = database.Query(sql, ["path"], [path])[0]["likes"];
        string likes = raw as string ?? "";
        server.Execute("get_likes_response", ["0", likes], id);
    }

    // Attempts to set a like or unlike a certain video on the server side.
    private void SetLikes(int id, string[] args)
    {
        if (server == null || database == null)
            return;

        string path = Path.GetFileName(args[0]) ?? "";
        if (!this.Exists(["file"], [path]))
        {
            server.Execute("set_likes_response", ["1"], id);
            return;
        }

        string sql1 = $"SELECT * FROM {NAME} WHERE file = @path";
        object raw = database.Query(sql1, ["path"], [path])[0]["likes"];
        string likes = raw as string ?? "";

        if (!server.actConnections.TryGetValue(id, out string? username) || username == null)
        {
            server.Execute("set_likes_response", ["1"], id);
            return;
        }

        if (string.IsNullOrEmpty(likes))
        {
            likes = $",{username}";
        }
        else
        {
            if (!likes.Contains(username))
                likes += $",{username}";
            else
                likes = likes.Replace($",{username}", "");
        }

        string sql2 = $"UPDATE {NAME} SET likes = @newLikes WHERE file = @path";
        database.NonQuery(sql2, ["newLikes", "path"], [likes, path]);
        server.Execute("set_likes_response", ["0", likes], id);
    }
}