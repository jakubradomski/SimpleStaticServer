namespace HTTPServer;

public class FileSystemFileProvider : IFileProvider
{
    private readonly string _root;

    public FileSystemFileProvider(string root)
    {
        _root = root;
    }

    public string GetFullPath(string path) => Path.Combine(_root, path.TrimStart('/').Replace("/", "\\"));

    public bool Exists(string path) => File.Exists(path);
    public Stream Open(string path) => File.OpenRead(path);
    public long GetSize(string path) => new FileInfo(path).Length;
}
