#!/bin/bash

# Publish the .NET API Project and create a Docker image

dotnet publish ./DynamicApiServer/DynamicApiServer.csproj -c Release -r linux-x64 --no-self-contained -o my-api-image:latest