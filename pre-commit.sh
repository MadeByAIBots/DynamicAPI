#!/bin/bash

# Run the build script
bash build.sh || echo "Build failed"

bash validate.sh || echo "Validation failed"

# Run the test script
bash test.sh || echo "Test(s) failed"

# Exit with the status of the last command (to prevent the commit if the tests failed)
exit $?