#!/bin/bash

tokenFileName="auth-token.security"

if [ ! -f $tokenFileName ]; then
  echo "Generating auth token..."
  openssl rand -base64 32 > auth-token.security
else
  echo "Auth token already exists. Skipping."
fi
