using Winreels.Core;
using WinReels.Feature;

namespace Winreels;

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