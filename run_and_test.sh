#!/bin/bash

# Run the application
./run.sh

# Wait for the application to start
sleep 4

# Test the endpoint
curl -X POST http://localhost:5054/bash-hello-world