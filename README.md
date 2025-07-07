# Simple HTTP Server


#### Functions
- Serves static files from local directory over HTTP
- Shows auto-generated HTML directory listing
- Detects and sets correct MIME types
- Uses config file (`config.json`) for easy configuration


#### How to run
``` bash
dotnet run
```

#### Example `config.json`
``` json
{
    "Settings" : {
        "RootPath" : "/files",
        "DefaultFile" : "index.html",
        "Port" : 8080,
        "MinThreads" : 10,
        "MaxThreads" : 100
    }
}
```