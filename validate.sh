#!/bin/bash

# Validate openapi.yaml
yamllint -s DynamicApiServer/wwwroot/openapi.yaml

# Validate ai-plugin.json
jsonlint -q DynamicApiServer/wwwroot/.well-known/ai-plugin.json