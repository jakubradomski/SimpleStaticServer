namespace HTTPServer;

internal interface IRequestHandler
{
    HttpResponse Handle(HttpRequest request);
}