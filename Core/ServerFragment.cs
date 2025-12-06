using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Winreels.Core;

/// <summary>
/// This class is used for creating a tcp server that listens for commands from the client.
/// It can also execute commands that will be sent to the client and processed there.
/// </summary>
public class ServerFragment
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

    public readonly Dictionary<int, Socket> connections = [];
    public Action<int, Socket>? OnConnection;
    public Action<string, string[]>? OnSent;
    public Action<int, string, string[]>? OnReceived;
    public Func<int, string, string[], bool, string[]>? DoEncryption;
    public Func<int, string, string[], bool, string[]>? DoDecryption;

    private LoggerFragment? logger;
    private readonly string address;
    private readonly int port;
    private readonly int maxQueue;
    private readonly int maxConnections;
    private int currConnection = 0;
    private Socket? server;

    public ServerFragment(string address, int port, int maxQueue = 10, int maxConnections = 10)
    {
        this.address = address;
        this.port = port;
        this.maxQueue = maxQueue;
        this.maxConnections = maxConnections;
    }

    // Links a LoggerFragment with this ServerFragment.
    public ServerFragment WithLogger(LoggerFragment logger)
    {
        this.logger = logger;
        return this;
    }

    // Establishes a server and listens for commands from the client.
    public void Establish()
    {
        try
        {
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(address), port);
            server = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            server.Bind(endPoint);
            server.Listen(maxQueue);

            Thread acceptingThread = new Thread(() =>
            {
                while (currConnection < maxConnections)
                {
                    Socket client = server.Accept();
                    logger?.Log(LogLevel.INFO, $"Established a connection with client: {currConnection}.");
                    Thread listeningThread = new Thread(() =>
                    {
                        try
                        {
                            while (true)
                            {
                                byte[] buffer = new byte[4096];
                                int amount = client.Receive(buffer);
                                buffer = [.. buffer.Take(amount)];

                                string cmd = Encoding.UTF8.GetString(buffer);
                                if (cmd.Contains(EndSign))
                                {
                                    string[] messages = cmd.Split(EndSign);
                                    foreach (var message in messages)
                                    {
                                        if (string.IsNullOrEmpty(message))
                                            continue;
                                        string[] splitCmd = message.Split(ArgSign);
                                        string name = splitCmd[0];
                                        string[] args = [];
                                        if (splitCmd.Length > 1)
                                            args = splitCmd[1].Split(SepSign);
                                        int id = GetClientId(client);
                                        if (DoDecryption != null)
                                            args = DoDecryption(id, name, args, true);

                                        if (!UnprintedCommands.Contains(name))
                                            logger?.Log(LogLevel.INFO, $"Received a command: {message}.");
                                        OnReceived?.Invoke(id, name, args);
                                    }
                                }
                            }
                        }
                        catch (SocketException ex)
                        {
                            logger?.Log(LogLevel.WARNING, $"One client had disconnected: {ex}.");
                        }
                        finally
                        {
                            client.Close();
                            foreach (int id in connections.Keys)
                                if (connections[id].Equals(client))
                                    connections.Remove(id);
                        }
                    });
                    listeningThread.Start();
                    connections[currConnection] = client;
                    OnConnection?.Invoke(currConnection, client);
                    currConnection++;
                }
            });
            acceptingThread.Start();
        }
        catch (Exception ex)
        {
            logger?.Log(LogLevel.ERROR, $"Failed to establish a server, {ex}.");
        }
    }

    // Executes a command based on its name and args.
    public void Execute(string cmd, string[] args, int id = -1)
    {
        if (id == -1)
        {
            string[] originalArgs = args;
            foreach (int clientId in connections.Keys)
            {
                if (!connections[clientId].Connected)
                {
                    connections[clientId].Dispose();
                    continue;
                }
                string[] sendArgs = originalArgs;
                if (DoEncryption != null)
                    sendArgs = DoEncryption(clientId, cmd, originalArgs, true);
                byte[] buffer = Encoding.UTF8.GetBytes($"{cmd}{ArgSign}{string.Join(SepSign, sendArgs)}{EndSign}");
                connections[clientId].Send(buffer);
                OnSent?.Invoke(cmd, sendArgs);
            }
        }
        else
        {
            if (!connections.TryGetValue(id, out Socket? value))
            {
                logger?.Log(LogLevel.ERROR, $"Tried to execute a command for an unassigned client: {id}.");
                return;
            }
            if (!connections[id].Connected)
            {
                connections[id].Dispose();
                return;
            }
            string[] sendArgs = args;
            if (DoEncryption != null)
                sendArgs = DoEncryption(id, cmd, args, true);
            byte[] buffer = Encoding.UTF8.GetBytes($"{cmd}{ArgSign}{string.Join(SepSign, sendArgs)}{EndSign}");
            value.Send(buffer);
            OnSent?.Invoke(cmd, sendArgs);
        }
    }

    private int GetClientId(Socket client)
    {
        foreach (int id in connections.Keys)
        {
            Socket value = connections[id];
            if (client == value)
                return id;
        }
        return -1;
    }

    // Closes the server socket.
    public void Close()
    {
        server?.Close();
        server = null;
    }
}