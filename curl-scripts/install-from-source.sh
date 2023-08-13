#!/bin/bash
set -e # Exit on any error

# Define default installation directory in the home directory
DEFAULT_INSTALL_DIR="~/DynamicAPI-new"

# Define installation directory, allowing override by environment variable
INSTALL_DIR=${INSTALL_DIR:-$DEFAULT_INSTALL_DIR}

# Define base URL for the GitHub repository
BASE_URL="https://github.com/MadeByAIBots/DynamicAPI.git"

echo "Installation directory: $INSTALL_DIR"
# Check if Git is installed and install it if necessary
if ! command -v git &> /dev/null; then
  echo "Git not found. Installing Git..."
  if [ "$(id -u)" -eq 0 ]; then
    # Running as root, so no need for sudo
    apt-get update
    apt-get install -y git
  else
    # Not running as root, so use sudo if available
    if command -v sudo &> /dev/null; then
      sudo apt-get update
      sudo apt-get install -y git
    else
      echo "Error: sudo not found and not running as root. Cannot install Git."
      exit 1
    fi
  fi
fi

# Expand the tilde to the actual home directory
INSTALL_DIR=$(eval echo $INSTALL_DIR)

# Check if the repository already exists
if [ -d "$INSTALL_DIR" ]; then
  echo "Repository already exists. Pulling latest changes..."
  cd "$INSTALL_DIR"
  git pull
else
  echo "Cloning repository..."
  # Construct the full URL based on the presence of the GitHub token
  FULL_URL=$BASE_URL
  if [ -n "$GITHUB_TOKEN" ]; then
    FULL_URL="https://$GITHUB_TOKEN@${BASE_URL#https://}"
  fi
  git clone $FULL_URL "$INSTALL_DIR"
  cd "$INSTALL_DIR"
fi

# Run the preparation, build, publish, and install scripts
echo "Running prepare.sh..."
bash prepare.sh
echo "Running build.sh..."
bash build.sh
echo "Running publish.sh..."
bash publish.sh
echo "Running install.sh..."
bash install.sh
echo "Installation complete!"

