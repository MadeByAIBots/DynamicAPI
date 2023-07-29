#!/bin/bash

# Define the name of the zip file
zipfile="release.zip"

# Define the directories and files to include
include=(
  "publish/*"
)

# Create the zip file
zip -r $zipfile ${include[@]}