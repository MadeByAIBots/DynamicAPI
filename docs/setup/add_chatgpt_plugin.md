# Installing DynamicAPI as ChatGPT Plugin

## Pre-requisites
- **Local Server Setup**: Ensure that the local server is set up as per the instructions in a separate document.
- **External URL Configuration**: Use the specific URL that was configured as the external URL in a previous document. This URL should include the port number if applicable.

## Procedure

### 1. Retrieve Auth Token
Open the `auth-token.security` file in a text editor or use the `cat` command to retrieve the authentication token.

### 2. Verify Server Accessibility
Ensure the server is accessible to the web by opening the following URL in a web browser: `YOUR_EXTERNAL_URL/openapi.yaml`, where `YOUR_EXTERNAL_URL` is the external URL configured previously.

### 3. Navigate to ChatGPT
Go to [ChatGPT](https://chat.openai.com).

### 4. Select "Plugins"
In the Model drop-down, select "Plugins."

### 5. Access Plugin Store
Click on "Plugin store."

### 6. Develop Your Own Plugin
Select "Develop your own plugin."

### 7. Enter External URL
Enter the external URL that you configured earlier, then select "Find manifest file."

### 8. Enter Service Access Token
Get your auth token:
```
cat auth-token.security
```
Copy and paste it into the field on ChatGPT then click "Next".

### 9. Receive Verification Token
You will be shown a verification token in a snippet of JSON like this:
```
{"openai":"abcdef123456"}
```
Copy only the value from this json like this:
```
abcdef123456
```
Then use it in the next step.

### 10. Set Verification Token
Execute the following command:

```bash
bash set-openai-verification-token.sh <verification_token>
```
For example:
```bash
bash set-openai-verification-token.sh abcdef123456
```

### 11. Restart the Server
Execute the following command:

```bash
bash restart.sh
```

### 12. Verify on ChatGPT
Return to ChatGPT and click the "Verify tokens" button.

Note: If this fails, make sure you restarted the server using the restart script above.
If it still fails, post a GitHub issue for assistance.

### 13. Finish Install on ChatGPT
1. Click "Install for me" button
   
2. Click "Continue"

3. Click "Install plugin"

### 14. Enable the Plugin
The new plugin should be enabled and the checkbox should be ticked. If it's not ticked, then tick it to enable it.

### 15. Run an Example Command
Send the following prompt to ChatGPT to test that it's working:

```
Please test the DynamicAPI by running an echo hello bash command
```
Click on the "DynamicAPI" icon in the chat to expand it, and see the request and the response.
In the request you should see this:
```
{
  "command": "echo hello",
  "workingDirectory": "."
}
```
In the response you should see this (or similar):
```
hello
```

If you didn't get the expected response, please log a GitHub issue to get assistance.

### Celebrate
If you got the desired response... celebrate! You can now get ChatGPT to interact with your server, run commands, and read/write/edit files.

## Notes
- The local server setup and external URL configuration are covered in separate documents and must be completed before following this procedure.
- The authentication token can be retrieved by simply opening the file or using standard commands like `cat`.
