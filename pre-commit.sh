#!/bin/bash

# Run the build script
bash build.sh

# Run the test script
bash test.sh

# Exit with the status of the last command (to prevent the commit if the tests failed)
exit $?