#!/bin/bash

# Variables
DEFAULT_INSTALL_DIR="~/DynamicAPI-new"
INSTALL_DIR="${INSTALL_DIR:-$DEFAULT_INSTALL_DIR}"
GITHUB_USER="MadeByAIBots"
GITHUB_REPO="DynamicAPI"
GITHUB_URL="https://github.com/$GITHUB_USER/$GITHUB_REPO.git"
GITHUB_TOKEN="${GITHUB_TOKEN:-}"

# Clone or update repository
if [ -d "$INSTALL_DIR" ]; then
  git -C "$INSTALL_DIR" pull
else
  if [ -n "$GITHUB_TOKEN" ]; then
    git clone "$GITHUB_URL" "$INSTALL_DIR" --depth 1
  else
    git clone "https://$GITHUB_TOKEN@github.com/$GITHUB_USER/$GITHUB_REPO.git" "$INSTALL_DIR" --depth 1
  fi
fi

# Run installation scripts
cd "$INSTALL_DIR"
bash prepare.sh
bash build.sh
bash publish.sh
bash install.sh