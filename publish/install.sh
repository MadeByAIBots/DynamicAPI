#!/bin/bash
# Check if the script is running from the workspace project folder
if [ -d "publish" ]; then
  # If the publish directory exists, hand off execution to the install.sh script inside it
  cp install.sh publish/install.sh
  cd publish
  bash install.sh
  # Exit the current script
  exit 0
fi

# If the publish directory does not exist, the script will continue with its normal installation process

echo "Installing DynamicAPI..."

bash generate-auth-token.sh

SERVICE_NAME="dynamicapi"
SCRIPT_DIR="$(cd "$(dirname \"${BASH_SOURCE[0]}\")" && pwd)"
APP_PATH="$SCRIPT_DIR/DynamicApiServer.dll"
WORKING_DIRECTORY="$SCRIPT_DIR"
GROUP_NAME="$(id -gn)"
SYSTEMD_FILE_PATH="/etc/systemd/system/$SERVICE_NAME.service"

echo "  Service name: $SERVICE_NAME"
echo "  Script dir: $SCRIPT_DIR"
echo "  Application path: $APP_PATH"
echo "  Working directory: $WORKING_DIRECTORY"
echo "  Group name: $GROUP_NAME"
echo "  Systemd File Path: $SYSTEMD_FILE_PATH"

cat << EOF | sudo tee $SYSTEMD_FILE_PATH
[Unit]
Description=DynamicAPI
After=network.target

[Service]
ExecStart=/usr/bin/dotnet $APP_PATH
WorkingDirectory=$WORKING_DIRECTORY
User=$USER_NAME
Group=$GROUP_NAME
Restart=always
RestartSec=10
SyslogIdentifier=dotnet-example
Environment=ASPNETCORE_ENVIRONMENT=Production 

[Install]
WantedBy=multi-user.target
EOF

# Reload the systemd daemon to recognize the new service
sudo systemctl daemon-reload

# Enable the service to start on boot
sudo systemctl enable $SERVICE_NAME

sudo systemctl restart $SERVICE_NAME

# Start the service immediately
sudo systemctl start $SERVICE_NAME

# Verification step
echo "Verifying the status of $SERVICE_NAME.service..."
systemctl status $SERVICE_NAME.service

# Check the exit status of the systemctl command
if [ $? -eq 0 ]; then
    echo "$SERVICE_NAME.service is active and running."
else
    echo "Error: $SERVICE_NAME.service is not active. Please check the service status for details."
fi
