namespace HTTPServer;

public interface IFileProvider
{
    bool FileExists(string path);
    bool DirectoryExists(string path);
    Stream Open(string path);
    long GetSize(string path);
    string GetFullPath(string path);
    string[] GetFilesInDirectory(string path);
    string[] GetDirectoriesInDirectory(string path);
}