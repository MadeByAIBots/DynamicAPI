#!/bin/bash

source config_utils.sh

URL=$(get_config_value 'Url')
PORT=$(get_config_value 'Port')

# Kill any dotnet process
bash stop.sh

bash generate-auth-token.sh

# Run the project
dotnet bin/DynamicApiServer.dll --urls "$URL:$PORT"

echo "The application is running... listening on $URL:$PORT"