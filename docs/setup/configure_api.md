# API Configuration

## Introduction

This document provides instructions for configuring the DynamicAPI for the ChatGPT plugin. You can configure the API using the provided set scripts or manually edit the `config.override.json` file. The recommended method is to use the set scripts.

## Script Locations

The location of the scripts depends on the installation method:

- **Installed from Source:** Scripts are located in the `~/DynamicAPI/publish` directory.
- **Installed from Release:** Scripts are located in the `~/DynamicAPI` directory.

Navigate to the appropriate directory based on your installation method before running the scripts.

## Configuration Options

### 1. Set Port (Default: 8888)

You can change the port on which the API runs using the following command:

```bash
bash set-port.sh <port_number>
```

Replace `<port_number>` with the desired port number.

### 2. Set External URL

If you have configured a Cloudflare tunnel or other tunnel, you may need to set the external URL using:

```bash
bash set-external-url.sh <external_url>
```

Replace `<external_url>` with the full external URL.

### 3. Set Name Postfix (Optional)

If you have multiple installations, you can set a different name postfix for each one to identify the installation:

```bash
bash set-name-postfix.sh <name_postfix>
```

Replace `<name_postfix>` with the desired name postfix.

### 4. Set Configuration Value

You can set any specific configuration value using the following command:

```bash
bash set-config-value.sh <key> <value>
```

Replace `<key>` with the configuration key and `<value>` with the desired value.

## Manual Configuration

You can manually edit the `config.override.json` file to change the configurations. Do not edit the `config.json` file, as it will be overwritten on update. If the `config.override.json` file does not exist, you can create it or copy the existing `config.json` to that file name.

## Restart the Service

After making changes to the configuration, you must restart the service for the changes to take effect:

```bash
bash restart.sh
```

## Conclusion

You have successfully configured the DynamicAPI for the ChatGPT plugin. You can use the provided set scripts to easily change common configurations or manually edit the `config.override.json` file for more advanced configurations. Remember to restart the service after making changes.
