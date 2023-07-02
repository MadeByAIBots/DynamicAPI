#!/bin/bash

# Exit the script as soon as a command fails
set -e

# Run the build script
bash build.sh

bash validate.sh 

# Generate the OpenAPI specification
bash GenerateOpenAPISpec.sh

# Stage the OpenAPI specification for commit
git add DynamicApiServer/wwwroot/openapi.yaml

# Run the test script
bash test.sh