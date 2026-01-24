namespace Winreels;

/// <summary>
/// Responsible for the registration user interface.
/// </summary>
public partial class RegisterInterface : Form
{
    public RegisterInterface()
    {
        InitializeComponent();
    }

    private void SignIn(object sender, EventArgs args)
    {
        LoginInterface li = new LoginInterface();
        this.Hide();
        li.Show();
    }

    private void SignUp(object sender, EventArgs args)
    {
        string eml = email.Text;
        string uname = username.Text;
        string pwd = password.Text;
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
}