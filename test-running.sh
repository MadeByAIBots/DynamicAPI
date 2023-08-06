echo "Testing the running DynamicAPI instance..."

source config_utils.sh

URL=$(get_config_value 'Url')
PORT=$(get_config_value 'Port')

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
