#!/bin/bash

# Check if the port number is provided
if [ -z "$1" ]; then
  echo "Usage: $0 <new_port>"
  exit 1
fi

# Call the set-config-value.sh script to update the Port value
bash set-config-value.sh Port $1