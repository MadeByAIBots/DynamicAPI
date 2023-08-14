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
Enter the external URL that you configured earlier (e.g., `YOUR_EXTERNAL_URL`), then select "Find manifest file."

### 8. Enter Auth Token
The user will be prompted to enter the authentication token. Copy and paste it in.

### 9. Receive Verification Token
The user will receive a verification token from the ChatGPT page.

### 10. Set Verification Token
Execute the following command:

```bash
bash set-openai-verification-token.sh (token)
```

### 11. Restart the Server
Execute the following command:

```bash
bash restart.sh
```

### 12. Verify on ChatGPT
Return to ChatGPT and click the verify button.

### 13. Enable the Plugin
Click the checkbox to enable the plugin.

### 14. Run an Example Command
Send the following prompt to ChatGPT:

```plaintext
Please test the DynamicAPI by running an echo hello bash command
```

## Notes
- The local server setup and external URL configuration are covered in separate documents and must be completed before following this procedure.
- The authentication token can be retrieved by simply opening the file or using standard commands like `cat`.