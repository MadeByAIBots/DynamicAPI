#!/bin/bash

TOKEN=$(cat auth-token.security)
curl -X POST -H "Authorization: Bearer $TOKEN" -H "Content-Type: application/json" -d '{"workingDirectory": "/root/workspace/DynamicAPI/", "subDirectory": "endpoints/ai-bot-send-file-paths", "message": "Please tell me how many files are in this list.", "recursive": "true"}' http://localhost:5054/ai-bot-send-file-paths