using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.CommandLine;
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

        IConfiguration config;
        try
        {
            config = new ConfigurationBuilder()
                .AddJsonFile("config.json", optional: true, reloadOnChange: false)
                .AddCommandLine(args, configMapping)
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
