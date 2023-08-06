#!/bin/bash

dir=$PWD

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

bash generate-auth-token.sh

# Run the project
dotnet bin/DynamicApiServer.dll --urls $URL:$PORT

echo "The application is running... listening on $URL:$PORT"