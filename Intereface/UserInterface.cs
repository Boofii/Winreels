namespace Winreels;

/// <summary>
/// Responsible for the login user interface.
/// </summary>
public partial class UserInterface : Form
{
    public UserInterface()
    {
        InitializeComponent();
    }

    private void SignUp(object sender, EventArgs args)
    {
        string eml = r_email.Text;
        string uname = r_username.Text;
        string pwd = r_password.Text;
        Program.ClientUmanager?.Register(eml, uname, pwd, (res) =>
        {
            switch (res)
            {
                case 0:
                    MessageBox.Show("Signed up successfully");
                    break;
                case 1:
                    MessageBox.Show("Email exists");
                    break;
                case 2:
                    MessageBox.Show("Username exists");
                    break;
                case 3:
                    MessageBox.Show("Email invalid");
                    break;
                case 4:
                    MessageBox.Show("Username invalid");
                    break;
                case 5:
                    MessageBox.Show("Password invalid");
                    break;
            }
        });
    }

    private void SignIn(object sender, EventArgs args)
    {
        string uname = l_username.Text;
        string pwd = l_password.Text;
        Program.ClientUmanager?.Login(uname, pwd, (res) =>
        {
            this.Invoke(() =>
            {
                switch (res)
                {
                    case 0:
                        MessageBox.Show("Logged in successfully");
                        WatchInterface wi = new WatchInterface();
                        this.Hide();
                        wi.Show();
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