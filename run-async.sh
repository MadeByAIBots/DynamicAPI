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

nohup bash run.sh > run.log &

echo "The application is running... listening on $URL:$PORT"