using System.Net.Sockets;
using System.Text;

namespace HTTPServer;

static class HttpResponseBuilder
{
    private const int BufferSize = 64 * 1024;

    public static void SendResponse(NetworkStream rawStream, HttpResponse response)
    {
        var headers = new StringBuilder();
        headers.AppendLine($"HTTP/1.1 {response.StatusCode}");
        headers.AppendLine($"Content-Type: {response.ContentType}");
        headers.AppendLine($"Content-Length: {response.ContentLength}");
        headers.AppendLine($"Connection: close");

        if (response.Headers != null)
        {
            foreach (var header in response.Headers)
            {
                headers.AppendLine($"{header.Key}: {header.Value}");
            }
        }

        headers.AppendLine();

        byte[] headerBytes = Encoding.ASCII.GetBytes(headers.ToString());
        rawStream.Write(headerBytes, 0, headerBytes.Length);

        byte[] buffer = new byte[BufferSize];
        int bytesRead;
        while ((bytesRead = response.Body.Read(buffer, 0, buffer.Length)) > 0)
        {
            rawStream.Write(buffer, 0, bytesRead);
        }

        rawStream.Flush();
    }
}