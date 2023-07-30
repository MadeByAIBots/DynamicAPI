#!/bin/bash

# Start at the root directory of the endpoints
cd config/endpoints

# Loop through all directories
for dir in */ ; do
    # Check if 'csharp.json' file exists
    if [ -f "${dir}csharp.json" ]; then
        # Navigate into the directory
        cd $dir
        # Loop through all subdirectories
        for subdir in */ ; do
            # Check if '.csproj' file exists in the subdirectory
            if ls ${subdir}*.csproj 1> /dev/null 2>&1; then
                # Navigate into the subdirectory
                cd $subdir
                # Build the .NET project
                dotnet build
                # Output a success message
                echo "Successfully built ${dir}${subdir}"
                # Navigate back to the parent directory
                cd ..
            fi
        done
        # Navigate back to the root directory
        cd ..
    fi
done