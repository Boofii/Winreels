namespace Winreels;

/// <summary>
/// Responsible for the login user interface.
/// </summary>
public partial class UploadInterface : Form
{
    private string? path = null;

    public UploadInterface()
    {
        InitializeComponent();
    }

    private void SelectFile(object sender, EventArgs args)
    {
        if (file.ShowDialog() == DialogResult.OK)
        {
            this.path = file.FileName;
        }
    }

    private void Publish(object sender, EventArgs args)
    {
        if (path != null)
        {
            Thread uploadThread = new Thread(() =>
            {
                string description = this.description.Text;
                string tags = this.tags.Text;
                Program.ClientVmanager?.Upload(path, description, tags, (status) =>
                {
                    MessageBox.Show(status.ToString());
                });
            });
            uploadThread.Start();
        }
    }

    private void Watch(object sender, EventArgs args)
    {
        WatchInterface wi = new WatchInterface();
        this.Close();
        wi.Show();
    }
}