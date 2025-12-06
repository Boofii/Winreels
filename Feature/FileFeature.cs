using System.Text;
using Winreels.Core;

namespace Winreels.Feature;

/// <summary>
/// This class is used to send and receive files to/from a server.
/// Upload files using the upload command and download them using the download command.
/// The maximum size for a file is 25 MB.
/// </summary>
public class FileFeature
{
    public const uint MAX_SIZE = 25000000;
    public const uint IO_RATE = 1000;

    private ServerFragment? server;
    private ClientFragment? client;
    private string? path;
    
    // Links a ServerFragment with this FileFeature
    public FileFeature WithServer(string name, ServerFragment server)
    {
        this.server = server;
        string temp = $"{name}";
        temp = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), temp);
        this.path = temp;
        Directory.CreateDirectory(path);
        return this;
    }

    // Links a ClientFragment with this FIleFeature
    public FileFeature WithClient(ClientFragment client)
    {
        this.client = client;
        return this;
    }

    public void Upload(string name, byte[] data)
    {
        if (client == null)
            return;
        
        double amount = Math.Ceiling((double) (data.Length / IO_RATE));
        uint index = 0;
        for (int i = 0; i < amount; i++)
        {
            byte[] current = new byte[IO_RATE];
            for (int j = 0; j < IO_RATE; j++)
            {
                if (index + j >= data.Length)
                    break;
                current[j] = data[index + j];
            }
            string encoded = Encoding.UTF8.GetString(current);
            client.Execute("upload", [name, i.ToString(), amount.ToString(), encoded]);
            Thread.Sleep(1);
            index += IO_RATE;
        }
    }
}