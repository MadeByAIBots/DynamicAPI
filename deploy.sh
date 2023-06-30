#!/bin/bash

# Run the tests
bash test.sh

# Check if the tests passed
if [ $? -eq 0 ]
then
  echo "Tests passed. Proceeding with deployment."

  bash stop.sh

  # Check if the deployment directory exists
  if [ ! -d "/root/deploy/live/DynamicAPI/.git" ]
  then
    # Clone the repository
    git clone /root/workspace/DynamicAPI /root/deploy/live/DynamicAPI
  fi

  # Change to the deployment directory
  cd /root/deploy/live/DynamicAPI

  # Pull the latest changes
  git pull || echo "Pull failed"

  # Run the application
  bash run.sh
else
  echo "Tests failed. Aborting deployment."
fi