#!/bin/bash

SERVICE_NAME="dynamicapi"

echo "Restarting $SERVICE_NAME.service..."
sudo systemctl restart $SERVICE_NAME

# Verification step
echo "Verifying the status of $SERVICE_NAME.service..."
systemctl status $SERVICE_NAME.service

# Check the exit status of the systemctl command
if [ $? -eq 0 ]; then
    echo "$SERVICE_NAME.service is active and running."
else
    echo "Error: $SERVICE_NAME.service is not active. Please check the service status for details."
fi