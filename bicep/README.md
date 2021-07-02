# How to deploy infrastructure

## Deploy manually

1. Create a new resource group to deploy your Azure infrastructure into.

```
New-AzResourceGroup -Name rg-dapr-workshop-ussc-demo -Location ussc
```

2. Generate an SSH key pair if you don't already have one.

```
ssh-keygen -t rsa -b 2048
```

Copy your public SSH key string so you can configure the AKS cluster to use it. It can be found in the "~/.ssh/id_rsa.pub" file.

3. Configure & run the following PowerShell to set up the input parameters.

```
$params = @{ 
  appName="dapr"
  region="ussc"
  environment="demo"
  adminUsername="adminBruce"
  publicSSHKey="ssh-rsa AAAAB...wnBTn bruce.wayne@wayneenterprises.com"
}
```

4. Run the following PowerShell command to deploy.

```
New-AzResourceGroupDeployment -ResourceGroupName rg-dapr-workshop-ussc-demo -TemplateParameterObject $params -TemplateFile ./main.bicep -Verbose
```

In the outputs for this command, you will find the name of the various Azure resources that have been created.
You will need these to configure your Dapr services.

```
Name                     Type     Value
=======================  =======  ==========
serviceBusName           String   sb-dapr-ussc-demo
redisCacheName           String   redis-dapr-ussc-demo
keyVaultName             String   kv-dapr-ussc-demo
logicAppName             String   logic-smtp-dapr-ussc-demo
containerRegistryName    String   crdaprusscdemo
aksName                  String   aks-dapr-ussc-demo
```

5. Run the following command to get the AKS credentials for your cluster.

```
Import-AzAksCredential -ResourceGroupName rg-dapr-workshop-ussc-demo -Name aks-dapr-ussc-demo -Force
```

Verify your "target" cluster is set correctly.

```
kubectl config get-contexts

CURRENT   NAME                 CLUSTER              AUTHINFO                                                    NAMESPACE
*         aks-dapr-ussc-demo   aks-dapr-ussc-demo   clusterUser_rg-dapr-workshop-ussc-demo_aks-dapr-ussc-demo
```

## GitHub Actions