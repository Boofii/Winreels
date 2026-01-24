namespace Winreels;

/// <summary>
/// Responsible for the video playback user interface.
/// </summary>
public partial class WatchInterface : Form
{
    public WatchInterface()
    {
        InitializeComponent();
    }

    private void Upload(object sender, EventArgs args)
    {
        VideoInterface vi = new VideoInterface();
        this.Hide();
        vi.Show();
    }

    private void FormLoad(object sender, EventArgs args)
    {
        Program.ClientVmanager?.Download((status, path) =>
        {
            this.Invoke(() =>
            {
                axWindowsMediaPlayer1.URL = new Uri(path).AbsolutePath;
            });
        });
    }
}