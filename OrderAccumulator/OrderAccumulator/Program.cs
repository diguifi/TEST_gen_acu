using Microsoft.Extensions.Logging;
using OrderAccumulator.Acceptor;
using QuickFix;
using QuickFix.Store;

class Program
{
    private const string HttpServerPrefix = "http://127.0.0.1:5080/";

    [STAThread]
    static void Main()
    {
        var settings = new SessionSettings("acceptor.cfg");
        var app = new FixApplication();
        var storeFactory = new FileStoreFactory(settings);
        using ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.SetMinimumLevel(LogLevel.Trace);
            builder.AddConsole();
        });
        var acceptor = new ThreadedSocketAcceptor(app, storeFactory, settings, loggerFactory);

        var srv = new HttpServer(HttpServerPrefix, settings);

        acceptor.Start();
        srv.Start();
        Console.WriteLine("OrderAccumulator rodando...");
        Console.ReadLine();
        srv.Stop();
        acceptor.Stop();
    }
}
