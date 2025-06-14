namespace HTTPServer;

interface IRequestHandler
{
    HttpResponse Handle(HttpRequest request);
}