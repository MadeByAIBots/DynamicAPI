#!/bin/bash

TOKEN=$(cat auth-token.security)
curl -X POST -H "Authorization: Bearer $TOKEN" -H "Content-Type: application/json" -d '{"message": "Hello, World!"}' http://localhost:5054/ai-bot-message
