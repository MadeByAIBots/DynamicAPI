#!/bin/bash

# The path to the PID file
PID_FILE="run.pid"

# If the PID file exists, read the PID
if [ -f "$PID_FILE" ]; then
  PID=$(cat "$PID_FILE")

  # If a process with the PID is running, kill it
  if ps -p "$PID" > /dev/null; then
    kill "$PID"
  fi

  # Delete the PID file
  rm "$PID_FILE"
fi