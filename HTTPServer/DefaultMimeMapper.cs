namespace HTTPServer;

public class DefaultMimeMapper : IMimeMapper
{
    public string Map(string extension) => extension.ToLower() switch
    {
        ".html" => "text/html",
        ".css" => "text/css",
        ".js" => "application/javascript",
        ".json" => "application/json",
        ".png" => "image/png",
        ".jpg" or ".jpeg" => "image/jpeg",
        ".gif" => "image/gif",
        _ => "application/octet-stream"
    };
}
