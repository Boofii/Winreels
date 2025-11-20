using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;

namespace Winreels.Core;

/// <summary>
/// This class is used to wrap a ServerFragment or a ClientFragment with a secure connection.
/// The sender always encrypts messages before they are sent and the receiver decrypts them.
/// It is important for the CryptoFragment to be initialized before the ServerFragment and the ClientFragment.
/// </summary>
public class CryptoFragment
{
    private LoggerFragment? logger;
    private ServerFragment? server;
    private ClientFragment? client;
    private string? publicPath;
    private RSA? publicKey;
    private RSA? privateKey;
    public readonly Dictionary<int, Aes> commKeys = [];
    public Aes? commKey;

    // Links a LoggerFragment with this CryptoFragment.
    public CryptoFragment WithLogger(LoggerFragment logger)
    {
        this.logger = logger;
        return this;
    }

    // Links a ServerFragment with this CryptoFragment, the private path should include a pem file.
    public CryptoFragment WithServer(ServerFragment server, string publicPath, string privatePath)
    {
        this.server = server;
        this.publicPath = publicPath;
        server.OnConnection += SendPublicKey;
        server.OnReceived += ReceiveAesKey;
        server.DoEncryption += Encrypt;
        server.DoDecryption += Decrypt;
        try
        {
            RSA rsa = RSA.Create();
            string content = File.ReadAllText(privatePath);
            rsa.ImportFromPem(content);
            this.privateKey = rsa;
        }
        catch (Exception ex)
        {
            logger?.Log(LogLevel.ERROR, $"Failed to set up a CryptoFragment for server: {ex}.");
        }
        return this;
    }

    // Links a ClientFragment with this CryptoFragment, the public path should include a pem file.
    public CryptoFragment WithClient(ClientFragment client)
    {
        this.client = client;
        client.OnReceived += ReceivePublicKey;
        client.DoEncryption += Encrypt;
        client.DoDecryption += Decrypt;
        return this;
    }

    // Hashes a string (Sha-256).
    public byte[] Hash(string str)
    {
        byte[] buffer = Encoding.UTF8.GetBytes(str);
        byte[] result = SHA256.HashData(buffer);
        return result;
    }

    // Sends the server's public key to the newly connected client.
    private void SendPublicKey(int id, Socket client)
    {
        if (server == null || publicPath == null)
        {
            logger?.Log(LogLevel.ERROR, $"Tried to send the public key to client {id} but server_fragment/public path was null.");
            return;
        }

        string content = File.ReadAllText(publicPath);
        server.Execute("public_key", [content], id);
    }

    // Receives on the client side the server's public key and sends a new AES key to the server.
    private void ReceivePublicKey(string cmd, string[] args)
    {
        if (client == null)
        {
            logger?.Log(LogLevel.ERROR, $"Tried to receive the public key for client {args[0]} but client_fragment was null.");
            return;
        }
        if (!cmd.Equals("public_key"))
            return;

        string content = args[1];
        RSA rsa = RSA.Create();
        rsa.ImportFromPem(content);
        this.publicKey = rsa;

        Aes aes = Aes.Create();
        aes.KeySize = 256;
        aes.GenerateKey();
        aes.GenerateIV();
        this.commKey = aes;

        byte[] encryptedKey = rsa.Encrypt(aes.Key, RSAEncryptionPadding.OaepSHA3_256);
        byte[] encryptedIV  = rsa.Encrypt(aes.IV, RSAEncryptionPadding.OaepSHA3_256);
        client.Execute("aes_key", [Convert.ToBase64String(encryptedKey), Convert.ToBase64String(encryptedIV)]);
    }

    // Receives on the server side the newly connected client's AES key.
    private void ReceiveAesKey(string cmd, string[] args)
    {
        if (server == null || privateKey == null)
        {
            logger?.Log(LogLevel.ERROR, $"Tried to receive the aes key for client {args[0]} but server_fragment/private_key was null.");
            return;
        }
        if (!cmd.Equals("aes_key"))
            return;

        int id = int.Parse(args[0]);
        byte[] key = privateKey.Decrypt(Convert.FromBase64String(args[1]), RSAEncryptionPadding.OaepSHA3_256);
        byte[] iv = privateKey.Decrypt(Convert.FromBase64String(args[2]), RSAEncryptionPadding.OaepSHA3_256);
        Aes aes = Aes.Create();
        aes.KeySize = 256;
        aes.Key = key;
        aes.IV = iv;
        this.commKeys[id] = aes;
        logger?.Log(LogLevel.INFO, $"Key exchange done with a client: {id}.");
    }

    private string[] Encrypt(string cmd, string[] args, bool serverSide = false)
    {
        if (cmd.Equals("aes_key") || cmd.Equals("public_key"))
            return args;

        Aes? aes = serverSide ? commKeys[int.Parse(args[0])] : commKey;
        if (aes == null)
        {
            logger?.Log(LogLevel.ERROR, "Failed to find an aes key for encryption.");
            return [];
        }
        string[] newArgs = new string[args.Length];
        newArgs[0] = args[0];
        for (int i = 1; i < args.Length; i++)
        {
            using var encryptor = aes.CreateEncryptor();
            byte[] buffer = Encoding.UTF8.GetBytes(args[i]);
            byte[] encryption = encryptor.TransformFinalBlock(buffer, 0, buffer.Length);
            newArgs[i] = Convert.ToBase64String(encryption);
        }
        return newArgs;
    }

    private string[] Decrypt(string cmd, string[] args, bool serverSide = false)
    {
        if (cmd.Equals("aes_key") || cmd.Equals("public_key"))
            return args;
        
        Aes? aes = serverSide ? commKeys[int.Parse(args[0])] : commKey;
        if (aes == null)
        {
            logger?.Log(LogLevel.ERROR, "Failed to find an aes key for decryption.");
            return [];
        }
        string[] newArgs = new string[args.Length];
        newArgs[0] = args[0];
        for (int i = 1; i < args.Length; i++)
        {
            using var decryptor = aes.CreateDecryptor();
            byte[] buffer = Convert.FromBase64String(args[i]);
            byte[] decryption = decryptor.TransformFinalBlock(buffer, 0, buffer.Length);
            newArgs[i] = Encoding.UTF8.GetString(decryption);
        }
        return newArgs;
    }

    // Closes the RSA and AES connections.
    public void Close()
    {
        publicKey?.Dispose();
        privateKey?.Dispose();
        commKey?.Dispose();
        foreach (int id in commKeys.Keys)
        {
            commKeys[id].Dispose();
            commKeys.Remove(id);
        }
    }
}