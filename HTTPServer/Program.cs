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

        var _fileProvider = new FileSystemFileProvider(AppConfig.GetInstance().RootPath);
        var _mimeMapper = new DefaultMimeMapper();
        StaticFileHandler _handler = new StaticFileHandler(_fileProvider, _mimeMapper);

        int minThreads = AppConfig.GetInstance().MinThreads;
        int maxThreads = AppConfig.GetInstance().MaxThreads;

        ThreadPool.SetMinThreads(minThreads, minThreads);
        ThreadPool.SetMaxThreads(maxThreads, maxThreads);

        WebServer server = new WebServer(_handler, AppConfig.GetInstance().Port);
        server.Start();
    }
}
