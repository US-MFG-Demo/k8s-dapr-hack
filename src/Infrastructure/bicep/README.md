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
Name                       Type     Value
=========================  =======  ==========
serviceBusName             String   sb-dapr-ussc-demo
redisCacheName             String   redis-dapr-ussc-demo
redisCachePrimaryAccessKey String   asdfasdflkasdflkjsdafdljksdf=
keyVaultName               String   kv-dapr-ussc-demo
logicAppName               String   logic-smtp-dapr-ussc-demo
containerRegistryName      String   crdaprusscdemo
aksName                    String   aks-dapr-ussc-demo
```

5. Run the following command to get the AKS credentials for your cluster.

```
Import-AzAksCredential -ResourceGroupName rg-dapr-workshop-ussc-demo -Name aks-dapr-ussc-demo -Force
```

Verify your "target" cluster is set correctly.

```
kubectl config get-contexts
```

Your results should look something like this.

```
CURRENT   NAME                 CLUSTER              AUTHINFO                                                    NAMESPACE
*         aks-dapr-ussc-demo   aks-dapr-ussc-demo   clusterUser_rg-dapr-workshop-ussc-demo_aks-dapr-ussc-demo
```

6. Install Dapr in your cluster

Run the following command to initialize Dapr in your Kubernetes cluster using your current context.

```
dapr init -k
```

Your results should look something like this.

```
Making the jump to hyperspace...
Note: To install Dapr using Helm, see here: https://docs.dapr.io/getting-started/install-dapr-kubernetes/#install-with-helm-advanced

Deploying the Dapr control plane to your cluster...
Success! Dapr has been installed to namespace dapr-system. To verify, run `dapr status -k' in your terminal. To get started, go here: https://aka.ms/dapr-getting-started
```

Verify with the following command.

```
dapr status -k
```

Your results should look something like this.

```
  NAME                   NAMESPACE    HEALTHY  STATUS   REPLICAS  VERSION  AGE  CREATED
  dapr-sentry            dapr-system  True     Running  1         1.2.2    1m   2021-07-02 08:45.44
  dapr-sidecar-injector  dapr-system  True     Running  1         1.2.2    1m   2021-07-02 08:45.44
  dapr-operator          dapr-system  True     Running  1         1.2.2    1m   2021-07-02 08:45.44
  dapr-dashboard         dapr-system  True     Running  1         0.6.0    1m   2021-07-02 08:45.44
  dapr-placement-server  dapr-system  True     Running  1         1.2.2    1m   2021-07-02 08:45.45
```

7. Assign RBAC permissions to AKS

You need to grant the managed identity of AKS access to your Azure Container Registry so it can pull images. Run the following command.

```
az aks update -n aks-dapr-ussc-demo -g rg-dapr-workshop-ussc-demo --attach-acr crdaprusscdemo
```

## GitHub Actions