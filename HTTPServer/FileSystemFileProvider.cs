namespace HTTPServer;

public class FileSystemFileProvider : IFileProvider
{
    private readonly string _root;

    public FileSystemFileProvider(string root)
    {
        _root = root;
    }

    public string GetFullPath(string path)
    {
        string sanitized = path.TrimStart('/', '\\');
        string systemPath = sanitized.Replace('/', Path.DirectorySeparatorChar)
                                    .Replace('\\', Path.DirectorySeparatorChar);

        return Path.Combine(_root, systemPath);
    }

    public bool FileExists(string path) => File.Exists(path);
    public bool DirectoryExists(string path) => Directory.Exists(path);    
    public Stream Open(string path) => File.OpenRead(path);
    public long GetSize(string path) => new FileInfo(path).Length;
    public string[] GetFilesInDirectory(string path) => Directory.GetFiles(path);
    public string[] GetDirectoriesInDirectory(string path) => Directory.GetDirectories(path);

}
