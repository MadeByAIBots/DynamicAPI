#!/bin/bash

source config_utils.sh

port=$(get_config_value 'Port')

pid=$(lsof -t -i:$port)

if [ -z "$pid" ]
then
  echo "No process running on port $port"
else
  echo "Killing process on port $port with PID: $pid"
  kill -9 $pid
fi

# TODO: Remove if not needed. Below was the old way of stopping it, but relies on the pid file having been successfully created

# The path to the PID file
#PID_FILE="run.pid"

# If the PID file exists, read the PID
#if [ -f "$PID_FILE" ]; then
#  PID=$(cat "$PID_FILE")

  # If a process with the PID is running, kill it
#  if ps -p "$PID" > /dev/null; then
#    kill "$PID"
#  fi

  # Delete the PID file
#  rm "$PID_FILE"
#fi