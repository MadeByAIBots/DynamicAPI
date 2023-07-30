#!/bin/bash

# Exit script on first error
set -e

# Build endpoints
bash build-endpoints.sh

# Build solution
dotnet build