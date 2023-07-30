#!/bin/bash

# Store the current directory
original_dir=$(pwd)

# Call the publish.sh script to build the project and publish the output
./publish.sh

rm release.zip

# Create the zip file with the contents of the publish directory at the root
cd publish
zip -r ../release.zip .

# Move back to the original directory
cd $original_dir

echo "Release created."
