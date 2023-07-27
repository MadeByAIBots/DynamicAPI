#!/bin/bash
version=$(cat version.txt)
dotnet pack -c Release /p:PackageVersion=$version -o ./packages