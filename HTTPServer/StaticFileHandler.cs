using System.Text;
using Microsoft.Extensions.Logging;

namespace HTTPServer;

internal class StaticFileHandler : IRequestHandler
{
    private readonly IFileProvider _fileProvider;
    private readonly IMimeMapper _mimeMapper;
    private readonly IDirectoryListingBuilder _listingBuilder;
    private readonly ILogger<StaticFileHandler> _logger;
    private readonly IServerSettings _settings;

    public StaticFileHandler(IFileProvider fileProvider, IMimeMapper mimeMapper, IDirectoryListingBuilder listingBuilder, ILogger<StaticFileHandler> logger, IServerSettings settings)
    {
        _fileProvider = fileProvider;
        _mimeMapper = mimeMapper;
        _listingBuilder = listingBuilder;
        _logger = logger;
        _settings = settings;
    }

    public HttpResponse Handle(HttpRequest request)
    {
        string rawPath = request.Path == "/" ? _settings.DefaultFile : request.Path;
        string path = Uri.UnescapeDataString(rawPath);
        string fullPath;
        
        try
        {
            fullPath = _fileProvider.GetFullPath(path);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized access to path: {Path}", path);
            return Create404Response();
        }

        Console.WriteLine($"path:{path} requestPath:{fullPath}");

        if (_fileProvider.FileExists(fullPath))
        {
            return HandleFile(path, fullPath);
        }
        else if (_fileProvider.DirectoryExists(fullPath))
        {
            return HandleDirectory(path, fullPath);
        }
        else
        {
            _logger.LogWarning($"File {fullPath} was not found");
            return Create404Response();
        }
    }

    private HttpResponse HandleFile(string path, string fullPath)
    {
        Stream fileStream = _fileProvider.Open(fullPath);
        return new HttpResponse
        {
            StatusCode = "200 OK",
            ContentType = _mimeMapper.Map(Path.GetExtension(path)),
            Body = fileStream,
            ContentLength = _fileProvider.GetSize(fullPath)
        };
    }

    private HttpResponse HandleDirectory(string path, string fullPath)
    {
        string html = _listingBuilder.BuildHtmlListing(fullPath, path);

        var htmlStream = new MemoryStream(Encoding.UTF8.GetBytes(html));
        return new HttpResponse
        {
            StatusCode = "200 OK",
            ContentType = "text/html",
            Body = htmlStream,
            ContentLength = htmlStream.Length
        };
    }

    private HttpResponse Create404Response()
    {
        var body = new MemoryStream(Encoding.UTF8.GetBytes("<h1>404 Not Found</h1>"));
        return new HttpResponse
        {
            StatusCode = "404 Not Found",
            ContentType = "text/html",
            Body = body,
            ContentLength = body.Length
        };
    }
}
