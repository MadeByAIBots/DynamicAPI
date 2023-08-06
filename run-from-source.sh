#!/bin/bash

dir=$PWD

source config_utils.sh

URL=$(get_config_value 'Url')
PORT=$(get_config_value 'Port')

# Kill any dotnet process
bash stop.sh

# Build the project
dotnet build

# Run the project
dotnet run --project DynamicApiServer --urls $URL:$PORT > run.log 2>&1 &

echo "The application is running... listening on $URL:$PORT"