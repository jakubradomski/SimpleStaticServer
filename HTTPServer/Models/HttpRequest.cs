namespace HTTPServer;

internal class HttpRequest
{
    public string Method { get; set; }
    public string Path { get; set; }
    public string HttpVersion { get; set; }
    public Dictionary<string, string> Headers { get; set; }
    public Dictionary<string, string> QueryParams { get; set; }
}