get_config_value() {
    local key=$1

    override_file='config.override.json'
    default_file='config.json'

    local value
    if [ -f "${override_file}" ]; then
        value=$(jq -r ".${key}" ${override_file})
    fi

    if [ "${value}" == "null" ] || [ -z "${value}" ]; then
        value=$(jq -r ".${key}" ${default_file})
    fi

    echo "${key}: ${value}" >&2
    
    # Return the value 
    echo "${value}"
}