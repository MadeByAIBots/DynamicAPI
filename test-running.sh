echo "Testing the running DynamicAPI instance..."

# Check if config.override.json exists
if [ -f "config.override.json" ]; then
    CONFIG_FILE="config.override.json"
else
    CONFIG_FILE="config.json"
fi

echo "  Config file: $CONFIG_FILE"

URL=$(jq -r '.Url' $CONFIG_FILE)
PORT=$(jq -r '.Port' $CONFIG_FILE)

authToken=$(cat auth-token.security)

echo ""
echo "Testing the file-read endpoint"
echo "  URL: http://localhost:$PORT"
echo ""

curl -X GET http://localhost:$PORT/file-read \
-H "Authorization: Bearer $authToken" \
-H 'Content-Type: application/json' \
-d "{
  \"workingDirectory\": \"$PWD\",
  \"filePath\": \"example-file.txt\"
}"

echo ""
echo "Testing the file-read-lines endpoint"
echo ""

curl -X GET http://localhost:$PORT/file-read-lines \
-H "Authorization: Bearer $authToken" \
-H 'Content-Type: application/json' \
-d "{
  \"workingDirectory\": \"$PWD\",
  \"filePath\": \"example-file.txt\"
}"

echo ""
echo "Testing the run-bash-command endpoint"
echo ""

curl -X GET http://localhost:$PORT/run-bash-command \
-H "Authorization: Bearer $authToken" \
-H 'Content-Type: application/json' \
-d "{
  \"workingDirectory\": \"$PWD\",
  \"command\": \"echo hello\"
}"

echo ""
echo "Testing the directory-list endpoint"
echo ""

curl -X GET http://localhost:$PORT/directory-list \
-H "Authorization: Bearer $authToken" \
-H 'Content-Type: application/json' \
-d "{
  \"workingDirectory\": \"$PWD\",
  \"directoryPath\": \".\"
}"

echo ""
echo "Finished Test"
