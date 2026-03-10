using LibVLCSharp.Shared;
using System.IO;
using Winreels.Data;

namespace Winreels;

/// <summary>
/// Responsible for the video playback user interface.
/// </summary>
public partial class WatchInterface : Form
{
    private readonly List<VideoData> videos = [];
    private int index = 0;
    private bool first = false;

    public LibVLC lib;
    public MediaPlayer player;
    private Media? media;

    public WatchInterface()
    {
        InitializeComponent();

        LibVLCSharp.Shared.Core.Initialize();
        lib = new LibVLC();
        player = new MediaPlayer(lib);
        view.MediaPlayer = player;
        Controls.Add(view);
    }

    private void Upload(object sender, EventArgs args)
    {
        UploadInterface vi = new UploadInterface();
        this.Close();
        vi.Show();
    }

    private void FormLoad(object sender, EventArgs args)
    {
        Thread downloadThread = new Thread(() =>
        {
            bool ready = true;
            while (true)
            {
                if (ready)
                {
                    ready = false;

                    Program.ClientVmanager?.Download((status, path, description, tags, likes) =>
                    {
                        if (status == 0)
                        {
                            videos.Add(new VideoData(path, description, tags, likes));
                            ready = true;

                            if (!first)
                            {
                                first = true;
                                SetVideo();
                            }
                        }
                    });
                }
            }
        });
        downloadThread.Start();
    }

    private void SetVideo()
    {
        Thread renderThread = new Thread(() =>
        {
            if (index < videos.Count)
            {
                VideoData data = videos[index];
                likes.Text = $"({data.likes.Length - 1})";
                description.Text = data.description;
                tags.Text = "#" + string.Join(" #", data.tags);

                media?.Dispose();
                media = new Media(lib, data.path, FromType.FromPath);
                player.Play(media);
            }
        });
        renderThread.Start();
    }

    protected override void OnFormClosing(FormClosingEventArgs args)
    {
        player?.Dispose();
        lib?.Dispose();
        media?.Dispose();
        view?.Dispose();
        base.OnFormClosing(args);
    }

    private void next_Click(object sender, EventArgs args)
    {
        if (videos.Count <= 0)
            return;

        index = Math.Clamp(index + 1, 0, videos.Count - 1);
        SetVideo();
    }

    private void previous_Click(object sender, EventArgs args)
    {
        if (videos.Count <= 0)
            return;

        index = Math.Clamp(index - 1, 0, videos.Count - 1);
        SetVideo();
    }

    private void likes_Click(object sender, EventArgs args)
    {
        VideoData data = videos[index];
        Program.ClientVmanager?.SetLikes(data.path, (res, array) =>
        {
            data.likes = array;
            likes.Text = $"({array.Length - 1})";
        });
    }
}