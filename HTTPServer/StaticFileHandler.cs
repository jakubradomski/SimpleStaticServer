using System.Text;
using Microsoft.Extensions.Logging;

namespace HTTPServer;

internal class StaticFileHandler : IRequestHandler
{
    private readonly IFileProvider _fileProvider;
    private readonly IMimeMapper _mimeMapper;
    private readonly IDirectoryListingBuilder _listingBuilder;
    private readonly ILogger<StaticFileHandler> _logger;

    public StaticFileHandler(IFileProvider fileProvider, IMimeMapper mimeMapper, IDirectoryListingBuilder listingBuilder, ILogger<StaticFileHandler> logger)
    {
        _fileProvider = fileProvider;
        _mimeMapper = mimeMapper;
        _listingBuilder = listingBuilder;
        _logger = logger;
    }

    public HttpResponse Handle(HttpRequest request)
    {
        string rawPath = request.Path == "/" ? AppConfig.GetInstance().DefaultFile : request.Path;
        string path = Uri.UnescapeDataString(rawPath);
        string fullPath = _fileProvider.GetFullPath(path);

        if (_fileProvider.FileExists(fullPath)) // Handle File
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
        else if (_fileProvider.DirectoryExists(fullPath)) // Handle Directory
        {
            string html = _listingBuilder.BuildHtmlListing(fullPath, path, _fileProvider);
            var htmlStream = new MemoryStream(Encoding.UTF8.GetBytes(html));

            return new HttpResponse
            {
                StatusCode = "200 OK",
                ContentType = "text/html",
                Body = htmlStream,
                ContentLength = htmlStream.Length
            };
        }

        _logger.LogWarning($"File {fullPath} was not found");

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
