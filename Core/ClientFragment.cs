using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Winreels.Core;

/// <summary>
/// This class is used for creating a tcp client that listens for commands from the server.
/// It can also execute commands that will be sent to the server and processed there.
/// </summary>
public class ClientFragment
{
    public static readonly string[] UnprintedCommands = [
        "public_key",
        "aes_key",
        "upload",
        "download",
        "upload_response",
        "download_response"  
    ];

    public static readonly string EndSign = "<|EOM|>";
    public static readonly string ArgSign = "<|EON|>";
    public static readonly string SepSign = "<|EOA|>";

    public Action<Socket>? OnConnection;
    public Action<string, string[]>? OnSent;
    public Action<string, string[]>? OnReceived;
    public Func<int, string, string[], bool, string[]>? DoEncryption;
    public Func<int, string, string[], bool, string[]>? DoDecryption;

    private LoggerFragment? logger;
    private readonly string serverAddress;
    private readonly int serverPort;
    private Socket? client;

    // Links a LoggerFragment with this ClientFragment.
    public ClientFragment(string serverAddress, int serverPort)
    {
        this.serverAddress = serverAddress;
        this.serverPort = serverPort;
    }

    // Links a LoggerFragment with this ServerFragment.
    public ClientFragment WithLogger(LoggerFragment logger)
    {
        this.logger = logger;
        return this;
    }

    // Connects to a server and listens for commands from it.
    public void Connect()
    {
        try
        {
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(serverAddress), serverPort);
            client = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            client.Connect(endPoint);
            logger?.Log(LogLevel.INFO, $"Connected to the server with address, port: {serverAddress}, {serverPort}.");

            Thread listeningThread = new Thread(() =>
            {
                var sb = new StringBuilder();
                while (true)
                {
                    byte[] buffer = new byte[8192];
                    int amount = client.Receive(buffer);
                    if (amount <= 0)
                        break;
                    string chunk = Encoding.UTF8.GetString(buffer, 0, amount);
                    sb.Append(chunk);

                    string all = sb.ToString();
                    int endIndex;
                    while ((endIndex = all.IndexOf(EndSign, StringComparison.Ordinal)) >= 0)
                    {
                        string message = all.Substring(0, endIndex);
                        all = all.Substring(endIndex + EndSign.Length);

                        if (string.IsNullOrEmpty(message))
                            continue;

                        string[] splitCmd = message.Split(ArgSign);
                        string name = splitCmd[0];
                        string[] args = [];
                        if (splitCmd.Length > 1)
                            args = splitCmd[1].Split(SepSign);
                        if (DoDecryption != null)
                            args = DoDecryption(-1, name, args, false);

                        if (!UnprintedCommands.Contains(name))
                            logger?.Log(LogLevel.INFO, $"Received a command: {message}.");
                        OnReceived?.Invoke(name, args);
                    }

                    sb.Clear();
                    sb.Append(all);
                }
            });
            listeningThread.Start();
            OnConnection?.Invoke(client);
        }
        catch (Exception ex)
        {
            logger?.Log(LogLevel.ERROR, $"Failed to connect to a server, {ex}.");
        }
    }

    // Executes a command based on its name and args.
    public void Execute(string cmd, string[] args)
    {
        if (DoEncryption != null)
            args = DoEncryption(-1, cmd, args, false);

        byte[] buffer = Encoding.UTF8.GetBytes(
            $"{cmd}{(args.Length > 0 ? ArgSign + string.Join(SepSign, args) : "")}{EndSign}"
        );
        client?.Send(buffer);
        OnSent?.Invoke(cmd, args);
    }

    // Closes the client socket.
    public void Close()
    {
        client?.Close();
        client = null;
    }
}