#!/bin/bash

publishDir="publish/"

rm -r $publishDir

# Stop it first or publish command will fail as binaries are in use
bash stop.sh

dotnet publish -o $publishDir/bin

cp -r endpoints/ $publishDir/endpoints/

# Array of files and scripts to copy
files=(
  "install.sh"
  "config.json"
  "generate-auth-token.sh"
  "example-file.txt"
  "stop.sh"
  "run.sh"
  "run-async.sh"
  "run-and-test.sh"
  "test-running.sh"
  "config_utils.sh"
  "set-external-url.sh"
  "set-config-value.sh"
  "set-name-postfix.sh"
  "set-openai-verification-token.sh"
  "set-port.sh"
  "restart.sh"
)

# Loop to copy files and scripts
for file in "${files[@]}"; do
  cp $file $publishDir
done