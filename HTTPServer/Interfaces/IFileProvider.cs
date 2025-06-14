namespace HTTPServer;

public interface IFileProvider
{
    bool Exists(string path);
    Stream Open(string path);
    long GetSize(string path);
    string GetFullPath(string path);
}