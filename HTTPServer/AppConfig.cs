using Microsoft.Extensions.Configuration;

namespace HTTPServer;

sealed class AppConfig
{
    private static AppConfig _instance;

    public static AppConfig GetInstance()
    {
        if (_instance == null)
        {
            _instance = new AppConfig();
        }
        return _instance;
    }

    public string RootPath { get; set; }
    public string DefaultFile { get; set; }
    public int Port { get; set; }
    public int MinThreads { get; set; }
    public int MaxThreads { get; set; }

    private AppConfig() { }

    public void LoadFrom(IConfiguration config)
    {
        IConfiguration section = config.GetSection("Settings");

        RootPath = section["RootPath"] ?? "/";
        DefaultFile = section["DefaultFile"] ?? "/";
        Port = int.Parse(section["Port"] ?? "80");
        MinThreads = int.Parse(section["MinThreads"] ?? "10");
        MaxThreads = int.Parse(section["MaxThreads"] ?? "100");
    }
}