# Simple C# Static HTTP Server

Lightweight and configurable static HTTP server written in c#.

#### Features
- Serves static files (html, css, js, images, etc)
- Shows auto-generated HTML directory listing
- Mime type support
- Handling of invalid paths
- Uses config file (`config.json`) for easy configuration
- Clean and Modular design
- Logging to console


#### Prerequisites
.NET 9 SDK 

#### Build and run
``` bash
dotnet build
dotnet run
```

#### Configuration
Server can be configured using either `config.json` file or command line arguments

#### `config.json`

| Key           | Description                           | Default      |
| ------------- | ------------------------------------- | ------------ |
| `RootPath`    | Directory to serve files from         | `wwwroot`    |
| `DefaultFile` | File served when no path is specified | `index.html` |
| `Port`        | Port to host the server               | `8080`       |


#### Example `config.json`
``` json
{
    "Settings" : {
        "RootPath" : "/wwwroot",
        "DefaultFile" : "index.html",
        "Port" : 8080
    }
}
```

#### Command-Line Arguments
| Flag       | Description   |
| ---------- | ------------- |
| `-p`       | `Port`        |
| `-f`       | `DefaultFile` |
| `-r`       | `RootPath `   |

for example:
`dotnet run -- -p 8080 -f index.html -r /wwwroot`

#### Using the server
- Browse to: `http://localhost:8080`
- Example URLs:
    - `/index.html` - Serves html file
    - `/docs/` - Lists a folder
    - `/images/logo.png` - Serves a image