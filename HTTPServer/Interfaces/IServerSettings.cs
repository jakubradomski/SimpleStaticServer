namespace HTTPServer;

public interface IServerSettings
{
    string RootPath { get; }
    string DefaultFile { get; }
    int Port { get; }
}