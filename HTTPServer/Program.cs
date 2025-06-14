namespace HTTPServer;

class Program
{
    static void Main(string[] args)
    {
        var _fileProvider = new FileSystemFileProvider("/home/kuba/server");
        var _mimeMapper = new DefaultMimeMapper();
        StaticFileHandler _handler = new StaticFileHandler(_fileProvider, _mimeMapper);

        WebServer server = new WebServer(_handler, 8080);
        server.Start();
    }
}
