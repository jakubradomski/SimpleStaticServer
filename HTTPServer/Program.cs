using Microsoft.Extensions.Configuration;

namespace HTTPServer;

class Program
{
    static void Main(string[] args)
    {
        IConfiguration config = new ConfigurationBuilder()
            .AddJsonFile("config.json")
            .Build();

        AppConfig.GetInstance().LoadFrom(config);

        System.Console.WriteLine(AppConfig.GetInstance().RootPath);
        System.Console.WriteLine(AppConfig.GetInstance().Port);

        var _fileProvider = new FileSystemFileProvider(AppConfig.GetInstance().RootPath);
        var _mimeMapper = new DefaultMimeMapper();
        StaticFileHandler _handler = new StaticFileHandler(_fileProvider, _mimeMapper);

        WebServer server = new WebServer(_handler, AppConfig.GetInstance().Port);
        server.Start();
    }
}
