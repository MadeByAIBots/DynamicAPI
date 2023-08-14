#!/bin/bash

# Check if the URL is provided
if [ -z "$1" ]; then
  echo "Usage: $0 <new_external_url>"
  exit 1
fi

# Call the set-config-value.sh script to update the ExternalUrl value
bash set-config-value.sh ExternalUrl $1