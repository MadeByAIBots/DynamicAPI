#!/bin/bash

# Check if the name postfix is provided
if [ -z "$1" ]; then
  echo "Usage: $0 <new_name_postfix>"
  exit 1
fi

# Call the set-config-value.sh script to update the NamePostfix value
bash set-config-value.sh NamePostfix $1