using Microsoft.Extensions.Configuration;

namespace HTTPServer;

sealed class AppConfig : IServerSettings
{
    public string RootPath { get; private set; }
    public string DefaultFile { get; private set; }
    public int Port { get; private set; }
    public AppConfig(IConfiguration config)
    {
        var section = config.GetSection("Settings");

        RootPath = section["RootPath"] ?? RootPath;
        DefaultFile = section["DefaultFile"] ?? DefaultFile;

        if (int.TryParse(section["Port"], out int port))
            Port = port;
    }
    public void LoadDefaults()
    {
        RootPath = "wwwroot";
        DefaultFile = "index.html";
        Port = 8080;
    }
}