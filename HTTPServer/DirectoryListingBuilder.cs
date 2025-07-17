using System.Text;

namespace HTTPServer;

public class DirectoryListingBuilder : IDirectoryListingBuilder
{
    private IFileProvider _fileProvider;

    public DirectoryListingBuilder(IFileProvider fileProvider)
    {
        _fileProvider = fileProvider;
    }

    public string BuildHtmlListing(string path, string requestPath)
    {
        StringBuilder builder = new StringBuilder();
        builder.AppendLine("<!DOCTYPE html>");
        builder.AppendLine($"<html><head><meta charset='UTF-8'><title>Index of {path}</title></head>");
        builder.AppendLine("<body>");
        builder.AppendLine($"<h1>Index of {requestPath}</h1>");
        builder.AppendLine("<ul>");

        if (requestPath != "/")
        {
            string parentPath = GetParentPath(requestPath);
            builder.AppendLine($"<li><a href=\"{parentPath}\">../</a></li>");
        }

        foreach (var directory in _fileProvider.GetDirectoriesInDirectory(path))
        {
            string dirName = Path.GetFileName(directory);
            string href = CombineUrlPath(requestPath, dirName + "/");
            builder.AppendLine($"<li><a href=\"{href}\">{dirName}/</a></li>");
        }

        foreach (var file in _fileProvider.GetFilesInDirectory(path))
        {
            string filename = Path.GetFileName(file);
            string href = CombineUrlPath(requestPath, filename);
            builder.AppendLine($"<li><a href=\"{href}\">{filename}</a></li>");
        }

        builder.AppendLine("</ul>");
        builder.AppendLine("</body></html>");
        return builder.ToString();
    }

    private string GetParentPath(string path)
    {
        path = path.TrimEnd('/');
        int lastSlash = path.LastIndexOf('/');
        return lastSlash <= 0 ? "/" : path.Substring(0, lastSlash) + "/";
    }

    private string CombineUrlPath(string basePath, string name)
    {
        if (!basePath.EndsWith("/"))
            basePath += "/";

        string encodedName = Uri.EscapeDataString(name.TrimEnd('/'));

        if (encodedName.EndsWith("/"))
            encodedName += "/";

        return basePath + encodedName;
    }
}