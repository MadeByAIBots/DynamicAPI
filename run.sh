#!/bin/bash

# Call the stop script to stop the previous version of the application
bash stop.sh

# Print a message before starting the application
echo "Starting the application..."

# Start the application and store its PID in the PID file
nohup dotnet run --project HelloWorldAPIProject/HelloWorldAPIProject.csproj > run.log 2>&1 &
echo $! > "run.pid"

# Print the PID of the application
echo "The application has been started. PID: $!"

# Wait a few seconds for the application to start and write to the log file
sleep 2

# Print the last few lines of the log file
echo "Latest log messages:"
tail run.log