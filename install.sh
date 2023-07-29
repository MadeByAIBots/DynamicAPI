#!/bin/bash
# Check if the script is running from the workspace project folder
if [ -d "publish" ]; then
  # If the publish directory exists, hand off execution to the install.sh script inside it
  ./publish/install.sh
  # Exit the current script
  exit 0
fi

# If the publish directory does not exist, the script will continue with its normal installation process


SERVICE_NAME="my-dotnet-service"
SCRIPT_DIR="$(cd "$(dirname \"${BASH_SOURCE[0]}\")" && pwd)"
APP_PATH="\$SCRIPT_DIR/bin/DynamicApiServer.dll"
WORKING_DIRECTORY="\$SCRIPT_DIR"
GROUP_NAME="$(id -gn)"

cat << EOF | sudo tee /etc/systemd/system/$SERVICE_NAME.service
[Unit]
Description=My .NET Application
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

# Start the service immediately
sudo systemctl start $SERVICE_NAME
