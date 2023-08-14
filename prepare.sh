#!/bin/bash

# Install .NET 7
#wget https://dot.net/v1/dotnet-install.sh
#chmod +x dotnet-install.sh
#./dotnet-install.sh -c 7.0 --install-dir /usr/share/dotnet

sudo apt-get update
sudo apt-get install -y jq dotnet-sdk-7.0 python3 python3-pip python-is-python3
