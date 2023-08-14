# Quick Install from Source

## Introduction

This document provides instructions for a quick installation of the ChatGPT plugin from source using Curl. This method has been tested on Ubuntu but should work on any Debian system. The installation will clone the source to the `~/DynamicAPI` directory, and the binaries and config files will be located in `~/DynamicAPI/publish`. The plugin will run as the user that installs it, not as a system process.

## Prerequisites

### Curl
Curl is required to download and run the installation script. You can check if Curl is already installed by running:

```bash
curl --version
```

If Curl is not installed, you can install it on Debian-based systems with:

```bash
sudo apt-get install curl
```

## Step-by-step Guide

### 1. Install Curl (if not already installed)

If you don't have Curl installed, run the following command:

```bash
sudo apt-get install curl
```

### 2. Download and Run the Script

Use the following command to download and execute the installation script:

```bash
curl -sSL https://raw.githubusercontent.com/MadeByAIBots/DynamicAPI/main/curl-scripts/install-from-source.sh | bash
```

This command will download the script from the GitHub repository, clone the source to the `~/DynamicAPI` directory, and set up the ChatGPT plugin in the `~/DynamicAPI/publish` directory.

### 3. Verify the Installation

You can verify the installation by checking the `~/DynamicAPI/publish` directory and ensuring that the necessary files are present. Additionally, you can verify the status of the `dynamicapi` service using:

```bash
systemctl status dynamicapi
```

Or view the service logs with:

```bash
journalctl -u dynamicapi
```

### 4. Proceed to API Configuration

After successfully installing the plugin, you'll need to configure the API. Please refer to the [Configure API](../configure_api.md) document for detailed instructions.

## Troubleshooting

If the installation does not work, please post the installation log and the service logs as an issue on the [GitHub repository](https://github.com/MadeByAIBots/DynamicAPI/issues). You can also refer to the [Troubleshooting](../../troubleshooting.md) document for common solutions.

## Conclusion

You have successfully installed the ChatGPT plugin from source. The source has been cloned to `~/DynamicAPI`, and the binaries and config files are located in `~/DynamicAPI/publish`. The plugin will run under the user context that performed the installation. Please proceed to [Configure API](../configure_api.md) for the next steps in the setup process.
