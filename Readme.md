## How to debug locally

1. Start postgresql docker container 
    - install docker: https://docs.docker.com/desktop/install/windows-install/
    - run the following command:
    ```bash
        docker run --name ilmhub-postgres -e POSTGRES_PASSWORD=example -e POSTGRES_USER=root -d -p 5432:5432 postgres
    ```

2. Set the environment variables in the local.settings.json file.
    - Create a new `local.settings.json` file in the Tally.Hooks folder.
    - Copy the following into the file:
    ```json
        {
            "IsEncrypted": false,
            "Host": {
                "CORS": "http://localhost:5113"
            },
            "Values": {
                "AzureWebJobsStorage": "UseDevelopmentStorage=true",
                "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
                "ConnectionStrings__FunctionsConnection": "Host=localhost;Port=5432;Username=root;Password=example;Database=functions;",
                "MigrateDatabase": "true", 
                "Bot__Token": "<your token here>"
            }
        }
        ```
3. Run the function app.
    - install vscode/cursor extension: https://marketplace.visualstudio.com/items?itemName=ms-azuretools.vscode-azurefunctions
    - install function core tools: 
    ```bash
        winget install Microsoft.Azure.FunctionsCoreTools
    ```
    - in the Tally.Hooks folder
    ```bash
        func start 
    ```
