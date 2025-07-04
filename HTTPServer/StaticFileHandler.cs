using System.Text;

namespace HTTPServer;

internal class StaticFileHandler : IRequestHandler
{
    private readonly IFileProvider _fileProvider;
    private readonly IMimeMapper _mimeMapper;

    public StaticFileHandler(IFileProvider fileProvider, IMimeMapper mimeMapper)
    {
        _fileProvider = fileProvider;
        _mimeMapper = mimeMapper;
    }

    public HttpResponse Handle(HttpRequest request)
    {
        string path = request.Path == "/" ? "/index.html" : request.Path;
        string fullPath = _fileProvider.GetFullPath(path);

        if (!_fileProvider.Exists(fullPath))
        {
            Console.WriteLine("file not found!");

            var body = new MemoryStream(Encoding.UTF8.GetBytes("<h1>404 File Not Found</h1>"));
            return new HttpResponse
            {
                StatusCode = "404 Not Found",
                ContentType = "text/html",
                Body = body,
                ContentLength = body.Length
            };
        }

        Stream fileStream = _fileProvider.Open(fullPath);
        return new HttpResponse
        {
            StatusCode = "200 OK",
            ContentType = _mimeMapper.Map(Path.GetExtension(path)),
            Body = fileStream,
            ContentLength = _fileProvider.GetSize(fullPath)
        };
    }
}
