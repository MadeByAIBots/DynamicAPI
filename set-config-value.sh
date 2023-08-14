#!/bin/bash

# Check if the key and value are provided
if [ -z "$1" ] || [ -z "$2" ]; then
  echo "Usage: $0 <key> <value>"
  exit 1
fi

# Relative path to the config.override.json file
CONFIG_FILE="./config.override.json"

# Check if the key starts with a dot
KEY="$1"
if [[ "$KEY" != .* ]]; then
  KEY=".$KEY"
fi

# Start message
echo "Updating '$1' in '$CONFIG_FILE'..."

# Check if the file exists, if not create it
if [ ! -f "$CONFIG_FILE" ]; then
  echo "Creating '$CONFIG_FILE'..."
  echo "{}" > $CONFIG_FILE
fi

# Read the current value for the specified key
CURRENT_VALUE=$(jq "$KEY" $CONFIG_FILE)

# Update the specified key with the new value
jq "$KEY = \"$2\"" $CONFIG_FILE > temp.json && mv temp.json $CONFIG_FILE

echo "'$1' changed from '$CURRENT_VALUE' to '$2'."
echo "Update successful."