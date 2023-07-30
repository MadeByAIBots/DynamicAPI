# Variables
HOST="localhost"
RELEASE_ZIP="release.zip"
TARGET_DIR="~/DynamicAPI"
#!/bin/bash



# Check SSH connection
echo "Checking SSH connection to $HOST..."
ssh -q $HOST exit
if [ $? -ne 0 ]; then
    echo "Error: Unable to establish SSH connection to $HOST"
    exit 1
fi

echo "Starting the deployment process..."

echo "Creating the release..."
# Create Release
./create-release.sh

echo "Release created. Preparing the target directory..."
# Prepare Target Directory and Transfer Release
ssh -T $HOST << EOF
  if [ -e "$TARGET_DIR" ] && [ ! -d "$TARGET_DIR" ]; then
    echo "Removing existing file at $TARGET_DIR..."
    rm -f $TARGET_DIR
    echo "Creating directory $TARGET_DIR..."
    mkdir -p $TARGET_DIR
  elif [ ! -d "$TARGET_DIR" ]; then
    echo "Creating directory $TARGET_DIR..."
    mkdir -p $TARGET_DIR
  fi
EOF

echo "Target directory prepared. Transferring the release..."
rsync -avz $RELEASE_ZIP $HOST:$TARGET_DIR

echo "Unzipping the release..."
ssh $HOST "cd $TARGET_DIR && unzip -qo $RELEASE_ZIP"


echo "Installing the release..."
# Install Release
ssh $HOST "cd $TARGET_DIR && bash install.sh"

echo "Release installed. Deployment completed."
