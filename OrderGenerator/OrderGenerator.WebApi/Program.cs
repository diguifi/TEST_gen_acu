using OrderGenerator.WebApi.Initiator;
using QuickFix;
using QuickFix.Store;
using QuickFix.Transport;
using OrderGenerator.WebApi.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin() //apenas para testes.
    );
});

var app = builder.Build();
app.UseCors();
app.UseWebSockets();

var fixApp = new FixClient();
var settings = new SessionSettings("initiator.cfg");
var storeFactory = new FileStoreFactory(settings);
using ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
{
    builder.SetMinimumLevel(LogLevel.Trace);
    builder.AddConsole();
});
var initiator = new SocketInitiator(fixApp, storeFactory, settings, loggerFactory);
initiator.Start();

// --- ESSA IDEIA BESTA ESTAVA ESTRAGANDO O RETORNO DO EXECUTOR POR ALGUM MOTIVO (detalhes nas anotações) ---
// Console.WriteLine("Aguardando OrderAccumulator...");
// await fixApp.WhenLoggedOn;
// Console.WriteLine("FIX logado com sucesso");

app.MapWebSocketRoutes(fixApp);
app.Run();
