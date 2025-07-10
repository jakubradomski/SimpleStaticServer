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
Edit `config.json` to configure the server:

| Key           | Description                    | Default      |
| ------------- | ------------------------------ | ------------ |
| `RootPath`    | Directory to serve files from  | `wwwroot`    |
| `DefaultFile` | File served at root (`/`) path | `index.html` |
| `Port`        | Port to host the server        | `8080`       |


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

#### Using the server
- Browse to: `http://localhost:8080`
- Example URLs:
    - `/index.html` - Serves html file
    - `/docs/` - Lists a folder
    - `/images/logo.png` - Serves a image