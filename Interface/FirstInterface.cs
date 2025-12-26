using Spectre.Console;
using Winreels.Core;
using WinReels.Feature;

namespace Winreels.Interface;

/// <summary>
/// This class is used to initialize a server.
/// </summary>
public class ServerInterface
{
    private readonly LoggerFragment logger;
    private readonly ServerFragment server;
    private readonly CryptoFragment crypto;
    private readonly UserFeature uf;

    public ServerInterface()
    {
        this.logger = new LoggerFragment("server");
        this.server = new ServerFragment("127.0.0.1", 4098).WithLogger(logger);
        this.crypto = new CryptoFragment().WithServer(server, "public.pem", "private.pem").WithLogger(logger);
        this.uf = new UserFeature(server).WithLogger(logger);
    }

    public void Initialize()
    {
        server.Establish();
    }
}

/// <summary>
/// This class is used to initialize a client.
/// It also initializes a login and register scene.
/// </summary>
public class ClientInterface
{
    public static ClientInterface? Instance;

    public readonly ClientFragment client;
    public readonly CryptoFragment crypto;
    public readonly UserFeature uf;

    public string username = "";

    public ClientInterface()
    {
        this.client = new ClientFragment("127.0.0.1", 4098);
        this.crypto = new CryptoFragment().WithClient(client);
        this.uf = new UserFeature(client);
        if (Instance == null)
            Instance = this;
    }

    public void Initialize()
    {
        client.Connect();
        SceneManager.AddScene(new LoginScene());
        SceneManager.AddScene(new RegisterScene());
        SceneManager.Initialize();
    }
}

/// <summary>
/// A login scene.
/// </summary>
public class LoginScene : Scene
{
    public override string Name => "Sign In";

    public override void Initialize()
    {
        base.Initialize();
        fields.Add(new Field("Username", true));
        fields.Add(new Field("Password", true));
        fields[1].OnEnter += Submit;
    }

    public override void Render()
    {
        base.Render();
        AnsiConsole.Write("\n");

        for (int i = 0; i < fields.Count; i++)
        {
            fields[i].Render(i == currentField);
        }
    }

    public void Submit()
    {
        string username = fields[0].text;
        string password = fields[1].text;
        ClientInterface.Instance?.uf.Login(username, password, (response) =>
        {
            string text = (response == 0) ? "[green]Success[/]" : "[red]Fail[/]";
            AnsiConsole.Write(new Markup(text).Centered());

            if (response != 0)
                return;
            
            AnsiConsole.Clear();
            ClientInterface.Instance.username = username;
            SceneManager.RemoveScene(SceneManager.Scenes[1]);
            SceneManager.RemoveScene(SceneManager.Scenes[0]);
            SceneManager.AddScene(new ChatInterface());
            SceneManager.Scenes[0].Initialize();
            SceneManager.Scenes[0].Render();
        });
    }
}

/// <summary>
/// A register scene.
/// </summary>
public class RegisterScene : Scene
{
    public override string Name => "Sign Up";

    public override void Initialize()
    {
        base.Initialize();
        fields.Add(new Field("Email", true));
        fields.Add(new Field("Username", true));
        fields.Add(new Field("Password", true));
        fields[2].OnEnter += Submit;
    }

    public override void Render()
    {
        base.Render();
        AnsiConsole.Write("\n");

        for (int i = 0; i < fields.Count; i++)
        {
            fields[i].Render(i == currentField);
        }
    }

    public void Submit()
    {
        string email = fields[0].text;
        string username = fields[1].text;
        string password = fields[2].text;
        ClientInterface.Instance?.uf.Register(email, username, password, (response) =>
        {
            string text = (response == 0) ? "[green]Success[/]" : "[red]Fail[/]";
            AnsiConsole.Write(new Markup(text).Centered());
        });
    }
}