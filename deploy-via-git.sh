#!/bin/bash

# Run the tests
bash test.sh

sourcePath=$PWD
destinationPath="../../deploy/live/DynamicAPI"

# Check if the tests passed
if [ $? -eq 0 ]
then
  echo "Tests passed. Proceeding with deployment."

  bash stop.sh

  # Check if the deployment directory exists
  if [ ! -d "$destinationPath/.git" ]
  then
    # Clone the repository
    git clone $sourcePath $destinationPath
  fi

  # Change to the deployment directory
  cd $destinationPath

  bash stop.sh

  # Pull the latest changes
  git pull || echo "Pull failed"

  # Run the application
  bash run_and_test.sh
else
  echo "Tests failed. Aborting deployment."
fi
