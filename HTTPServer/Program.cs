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

        var configMapping = new Dictionary<string, string>
        {
            {"-p", "Settings:Port" },
            {"-r", "Settings:RootPath" },
            {"-f", "Settings:DefaultFile" }
        };

        IServerSettings settings;
        IConfiguration config;
        try
        {
            config = new ConfigurationBuilder()
                .AddJsonFile("config.json", optional: true, reloadOnChange: false)
                .AddCommandLine(args, configMapping)
                .Build();
            settings = new AppConfig(config);
        }
        catch (Exception ex)
        {
            programLogger.LogError(ex, "Failed to load config.json. Using defaults.");
            settings = new AppConfig(new ConfigurationBuilder().Build());
        }

        var _fileProvider = new FileSystemFileProvider(settings.DefaultFile);
        var _mimeMapper = new DefaultMimeMapper();
        var _listingBuilder = new DirectoryListingBuilder();

        var _handlerLogger = loggerFactory.CreateLogger<StaticFileHandler>();
        var _webServerLogger = loggerFactory.CreateLogger<WebServer>();

        StaticFileHandler _handler = new StaticFileHandler(_fileProvider, _mimeMapper, _listingBuilder, _handlerLogger, settings);

        WebServer server = new WebServer(_handler, _webServerLogger, settings.Port);
        server.Start();
    }
}
