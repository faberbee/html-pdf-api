#!/bin/sh

# Build a production version of the image
echo Building faberbee/htmlpdf-api:build
docker build -t faberbee/htmlpdf-api:build . -f ./k8s/Dockerfile.build

# Extract the content of build image to the local machine in ./app/publish folder
echo Extracting built files from faberbee/htmlpdf-api:build
mkdir -p ./app/publish
docker container create --name extract faberbee/htmlpdf-api:build  
docker container cp extract:/app/publish ./app
docker container rm -f extract

# Create the final image by cloning the content from local machine to image
echo Building faberbee/htmlpdf-api:latest
docker build --no-cache -t faberbee/htmlpdf-api:latest . -f ./k8s/Dockerfile
rm -r ./app

# Optionally run
docker run faberbee/htmlpdf-api:latest