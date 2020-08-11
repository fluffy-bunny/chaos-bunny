docker images
az acr login --name acrchaosbunny
docker tag chaosbunny/app:latest acrchaosbunny.azurecr.io/chaosbunny/app:latest

docker push acrchaosbunny.azurecr.io/chaosbunny/app:latest
