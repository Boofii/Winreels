namespace WinReels.Feature;

using Winreels.Core;

/// <summary>
/// This class is used to automatically create a hashed user database.
/// To interact with it, a client should execute register or login commands.
/// A response will be provided by the register_response or login_response commands.
/// </summary>
public class UserFeature
{
    public const uint EMAIL_LENGTH = 254;
    public const uint UNAME_LENGTH = 32;
    public const uint PWD_LENGTH = 64;

    private LoggerFragment? logger;
    private ServerFragment? server;
    private DatabaseFragment? database;
    private ClientFragment? client;
    private Action<int>? OnRegister;
    private Action<int>? OnLogin;

    // Links a LoggerFragment with this UserFeature.
    public UserFeature WithLogger(LoggerFragment logger)
    {
        this.logger = logger;
        return this;
    }

    // Links a ServerFragment with this UserFeature.
    public UserFeature WithServer(ServerFragment server)
    {
        this.server = server;
        server.OnReceived += ParseCommand;
        this.database = new DatabaseFragment("users")
            .WithFormat(new DatabaseFormat("users", "id", "int")
                .AddArgument("email", $"char({EMAIL_LENGTH})")
                .AddArgument("username", $"char({UNAME_LENGTH})")
                .AddArgument("password", $"char({PWD_LENGTH})"))
            .WithLogger(logger);
        database.Establish();
        return this;
    }

    // Links a ClientFragment with this UserFeature.
    public UserFeature WithCLient(ClientFragment client)
    {
        this.client = client;
        client.OnReceived += ParseCommand;
        return this;
    }

    /// <summary>
    /// Attempts to register a user on the client side.
    /// The OnRegister handler int parameter is the response code:
    ///     1 - Invalid email
    ///     2 - Invalid username
    ///     3 - Invalid password
    ///     0 - Success
    /// </summary>
    public void Register(string email, string username, string password, Action<int> OnRegister)
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
    ///     1 - Invalid details
    ///     0 - Success
    /// </summary>
    public void Login(string username, string password, Action<int> OnLogin)
    {
        if (client == null)
            return;
        
        if (this.OnLogin == null)
            this.OnLogin = OnLogin;

        client.Execute("login", [username, password]);
    }

    // Attempts to register a user on the server side.
    private void Register(int id, string email, string username, string password)
    {
        if (server == null || database == null)
            return;

        var res1 = database.Query("SELECT * FROM users WHERE email = @email", ["@email"], [email]);
        if (!(email.Length > 0 && email.Length <= EMAIL_LENGTH) || (res1 != null && res1.Count > 0))
        {
            server.Execute("register_response", ["1"], id);
            return;
        }
        var res2 = database.Query("SELECT * FROM users WHERE username = @uname", ["@uname"], [username]);
        if (!(username.Length > 0 && username.Length <= UNAME_LENGTH) || (res2 != null && res2.Count > 0))
        {
            server.Execute("register_response", ["2"], id);
            return;
        }
        if (!(password.Length > 0 && password.Length <= PWD_LENGTH))
        {
            server.Execute("register_response", ["3"], id);
            return;
        }

        string hash = CryptoFragment.Hash(password);
        database.NonQuery("INSERT INTO users (email, username, password) VALUES (@email, @uname, @pwd)", ["@email", "@uname", "@pwd"], [email, username, hash]);
        server.Execute("register_response", ["0"], id);
        logger?.Log(LogLevel.INFO, $"Added a user named: {username}.");
    }

    // Attempts to login to a user on the server side.
    private void Login(int id, string username, string password)
    {
        if (server == null || database == null)
            return;
        
        string hash = CryptoFragment.Hash(password);
        var res = database.Query("SELECT * FROM users WHERE username = @uname AND password = @pwd", ["@uname", "@pwd"], [username, hash]);
        if (res.Count <= 0)
        {
            server.Execute("login_response", ["1"], id);
            return;
        }

        server.Execute("login_response", ["0"], id);
        logger?.Log(LogLevel.INFO, $"Signed in to a user named: {username}.");
    }

    // Parses commands on the server side.
    private void ParseCommand(int id, string cmd, string[] args)
    {
        switch (cmd)
        {
            case "register":
                if (args.Length != 3)
                    return;
                
                Register(id, args[0], args[1], args[2]);
                break;
            case "login":
                if (args.Length != 2)
                    return;
                
                Login(id, args[0], args[1]);
                break;
        }
    }

    // Parses commands on the client side.
    private void ParseCommand(string cmd, string[] args)
    {
        switch (cmd)
        {
            case "register_response":
                OnRegister?.Invoke(int.Parse(args[0]));
                break;
            case "login_response":
                OnLogin?.Invoke(int.Parse(args[0]));
                break;
        }
    }
}