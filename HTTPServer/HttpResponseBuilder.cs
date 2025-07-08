using System.Net.Sockets;
using System.Reflection;
using System.Text;

namespace HTTPServer;

static class HttpResponseBuilder
{
    private const int BufferSize = 64 * 1024;

    public static async Task SendResponse(NetworkStream rawStream, HttpResponse response)
    {
        var headers = new StringBuilder();
        headers.Append($"HTTP/1.1 {response.StatusCode}\r\n");
        headers.Append($"Content-Type: {response.ContentType}\r\n");
        headers.Append($"Content-Length: {response.ContentLength}\r\n");
        headers.Append($"Connection: close\r\n");

        if (response.Headers != null)
        {
            foreach (var header in response.Headers)
            {
                headers.Append($"{header.Key}: {header.Value}\r\n");
            }
        }

        headers.Append("\r\n");

        byte[] headerBytes = Encoding.ASCII.GetBytes(headers.ToString());
        await rawStream.WriteAsync(headerBytes, 0, headerBytes.Length);

        if (response.Body.CanSeek)
        {
            response.Body.Position = 0;
        }

        byte[] buffer = new byte[BufferSize];
        int bytesRead;
        while ((bytesRead = await response.Body.ReadAsync(buffer, 0, buffer.Length)) > 0)
        {
            await rawStream.WriteAsync(buffer, 0, bytesRead);
        }

        await rawStream.FlushAsync();
    }
}