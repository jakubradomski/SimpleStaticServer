namespace HTTPServer;

internal static class HttpRequestParser
{
    public static HttpRequest Parse(string raw)
    {
        var lines = raw.Split("\r\n", StringSplitOptions.RemoveEmptyEntries);

        var requestLine = lines[0].Split(' ');
        if (requestLine.Length != 3)
            throw new Exception("Invalid request header!");
            
        var method = requestLine[0];
        var path = requestLine[1];
        var version = requestLine[2];
        

        var headers = new Dictionary<string, string>();
        foreach (var line in lines.Skip(1))
        {
            int idx = line.IndexOf(':');
            if (idx > 0)
            {
                var key = line[..idx];
                var val = line[(idx + 1)..];
                headers.Add(key.Trim(), val.Trim());
            }
        }

        var query = new Dictionary<string, string>();
        var pathParts = path.Split('?', 2);
        if (pathParts.Length > 1)
        {
            foreach (var pair in pathParts[1].Split('&'))
            {
                var parts = pair.Split('=');
                if (parts.Length == 2)
                {
                    query.Add(parts[0], parts[1]);
                }
            }
        }

        return new HttpRequest
        {
            Method = method,
            Path = path,
            HttpVersion = version,
            Headers = headers,
            QueryParams = query
        };
    }
}