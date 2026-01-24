namespace Winreels;

/// <summary>
/// Responsible for the login user interface.
/// </summary>
public partial class VideoInterface : Form
{
    private string? path = null;

    public VideoInterface()
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
            string description = this.description.Text;
            string tags = this.tags.Text;
            Program.ClientVmanager?.Upload(path, description, tags, (status) =>
            {
                MessageBox.Show(status.ToString());
            });
        }
    }

    private void Watch(object sender, EventArgs args)
    {
        WatchInterface wi = new WatchInterface();
        this.Hide();
        wi.Show();
    }
}