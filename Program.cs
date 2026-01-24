using Winreels.Core;
using Winreels.Feature;
using WinReels.Feature;

namespace Winreels;

/// <summary>
/// The entry point for the application.
/// </summary>
public static class Program
{
    public static ServerFragment? Server { get; private set; }
    public static ClientFragment? Client { get; private set; }
    public static UserFeature? ServerUmanager { get; private set; }
    public static UserFeature? ClientUmanager { get; private set; }
    public static VideoFeature? ServerVmanager { get; private set; }
    public static VideoFeature? ClientVmanager { get; private set; }

    [STAThread]
    private static void Main(string[] args)
    {
        char input = args.Length > 0 ? args[0][0] : 'c';
        switch (input)
        {
            case 's':
                Thread t1 = new Thread(() =>
                {
                    LoggerFragment logger1 = new LoggerFragment("server-log");
                    ServerFragment server = new ServerFragment("127.0.0.1", 4098)
                        .WithLogger(logger1);
                    CryptoFragment crypto1 = new CryptoFragment()
                        .WithServer(server, "public.pem", "private.pem")
                        .WithLogger(logger1);
                    UserFeature uf1 = new UserFeature(server)
                        .WithLogger(logger1);
                    VideoFeature vf1 = new VideoFeature(server)
                        .WithLogger(logger1);

                    server.Establish();
                    Server = server;
                    ServerUmanager = uf1;
                    ServerVmanager = vf1;
                });
                t1.Start();
                break;
            case 'c':
                Thread t2 = new Thread(() =>
                {
                    LoggerFragment logger2 = new LoggerFragment("client-log");
                    ClientFragment client = new ClientFragment("127.0.0.1", 4098)
                        .WithLogger(logger2);
                    CryptoFragment crypto2 = new CryptoFragment()
                        .WithClient(client)
                        .WithLogger(logger2);
                    UserFeature uf2 = new UserFeature(client)
                        .WithLogger(logger2);
                    VideoFeature vf2 = new VideoFeature(client)
                        .WithLogger(logger2);

                    client.Connect();
                    Client = client;
                    ClientUmanager = uf2;
                    ClientVmanager = vf2;

                    ApplicationConfiguration.Initialize();
                    Application.Run(new LoginInterface());
                });
                t2.SetApartmentState(ApartmentState.STA);
                t2.Start();
                break;
        }
    }
}