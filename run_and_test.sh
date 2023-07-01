#!/bin/bash

# Run the application
./run.sh

# Wait for the application to start
sleep 4

authToken=$(cat auth-token.security)

# Test the endpoint
#curl -X POST http://localhost:5054/bash-hello-world

# Test the endpoint
#curl -X GET http://localhost:5054/bash-hello-target?target=universe


PORT=$(jq -r '.Port' config.json)

echo ""
echo "Testing the file-read endpoint"
echo ""

curl -X GET http://localhost:$PORT/file-read \
-H "Authorization: Bearer $authToken" \
-H 'Content-Type: application/json' \
-d '{
  "workingDirectory": "/root/workspace/DynamicAPI",
  "filePath": "build.sh"
}'

echo ""
echo "Testing the file-read-lines endpoint"
echo ""

curl -X GET http://localhost:$PORT/file-read-lines \
-H "Authorization: Bearer $authToken" \
-H 'Content-Type: application/json' \
-d '{
  "workingDirectory": "/root/workspace/DynamicAPI",
  "filePath": "build.sh"
}'
