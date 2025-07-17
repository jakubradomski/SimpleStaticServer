using Moq;

namespace HTTPServer.Tests;

public class DirectoryListingBuilderTests
{
    private readonly Mock<IFileProvider> _fileProviderMock = new();

    private DirectoryListingBuilder CreateBuilder() =>
        new(_fileProviderMock.Object);

    [Theory]
    [InlineData()]
    public void Builder_Path_ReturnsValidListing()
    {
        string path = "/data";
        string requestPath = "wwwroot/data";

        var builder = CreateBuilder();

        _fileProviderMock.Setup(f => f.GetDirectoriesInDirectory("/data"))
            .Returns(new[] { "/data/dir1", "/data/dir2" });

        _fileProviderMock.Setup(f => f.GetFilesInDirectory("/data"))
            .Returns(new[] { "/data/file1.txt", "/data/file2.txt" });

        string actualHtml = builder.BuildHtmlListing(path, requestPath);

        _fileProviderMock.Verify(f => f.GetDirectoriesInDirectory(path), Times.Once);
        _fileProviderMock.Verify(f => f.GetFilesInDirectory(path), Times.Once);
        _fileProviderMock.VerifyNoOtherCalls();

        Assert.Contains("<!DOCTYPE html>", actualHtml);
        Assert.Contains("<html><head><meta charset='UTF-8'><title>Index of /data</title></head>", actualHtml);
        Assert.Contains("<body>", actualHtml);
        Assert.Contains("<h1>Index of wwwroot/data</h1>", actualHtml);
        Assert.Contains("<li><a href=\"wwwroot/\">../</a></li>", actualHtml);
        Assert.Contains("<li><a href=\"wwwroot/data/dir1\">dir1/</a></li>", actualHtml);
        Assert.Contains("<li><a href=\"wwwroot/data/dir2\">dir2/</a></li>", actualHtml);
        Assert.Contains("<li><a href=\"wwwroot/data/file1.txt\">file1.txt</a></li>", actualHtml);
        Assert.Contains("<li><a href=\"wwwroot/data/file2.txt\">file2.txt</a></li>", actualHtml);
        Assert.Contains("</ul>", actualHtml);
        Assert.Contains("</body></html>", actualHtml);
    }
}