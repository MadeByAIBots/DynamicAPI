#!/bin/bash

publishDir="publish/"

rm -r $publishDir

# Stop it first or publish command will fail as binaries are in use
bash stop.sh

dotnet publish -o $publishDir/bin

cp install.sh $publishDir
cp config.json $publishDir
cp generate-auth-token.sh $publishDir
cp endpoints/ $publishDir/endpoints/ -r
cp example-file.txt $publishDir -f
cp stop.sh $publishDir -f
cp run.sh $publishDir -f
cp run-async.sh $publishDir -f
cp run-and-test.sh $publishDir -f
cp test-running.sh $publishDir -f
