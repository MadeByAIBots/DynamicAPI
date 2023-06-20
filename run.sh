#!/bin/bash

URL=$(jq -r '.Url' config.json)
PORT=$(jq -r '.Port' config.json)

# Kill any dotnet process
pkill dotnet

# Build the project
(cd HelloWorldAPIProject && dotnet build)

# Run the project
(dotnet run --project HelloWorldAPIProject/HelloWorldAPIProject.csproj --urls $URL:$PORT > run.log 2>&1 &)

echo "The application is listening on $URL:$PORT"