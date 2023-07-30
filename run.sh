#!/bin/bash

# Check if config.override.json exists
if [ -f "config.override.json" ]; then
    CONFIG_FILE="config.override.json"
else
    CONFIG_FILE="config.json"
fi

URL=$(jq -r '.Url' $CONFIG_FILE)
PORT=$(jq -r '.Port' $CONFIG_FILE)

# Kill any dotnet process
bash stop.sh

# Build the project
(cd DynamicApiServer && dotnet build)

# Run the project
(dotnet run --project DynamicApiServer/DynamicApiServer.csproj --urls $URL:$PORT > run.log 2>&1 &)

echo "The application is running... listening on $URL:$PORT"