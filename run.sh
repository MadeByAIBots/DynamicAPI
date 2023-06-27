#!/bin/bash

URL=$(jq -r '.Url' config.json)
PORT=$(jq -r '.Port' config.json)

# Kill any dotnet process
pkill dotnet

# Build the project
(cd DynamicApiServer && dotnet build)

# Run the project
(dotnet run --project DynamicApiServer/DynamicApiServer.csproj --urls $URL:$PORT > run.log 2>&1 &)

echo "The application is listening on $URL:$PORT"