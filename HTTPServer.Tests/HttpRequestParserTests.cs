namespace HTTPServer.Tests;

public class HttpRequestParserTests
{
    [Fact]
    public void Parse_ValidGetRequest_ReturnsExpectedValues()
    {
        string input = "GET /index.html HTTP/1.1\r\nHost: Test";
        var result = HttpRequestParser.Parse(input);

        Assert.Equal("GET", result.Method);
        Assert.Equal("/index.html", result.Path);
        Assert.Equal("HTTP/1.1", result.HttpVersion);
    }

    [Fact]
    public void Parse_ValidRequestHeaders_ReturnsExpectedValues()
    {
        string input = "GET /index.html HTTP/1.1\r\nHost: localhost\r\nUser-Agent: Test";
        HttpRequest result = HttpRequestParser.Parse(input);

        Assert.Equal("localhost", result.Headers["Host"]);
        Assert.Equal("Test", result.Headers["User-Agent"]);
    }

    [Fact]
    public void Parse_ThrowsException_WithExpectedMessage()
    {
        string input = "GET /index.html\r\nHost: Test";
        var ex = Assert.Throws<Exception>(() => HttpRequestParser.Parse(input));
        Assert.Contains("Invalid request header!", ex.Message);
    }

    [Fact]
    public void Parse_ValidQueryParams_ReturnsExpectedValues()
    {
        string input = "GET /index.html?page=1&limit=50 HTTP/1.1\r\nHost: Test";
        var result = HttpRequestParser.Parse(input);

        Assert.Equal("1", result.QueryParams["page"]);
        Assert.Equal("50", result.QueryParams["limit"]);
    }
}
