namespace HTTPServer;

internal class HttpResponse
{
    public string StatusCode { get; set; } = "200 OK";
    public string ContentType { get; set; } = "text/html";
    public Stream Body { get; set; }
    public long ContentLength { get; set; }
    public Dictionary<string, string> Headers { get; set; }
}