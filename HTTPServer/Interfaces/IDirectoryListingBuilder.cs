namespace HTTPServer;

public interface IDirectoryListingBuilder
{
    string BuildHtmlListing(string path, string requestPath,IFileProvider fileProvider);
}