# DynamicAPI

DynamicAPI is a highly flexible and adaptable API server that dynamically creates its endpoints based on configuration files. This allows for rapid development and deployment of various functionalities without modifying the core codebase.

## Concept

The core concept behind DynamicAPI is to define endpoints through configuration files and scripts. Each endpoint is defined in its own directory under `endpoints/`. The name of the directory corresponds to the endpoint's name, and the contents of the directory define the behavior of the endpoint.

## Usage

1. **Define Endpoints**: Create a directory under `endpoints/` for each endpoint you want to define. Include any necessary configuration files, scripts, or other assets in the directory.

2. **Run the Server**: Start the DynamicAPI server. It will automatically load the endpoints defined in the `endpoints/` directory.

3. **Interact with the API**: Send HTTP requests to the DynamicAPI server to interact with the endpoints you've defined. The server will execute the corresponding configurations and scripts and return the results.

## Benefits

- **Flexibility**: Easily add or modify endpoints without changing the server's code.
- **Rapid Prototyping**: Quickly prototype and test new API functionalities.
- **Customization**: Tailor the behavior of endpoints to specific use cases through configurations and scripts.

## Contributing

As the project evolves, contributions are welcome. Please ensure that any added endpoints are well-documented and that changes are communicated effectively.