using System.Net;
using System.Net.Sockets;
using System.Text;

namespace HTTPServer;

class WebServer
{
    int _port = 8080;
    IPAddress serverAddress = IPAddress.Any;
    TcpListener _listener;
    bool isRunning;
    IRequestHandler _handler;

    public WebServer(IRequestHandler Handler, int port = 8080)
    {
        _port = port;
        _handler = Handler;
        _listener = new TcpListener(serverAddress, port);
    }

    public void Start()
    {
        isRunning = true;
        _listener.Start();
        Console.WriteLine($"Server started listening on: {_port}");

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
                Console.WriteLine("Empty request recieved");
                return;
            }

            string requestText = Encoding.UTF8.GetString(buffer, 0, bytesRead);

            var request = HttpRequestParser.Parse(requestText);
            Console.WriteLine($"{request.Method} {request.Path}");

            var response = _handler.Handle(request);
            await HttpResponseBuilder.SendResponse(stream, response);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] {ex.Message}");
        }
        finally
        {
            client.Close();
        }
    }
}
