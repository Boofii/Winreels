namespace Winreels;

/// <summary>
/// Responsible for the login user interface.
/// </summary>
public partial class LoginInterface : Form
{
    public LoginInterface()
    {
        InitializeComponent();
    }

    private void SignUp(object sender, EventArgs args)
    {
        RegisterInterface ri = new RegisterInterface();
        this.Hide();
        ri.Show();
    }

    private void SignIn(object sender, EventArgs args)
    {
        string uname = username.Text;
        string pwd = password.Text;
        Program.ClientUmanager?.Login(uname, pwd, (res) =>
        {
            this.Invoke(() =>
            {
                switch (res)
                {
                    case 0:
                        MessageBox.Show("Logged in successfully");
                        VideoInterface vi = new VideoInterface();
                        this.Hide();
                        vi.Show();
                        break;
                    case 1:
                        MessageBox.Show("Invalid username");
                        break;
                    case 2:
                        MessageBox.Show("Invalid password");
                        break;
                }
            });
        });
    }
}