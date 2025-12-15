using Spectre.Console;
using Winreels.Core;
using Winreels.Interface;
using WinReels.Feature;

namespace Winreels;

/// <summary>
/// This class is used to create a TUI for registering a user and logging in to a user.
/// </summary>
public class UserInterface : MainInterface
{
    private readonly ClientFragment client;
    private readonly CryptoFragment crypto;
    private readonly UserFeature uf;

    // Creates a new UserInterface.
    public UserInterface()
    {
        this.client = new ClientFragment("127.0.0.1", 4098);
        this.crypto = new CryptoFragment().WithClient(client);
        this.uf = new UserFeature(client);
        this.AddScene("Sign In");
        this.AddScene("Sign Up");
        this.AddFields(0, ["Username", "Password"]);
        this.AddFields(1, ["Email", "Username", "Password"]);
        this.OnFieldEnter += FieldEnter;
    }

    // Starts a client and renders the first scene.
    public override void Initialize()
    {
        base.Initialize();
        client.Connect();
    }

    // Handles the enter key on a certain field.
    private void FieldEnter(int scene, int id, string input)
    {
        switch (scene)
        {
            case 0:
                    string username1 = texts[scene][0];
                    string password1 = texts[scene][1];
                    uf.Login(username1, password1, (status) =>
                    {
                        if (status == 0)
                        {
                            AnsiConsole.Write(new Markup($"[green][bold]Logged in successfully[/][/]").Centered());
                            this.Close();
                            return;
                        }
                        AnsiConsole.Write(new Markup($"[red][bold]Failed to login with error code: {status}[/][/]").Centered());
                    });
                break;
            case 1:
                string email = texts[scene][0];
                string username2 = texts[scene][1];
                string password2 = texts[scene][2];
                uf.Register(email, username2, password2, (status) =>
                {
                    if (status == 0)
                    {
                        AnsiConsole.Write(new Markup($"[green][bold]Registered successfully[/][/]").Centered());
                        return;
                    }
                    AnsiConsole.Write(new Markup($"[red][bold]Failed to register with error code: {status}[/][/]").Centered());
                });
                break;
        }
    }
}