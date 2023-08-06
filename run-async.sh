#!/bin/bash

source config_utils.sh

URL=$(get_config_value 'Url')
PORT=$(get_config_value 'Port')

nohup bash run.sh > run.log &

echo "The application is running... listening on $URL:$PORT"