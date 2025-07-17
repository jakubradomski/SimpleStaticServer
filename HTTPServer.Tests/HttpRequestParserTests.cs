namespace HTTPServer.Tests;

public class HttpRequestParserTests
{
    [Theory]
    [InlineData("GET /index.html HTTP/1.1", "GET", "/index.html", "HTTP/1.1")]
    [InlineData("GET / HTTP/1.1", "GET", "/", "HTTP/1.1")]
    [InlineData("GET /cat.png HTTP/2", "GET", "/cat.png", "HTTP/2")]
    [InlineData("POST /cat.png HTTP/2", "POST", "/cat.png", "HTTP/2")]
    public void Parse_ValidGetRequest_ReturnsExpectedValues(string input, string method, string path, string HttpVersion)
    {
        var result = HttpRequestParser.Parse(input);

        Assert.Equal(method, result.Method);
        Assert.Equal(path, result.Path);
        Assert.Equal(HttpVersion, result.HttpVersion);
    }

    [Theory]
    [InlineData("GET /index.html HTTP/1.1\r\nHost: localhost\r\nUser-Agent: Test", "Host", "localhost")]
    [InlineData("GET /index.html HTTP/1.1\r\nHost: localhost:8080\r\nUser-Agent: Chrome", "Host", "localhost:8080")]
    [InlineData("GET /index.html HTTP/1.1\r\nHost: localhost\r\nUser-Agent: Chrome", "User-Agent", "Chrome")]
    [InlineData("GET /index.html HTTP/1.1\r\nHost: localhost\r\nUser-Agent: Firefox", "User-Agent", "Firefox")]
    public void Parse_ValidRequestHeaders_ReturnsExpectedValues(string input, string key, string expected)
    {
        HttpRequest result = HttpRequestParser.Parse(input);
        Assert.Equal(expected, result.Headers[key]);
    }

    [Theory]
    [InlineData("GET /index.html")]
    [InlineData("/index.html HTTP/1.1")]
    [InlineData("GET HTTP/1.1")]
    public void Parse_ThrowsException_WithExpectedMessage(string input)
    {
        var ex = Assert.Throws<FormatException>(() => HttpRequestParser.Parse(input));
        Assert.Contains("Invalid HTTP request line format.", ex.Message);
    }

    [Theory]
    [InlineData("GET /index.html?limit=10&page=2&query=cat HTTP/1.1", "limit", "10")]
    [InlineData("GET /index.html?limit=10&page=2&query=cat HTTP/1.1", "page", "2")]
    [InlineData("GET /index.html?limit=10&page=2&query=cat HTTP/1.1", "query", "cat")]    
    public void Parse_ValidQueryParams_ReturnsExpectedValues(string input, string key, string expected)
    {
        var result = HttpRequestParser.Parse(input);
        Assert.Equal(expected, result.QueryParams[key]);
    }
}
