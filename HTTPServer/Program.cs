using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace HTTPServer;

class Program
{
    static void Main(string[] args)
    {
        using ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
        });

        ILogger<Program> programLogger = loggerFactory.CreateLogger<Program>();

        IConfiguration config;
        try
        {
            config = new ConfigurationBuilder()
                .AddJsonFile("config.json")
                .Build();
            AppConfig.GetInstance().LoadFrom(config);
        }
        catch (Exception ex)
        {
            programLogger.LogError(ex, "Failed to load config.json. Using defaults.");
            AppConfig.GetInstance().LoadDefaults();
        }

        var _fileProvider = new FileSystemFileProvider(AppConfig.GetInstance().RootPath);
        var _mimeMapper = new DefaultMimeMapper();
        var _listingBuilder = new DirectoryListingBuilder();

        var _handlerLogger = loggerFactory.CreateLogger<StaticFileHandler>();
        var _webServerLogger = loggerFactory.CreateLogger<WebServer>();

        StaticFileHandler _handler = new StaticFileHandler(_fileProvider, _mimeMapper, _listingBuilder, _handlerLogger);

        WebServer server = new WebServer(_handler, _webServerLogger, AppConfig.GetInstance().Port);
        server.Start();
    }
}
