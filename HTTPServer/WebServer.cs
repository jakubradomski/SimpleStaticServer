using System.Net;
using System.Net.Sockets;
using System.Text;
using Microsoft.Extensions.Logging;

namespace HTTPServer;

class WebServer
{
    int _port = 8080;
    IPAddress serverAddress = IPAddress.Any;
    TcpListener _listener;
    ILogger<WebServer> _logger;
    bool isRunning;
    IRequestHandler _handler;

    public WebServer(IRequestHandler handler, ILogger<WebServer> logger, int port = 8080)
    {
        _port = port;
        _handler = handler;
        _logger = logger;
        _listener = new TcpListener(serverAddress, port);
    }

    public void Start()
    {
        isRunning = true;
        _listener.Start();
        _logger.LogInformation($"Server started listening on: {_port}");

        while (isRunning)
        {
            var client = _listener.AcceptTcpClient();
            _ = HandleClientAsync(client);
        }
    }

    private async Task HandleClientAsync(TcpClient client)
    {
        try
        {
            using var stream = client.GetStream();
            using var reader = new StreamReader(stream, Encoding.UTF8);

            var buffer = new byte[4096];
            int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);

            if (bytesRead == 0)
            {
                _logger.LogWarning("Empty request recieved");
                return;
            }

            string requestText = Encoding.UTF8.GetString(buffer, 0, bytesRead);

            var request = HttpRequestParser.Parse(requestText);
            _logger.LogInformation($"{request.Method} {request.Path}");

            var response = _handler.Handle(request);
            await HttpResponseBuilder.SendResponse(stream, response);
        }
        catch (Exception ex)
        {
            _logger.LogError($"[ERROR] {ex.Message}");
        }
        finally
        {
            client.Close();
        }
    }
}
