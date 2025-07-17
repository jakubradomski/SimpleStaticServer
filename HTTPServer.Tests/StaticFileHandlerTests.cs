using System.Text;
using Microsoft.Extensions.Logging;
using Moq;

namespace HTTPServer.Tests;

public class StaticFileHandlerTests
{
    private readonly Mock<IFileProvider> _fileProviderMock = new();
    private readonly Mock<IMimeMapper> _mimeMapperMock = new();
    private readonly Mock<IDirectoryListingBuilder> _listingBuilderMock = new();
    private readonly Mock<IServerSettings> _settingsMock = new();
    private readonly ILogger<StaticFileHandler> _logger = new LoggerFactory().CreateLogger<StaticFileHandler>();

    private StaticFileHandler CreateHandler() =>
        new(_fileProviderMock.Object, _mimeMapperMock.Object, _listingBuilderMock.Object, _logger, _settingsMock.Object);

    [Fact]
    public void Handle_FileExists_ReturnFileResponse()
    {
        var request = new HttpRequest { Path = "/test.txt" };
        string fullPath = "/wwwroot/test.txt";

        _settingsMock.Setup(s => s.RootPath).Returns("wwwroot");
        _fileProviderMock.Setup(f => f.GetFullPath("/test.txt")).Returns(fullPath);
        _fileProviderMock.Setup(f => f.FileExists(fullPath)).Returns(true);
        _fileProviderMock.Setup(f => f.Open(fullPath))
            .Returns(new MemoryStream(Encoding.UTF8.GetBytes("file content")));
        _fileProviderMock.Setup(f => f.GetSize(fullPath)).Returns(12);

        _mimeMapperMock.Setup(m => m.Map(".txt")).Returns("text/plain");

        var handler = CreateHandler();

        var response = handler.Handle(request);

        Assert.Equal("200 OK", response.StatusCode);
        Assert.Equal("text/plain", response.ContentType);
        Assert.Equal(12, response.ContentLength);
    }

    [Fact]
    public void Handle_DirectoryExists_ReturnsHtmlListing()
    {
        var request = new HttpRequest { Path = "/dir" };
        string fullPath = "/wwwroot/dir";

        _fileProviderMock.Setup(f => f.GetFullPath("/dir")).Returns(fullPath);
        _fileProviderMock.Setup(f => f.FileExists(fullPath)).Returns(false);
        _fileProviderMock.Setup(f => f.DirectoryExists(fullPath)).Returns(true);

        _listingBuilderMock.Setup(l =>
            l.BuildHtmlListing(fullPath, "/dir"))
             .Returns("<html>listing</html>");

        var handler = CreateHandler();

        var response = handler.Handle(request);

        Assert.Equal("200 OK", response.StatusCode);
        Assert.Equal("text/html", response.ContentType);
        using var reader = new StreamReader(response.Body, Encoding.UTF8);
        string content = reader.ReadToEnd();
        Assert.Contains("listing", content);
    }

    [Fact]
    public void Handle_FileNotFound_Returns404()
    {
        var request = new HttpRequest { Path = "/empty" };
        string fullPath = "/wwwroot/empty";

        _fileProviderMock.Setup(f => f.GetFullPath("/dir")).Returns(fullPath);
        _fileProviderMock.Setup(f => f.FileExists(fullPath)).Returns(false);
        _fileProviderMock.Setup(f => f.DirectoryExists(fullPath)).Returns(false);

        var handler = CreateHandler();

        var response = handler.Handle(request);

        Assert.Equal("404 Not Found", response.StatusCode);
        Assert.Equal("text/html", response.ContentType);
        using var reader = new StreamReader(response.Body, Encoding.UTF8);
        string content = reader.ReadToEnd();
        Assert.Contains("404 Not Found", content);
    }

    [Fact]
    public void Handle_RootPath_ReturnsDefaultFile()
    {
        var request = new HttpRequest { Path = "/" };
        string defaultFile = "index.html";
        string fullPath = $"/wwwroot/{defaultFile}";

        _settingsMock.Setup(s => s.DefaultFile).Returns(defaultFile);
        _fileProviderMock.Setup(f => f.GetFullPath(defaultFile)).Returns(fullPath);
        _fileProviderMock.Setup(f => f.FileExists(fullPath)).Returns(true);
        _fileProviderMock.Setup(f => f.Open(fullPath))
            .Returns(new MemoryStream(Encoding.UTF8.GetBytes("<html>Default Content</html>")));
        _fileProviderMock.Setup(f => f.GetSize(fullPath)).Returns(28);

        _mimeMapperMock.Setup(m => m.Map(".html")).Returns("text/html");

        var handler = CreateHandler();

        var response = handler.Handle(request);

        Assert.Equal("200 OK", response.StatusCode);
        Assert.Equal("text/html", response.ContentType);
        Assert.Equal(28, response.ContentLength);
    }
}