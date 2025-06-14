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
            Task.Run(() => HandleClient(client));
        }
    }

    void HandleClient(TcpClient client)
    {
        using var stream = client.GetStream();
        using var reader = new StreamReader(stream, Encoding.UTF8);

        var buffer = new byte[4096];
        int bytesRead = stream.Read(buffer, 0, buffer.Length);
        string requestText = Encoding.UTF8.GetString(buffer, 0, bytesRead);

        var request = HttpRequestParser.Parse(requestText);
        System.Console.WriteLine($"{request.Method} {request.Path}");
        var response = _handler.Handle(request);

        HttpResponseBuilder.SendResponse(stream, response);
    }
}
