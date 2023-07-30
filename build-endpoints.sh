#!/bin/bash

# Function to build a .NET project and verify the 'bin' directory
build_and_verify_dotnet_project() {
    cd $1
    dotnet build
    cd ..
    if [ -d "bin" ]; then
        echo "Successfully built $1"
    else
        echo "Error: Build failed. 'bin' directory not found in $1"
        exit 1
    fi
    cd ..
}

# Function to build .NET projects in a directory
build_dotnet_projects_in_directory() {
    for subdir in $1*/ ; do
        if ls ${subdir}*.csproj 1> /dev/null 2>&1; then
            build_and_verify_dotnet_project $subdir
        fi
    done
}

# Function to build all .NET endpoints
build_all_dotnet_endpoints() {
    for dir in config/endpoints/*/ ; do
        if [ -f ${dir}csharp.json ]; then
            build_dotnet_projects_in_directory $dir
        fi
    done
}

# Call the function to build all .NET endpoints
build_all_dotnet_endpoints