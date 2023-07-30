#!/bin/bash

publishDir="publish/"

rm -r $publishDir

dotnet publish -o $publishDir

cp install.sh $publishDir
cp config.json $publishDir
cp generate-auth-token.sh $publishDir
