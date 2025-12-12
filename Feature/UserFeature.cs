namespace WinReels.Feature;

using Winreels;
using Winreels.Core;

/// <summary>
/// This class is used to automatically create a hashed user database.
/// To interact with it, a client should execute register or login commands.
/// A response will be provided by the register_response or login_response commands.
/// </summary>
public class UserFeature : DataFeature
{
    public const uint EMAIL_LENGTH = 254;
    public const uint UNAME_LENGTH = 32;
    public const uint PWD_LENGTH = 64;

    public readonly string NAME = "users";
    public readonly DatabaseFormat FORMAT = new DatabaseFormat("users", "id", "int")
        .AddArgument("email", $"char({EMAIL_LENGTH})")
        .AddArgument("username", $"char({UNAME_LENGTH})")
        .AddArgument("password", $"char({PWD_LENGTH})");

    private LoggerFragment? logger;

    public Action<string[]>? OnRegister;
    public Action<string[]>? OnLogin;

    // Creates a new UserFeature on the server side.
    public UserFeature(ServerFragment server)
    {
        this.WithServer(server, NAME, FORMAT);
        this.AddCommandServer("register", Register);
        this.AddCommandServer("login", Login);
    }

    // Creates a new UserFeature on the client side.
    public UserFeature(ClientFragment client)
    {
        this.WithClient(client);
        this.AddCommandClient("register_response", OnRegister);
        this.AddCommandClient("login_response", OnLogin);
    }

    // Links a LoggerFragment with this UserFeature.
    public UserFeature WithLogger(LoggerFragment logger)
    {
        this.logger = logger;
        return this;
    }

    /// <summary>
    /// Attempts to register a user on the client side.
    /// The OnRegister handler int parameter is the response code:
    ///     1 - Email exists
    ///     2 - Username exists
    ///     3 - Email invalid
    ///     4 - Username invalid
    ///     5 - Password invalid
    ///     0 - Success
    /// </summary>
    public void Register(string email, string username, string password, Action<string[]> OnRegister)
    {
        if (client == null)
            return;
        
        if (this.OnRegister == null)
            this.OnRegister = OnRegister;

        client.Execute("register", [email, username, password]);
    }

    /// <summary>
    /// Attempts to login to a user on the client side.
    /// The OnLogin handler int parameter is the response code:
    ///     1 - Username doesn't exist
    ///     2 - Invalid password
    ///     0 - Success
    /// </summary>
    public void Login(string username, string password, Action<string[]> OnLogin)
    {
        if (client == null)
            return;
        
        if (this.OnLogin == null)
            this.OnLogin = OnLogin;

        client.Execute("login", [username, password]);
    }

    // Attempts to register a user on the server side.
    private void Register(int id, string[] args)
    {
        if (server == null || database == null)
            return;

        string email = args[0];
        string username = args[1];
        string password = args[2];

        if (this.Exists(["email"], [email]))
        {
            server.Execute("register_response", ["1"], id);
            return;
        }
        if (this.Exists(["username"], [username]))
        {
            server.Execute("register_response", ["2"], id);
            return;
        }

        string hash = CryptoFragment.Hash(password);
        this.Put(["email", "username", "password"], [email, username, hash]);
        server.Execute("register_response", ["0"], id);
        logger?.Log(LogLevel.INFO, $"Added a user named: {username}.");
    }

    // Attempts to login to a user on the server side.
    private void Login(int id, string[] args)
    {
        if (server == null || database == null)
            return;
        
        string username = args[0];
        string password = args[1];

        if (!this.Exists(["username"], [username]))
        {
            server.Execute("login_response", ["1"], id);
            return;
        }
        string hash = CryptoFragment.Hash(password);
        if (!this.Exists(["password"], [hash]))
        {
            server.Execute("login_response", ["2"], id);
            return;
        }

        server.Execute("login_response", ["0"], id);
        logger?.Log(LogLevel.INFO, $"Signed in to a user named: {username}.");
    }
}