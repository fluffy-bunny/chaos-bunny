# Github Actions Build

## Locally  
Using [nektos act](https://github.com/nektos/act) to run github actions locally.  
### bash 
Set the following environment variables that will become our secrets.  
```
export REGISTRY_NAME='acrchaosbunny'
export REGISTRY_LOGIN_SERVER='acrchaosbunny.azurecr.io'
export AZURE_CREDENTIALS='{{secrets}}'
export ARM_CLIENT_ID="bd96aa5e-ff6a-4062-b514-c372053507b3"
export ARM_CLIENT_SECRET="{{secrets}}"
export ARM_SUBSCRIPTION_ID=$(az account show --query id | xargs)
export ARM_TENANT_ID=$(az account show --query tenantId | xargs)
```

