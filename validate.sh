#!/bin/bash

# Validate openapi.yaml
yamllint -s DynamicApiServer/wwwroot/openapi.yaml

swagger-cli validate DynamicApiServer/wwwroot/openapi.yaml

# Validate ai-plugin.json
jsonlint -q DynamicApiServer/wwwroot/.well-known/ai-plugin.json

#!/bin/bash
find ./config -name "*.json" -print0 | while IFS= read -r -d '' file; do
    jsonlint -q "$file" || echo "Invalid JSON: $file"
done
