#!/bin/bash

# Define the base directory
BASE_DIR="~/workspace/DynamicAPI"

# Define the script to compile
SCRIPT="$BASE_DIR/config/endpoints/endpoint-list/ListEndpointsScript.csx"

# Extract references from config.json
REFERENCES=$(jq -r '.CSharpScript.References[]' $BASE_DIR/config.json)

# Create a string with all references
REFS=""
for ref in $REFERENCES
do
  # Check if the reference contains a wildcard
  if [[ "$ref" == *"*"* ]]; then
    # Resolve the wildcard and add each file as a separate reference
    for file in $ref
do
      REFS="$REFS -r $file"
done
  else
    # If there's no wildcard, just add the reference as is
    REFS="$REFS -r $ref"
  fi
done

# Print the references
echo "References:"
echo "$REFERENCES"

# Try to compile the script with references
dotnet script "$SCRIPT" $REFS

# Check if the compilation was successful
if [ $? -eq 0 ]
then
  echo "Compilation of $SCRIPT was successful."
else
  echo "Compilation of $SCRIPT failed with error: $?"
fi