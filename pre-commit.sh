#!/bin/bash
bash build.sh
if [ $? -ne 0 ]; then 
  echo "Build failed, aborting commit"
  exit 1
fi