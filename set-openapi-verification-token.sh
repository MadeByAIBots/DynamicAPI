#!/bin/bash

# Check if the token is provided
if [ -z "$1" ]; then
  echo "Usage: $0 <new_verification_token>"
  exit 1
fi

# Call the set-config-value.sh script to update the OpenAIVerificationToken value
bash set-config-value.sh OpenAIVerificationToken $1