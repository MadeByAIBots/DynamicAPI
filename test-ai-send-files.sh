#!/bin/bash

TOKEN=$(cat auth-token.security)
curl -X POST -H "Authorization: Bearer $TOKEN" -H "Content-Type: application/json" -d '{"workingDirectory": "/root/workspace/DynamicAPI", "files": "brokenCode.txt", "message": "Please fix the broken C# code in these files."}' http://localhost:5054/ai-bot-send-files
