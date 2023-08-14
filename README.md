# DynamicAPI

DynamicAPI is a modular API written in C# with .NET 7, which can be used as a ChatGPT plugin.

The DynamicAPI allows endpoints to be added as modules without having to restart or rebuild the app.

## Features

- **Modular Design**: Add or update endpoints without restarting the application.
- **Multiple Executor Types**: Supports the following:
  - Bash
  - C# script
  - C#
  - Python (coming soon)
- **Built-in Endpoints**: Comes with a variety of essential endpoints (see below).
- **ChatGPT Plugin**: Can be used as a standalone Dynamic API or as a ChatGPT plugin.

## Quick Start

For a detailed setup guide, please refer to the [Setup README](./docs/setup/README.md).

## Built-in Endpoints

DynamicAPI comes with a variety of built-in endpoints that provide essential functionalities out of the box. These endpoints cover a wide range of operations, from file and directory management to code execution and experimental AI interactions. Below is a selection of some of the most useful built-in endpoints:

### File Operations
- File Read/Write
- File Search (Names & Content)
- Line Operations (Insert, Replace, Delete, etc.)

### Directory Operations
- Directory List/Create/Search
- Directory Copy/Move

### Code/Command Execution
- Run Bash Command
- Run Python Snippet

### AI Interaction (Experimental)
- AI Bot Message
- AI Bot Chat
- AI Bot Send Files and Ask Questions

## Support and Community

Have questions, ideas, or need help with DynamicAPI? Feel free to open an issue or contribute to the project. We welcome feedback and contributions from the community!

Join the new discord server here:

https://discord.gg/ECG3YTMpPK

## License

This project is licensed under the GNU General Public License (GPL).
