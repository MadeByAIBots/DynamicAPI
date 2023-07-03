#!/bin/bash
echo 'Starting deploy... Check deploy.log for output.'
nohup ./deploy.sh > deploy.log 2>&1 &
