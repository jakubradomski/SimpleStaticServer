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
    public int Port { get; set; }

    private AppConfig() { }
}