dotnet clean

#!/bin/bash

# Function to delete directories and print their names
delete_dirs() {
    for dir in $(find . -type d -name $1); do
        echo "Deleting $dir"
        rm -rf $dir
    done
}

# Delete 'bin' and 'obj' directories
delete_dirs bin
delete_dirs obj
