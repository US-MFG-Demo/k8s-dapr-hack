# Assignment 0 - Install tools and Azure pre-requisites

## Assignment goals

In this assignment, you'll configure and make sure you have all the pre-requisites installed on your machine as well as have all the Azure resources created.
Once you start creating the Azure resources you can move on to Assignment 1 while the resources get created, which can take from 5 to 25 minutes, depending on the region used.

## Step 1. Install pre-requisites

1. Make sure you have access to an Azure Subscription as a contributor where you can deploy resources to.
- Access to an Azure subscription with Contributor access
   - If you don't have one, [Sign Up for Azure HERE](https://azure.microsoft.com/en-us/free/)
   - If you already have one, make sure you have at least Contributor access ([instructions](https://docs.microsoft.com/en-us/azure/role-based-access-control/check-access)) 
     - Your IT organization may have given Contributor access to a resource group only, not the entire subscription. If that's the case, take note of that resource group name and make sure you have Contributor access to it, using the instructions linked

1. Install all the pre-requisites listed above and make sure they're working fine

- Git ([download](https://git-scm.com/))
- .NET 5 SDK ([download](https://dotnet.microsoft.com/download/dotnet/5.0))
- Visual Studio Code ([download](https://code.visualstudio.com/download)) with at least the following extensions installed:
  - [C#](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csharp)
  - [REST Client](https://marketplace.visualstudio.com/items?itemName=humao.rest-client)
- Docker for desktop ([download](https://www.docker.com/products/docker-desktop))
- Dapr CLI and Dapr runtime ([instructions](https://docs.dapr.io/getting-started/install-dapr-selfhost/))
- Install Azure CLI ([instructions]())
  - Linux ([instructions](https://docs.microsoft.com/en-us/azure/azure-resource-manager/bicep/install#linux))
  - macOS ([instructions](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli-macos))
  - Windows ([instructions](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli-windows?tabs=azure-cli))
- Install Azure CLI Bicep tools ([instructions](https://docs.microsoft.com/en-us/azure/azure-resource-manager/bicep/install#azure-cli))
- Install Bicep extension for VS Code ([instructions](https://marketplace.visualstudio.com/items?itemName=ms-azuretools.vscode-bicep))
- If you are on Windows, you may need a bash shell to run some of the commands. Use the [Windows Subsystem for Linux 2](https://docs.microsoft.com/en-us/windows/wsl/install-win10)

Make sure you have at least the following versions installed. This workshop has been tested with the following versions:

| Attribute            | Details |
| -------------------- | ------- |
| Dapr runtime version | v1.0.0  |
| Dapr.NET SDK version | v1.0.0  |
| Dapr CLI version     | v1.0.0  |
| Platform             | .NET 5  |
| azure-cli            | 2.24.0  |

2. Clone the Github repository to a local folder on your machine:

   ```shell
   git clone https://github.com/robvet/dapr-workshop.git
   ```
Now we need to create the Azure resources will be using for the subsequent assignments. 

You will use Bicep and PowerShell to create the resources needed:

1. If you are using [Azure Cloud Shell](https://shell.azure.com)) then you can skip this step. Open the [terminal window](https://code.visualstudio.com/docs/editor/integrated-terminal) in VS Code and make sure you're logged in to Azure

   ```shell
   az login
   ```
2. Make sure you have selected the subscription you want to work in. Replace the Xs with your subscription GUID or subscription name:

   ```shell
   az account set --subscription "xxxx-xxxx-xxxx-xxxx"
   ```
3. Generate an SSH key pair if you don't already have one.

   ```shell
   ssh-keygen -t rsa -b 2048
   ```
   Copy your public SSH key string so you can configure the parameter file in the next step to use it. It can be found in the "id_rsa.pub" file that was created or updated by the `ssh-keygen` command.

4. Modify the `src\Infrastructure\bicep\main.parameters.json` file so it contains the proper data for your deployment:

   ```json
   {
      "appName": {
         "value": "dapr"
      },
      "region": {
         "value": "southcentralus"
      },
      "environment": {
         "value": "youruniqueid123"
      },
      "adminUserName": {
         "value": "adminbruce"
      },
      "publicSSHKey": {
         "value": "ssh-rsa AAAAB...wnBTn bruce.wayne@wayneenterprises.com"        
      }
   }
   ```

3. Create a new resource group to deploy your Azure infrastructure into, by deploying the `src\Infrastructure\rg.bicep` file and store the name of the created resource group name in a variable. Make sure to replace the location parameter below with the proper Azure region that you want to use:

   ```shell
   az deployment sub create --location "southcentralus" --template-file rg.bicep --parameters .\main.parameters.json --query "properties.outputs" --output yamlc
   ```

4. The previous command should have ended with success, by displaying the name of the resource group that was created. Something similar to this:  

   ```yaml
   resourceGroupName:
   type: String
   value: rg-dapr-youruniqueid123
   ```

5. Take note of the resource group name. You're now ready to create all the Azure resources under that resource group. To do that, run the following Azure CLI command:

   ```shell
   az deployment group create --resource-group "rg-dapr-youruniqueid123" --template-file main.bicep --parameters .\main.parameters.json --query "properties.outputs" --output yamlc
   ```

   **NOTE**: This is a long-running command and may take several minutes. You're encouraged to jump to the next lab while the command is creating all the Azure resources.

   In the outputs for this command, you will find the name of the various Azure resources that have been created.
   You will need these to configure your Dapr services.

   ```yaml
   aksFQDN:
      type: String
      value: dapr-mce123-609718f5.hcp.southcentralus.azmk8s.io
   aksName:
      type: String
      value: aks-dapr-mce123
   aksazurePortalFQDN:
      type: String
      value: dapr-mce123-609718f5.portal.hcp.southcentralus.azmk8s.io
   containerRegistryLoginServerName:
      type: String
      value: crdaprmce123.azurecr.io
   containerRegistryName:
      type: String
      value: crdaprmce123
   eventHubEntryCamName:
      type: String
      value: ehn-dapr-mce123-trafficcontrol/entrycam
   eventHubExitCamName:
      type: String
      value: ehn-dapr-mce123-trafficcontrol/exitcam
   eventHubNamespaceHostName:
      type: String
      value: https://ehn-dapr-mce123-trafficcontrol.servicebus.windows.net:443/
   eventHubNamespaceName:
      type: String
      value: ehn-dapr-mce123-trafficcontrol
   iotHubName:
      type: String
      value: iothub-dapr-mce123
   keyVaultName:
      type: String
      value: kv-dapr-mce123
   logicAppAccessEndpoint:
      type: String
      value: https://prod-29.southcentralus.logic.azure.com:443/workflows/9bd179c8dd7049b8a152e5f2608f8efc
   logicAppName:
      type: String
      value: logic-smtp-dapr-mce123
   redisCacheName:
      type: String
      value: redis-dapr-mce123
   serviceBusEndpoint:
      type: String
      value: https://sb-dapr-mce123.servicebus.windows.net:443/
   serviceBusName:
      type: String
      value: sb-dapr-mce123
   storageAccountContainerName:
      type: String
      value: trafficcontrol
   storageAccountKey:
      type: String
      value: 7Ck76nP/5kFEhNx6C...V85L+0dFMFOA/xJLIvK25f2irUmVouPRbSGXKEzRQ==
   storageAccountName:
      type: String
      value: sadaprmce123
   ```

5. Run the following command to get the AKS credentials for your cluster.

   ```shell
   az aks get-credentials --name "<aksname>" --resource-group "<resource-group-name>"
   ```

   Verify your "target" cluster is set correctly.

   ```shell
   kubectl config get-contexts
   ```

   Your results should look something like this.

   ```shell
   CURRENT   NAME                 CLUSTER              AUTHINFO                                                    NAMESPACE
   *         aks-dapr-mce123      aks-dapr-mce123      clusterUser_rg-dapr-mce123_aks-dapr-mce123
   ```

6. Install Dapr in your cluster

   Run the following command to initialize Dapr in your Kubernetes cluster using your current context.

   ```shell
   dapr init -k
   ```
   Your results should look something like this.

   ```shell
   Making the jump to hyperspace...
   Note: To install Dapr using Helm, see here: https://docs.dapr.io/getting-started/install-dapr-kubernetes/#install-with-helm-advanced

   Deploying the Dapr control plane to your cluster...
   Success! Dapr has been installed to namespace dapr-system. To verify, run `dapr status -k' in your terminal. To get started, go here: https://aka.ms/dapr-getting-started
   ```

   Verify with the following command.

   ```shell
   dapr status -k
   ```

   Your results should look something like this.

   ```shell
   NAME                   NAMESPACE    HEALTHY  STATUS   REPLICAS  VERSION  AGE  CREATED
   dapr-sentry            dapr-system  True     Running  1         1.2.2    1m   2021-07-02 08:45.44
   dapr-sidecar-injector  dapr-system  True     Running  1         1.2.2    1m   2021-07-02 08:45.44
   dapr-operator          dapr-system  True     Running  1         1.2.2    1m   2021-07-02 08:45.44
   dapr-dashboard         dapr-system  True     Running  1         0.6.0    1m   2021-07-02 08:45.44
   dapr-placement-server  dapr-system  True     Running  1         1.2.2    1m   2021-07-02 08:45.45
   ```

7. Assign RBAC permissions to AKS

   You need to grant the managed identity of AKS access to your Azure Container Registry so it can pull images. Run the following command.

   ```shell
   az aks update --name "<aksname>" --resource-group "<resource-group-name>" --attach-acr "<acrname>"
   ```

8. Assign RBAC permissions to KeyVault

   You will need to assign yourself access to the KeyVault so you can create secrets.

   ```shell
   az role assignment create --role "Key Vault Secrets Officer" --assignee "<user principal name>" --scope /subscriptions/<subscriptionId>/resourceGroups/<resource-group-name>/providers/Microsoft.KeyVault/vaults/<key-vault-name>
   ```

8. Go to [assignment 1](Assignment01/README.md).

