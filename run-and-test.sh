#!/bin/bash

# Run the application
bash run-async.sh

# Wait for the application to start
sleep 4

# Test the endpoint
#curl -X POST http://localhost:5054/bash-hello-world

# Test the endpoint
#curl -X GET http://localhost:5054/bash-hello-target?target=universe

bash test-running.sh
