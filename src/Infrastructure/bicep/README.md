# How to deploy infrastructure

## Deploy manually

1. Create a new resource group to deploy your Azure infrastructure into.

   ```
   New-AzResourceGroup -Name rg-dapr-workshop-ussc-demo -Location ussc
   ```

2. Register some additional providers needed to deploy.

   ```
   az feature register --name EnablePodIdentityPreview --namespace Microsoft.ContainerService
   az provider register -n Microsoft.ContainerService
   ```

2. Generate an SSH key pair if you don't already have one.

   ```
   ssh-keygen -t rsa -b 2048
   ```

Copy your public SSH key string so you can configure the AKS cluster to use it. It can be found in the "~/.ssh/id_rsa.pub" file.

3. Configure & run the following PowerShell to set up the input parameters.

   ```powershell
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
   Name                                   Type                       Value
   =====================================  =========================  ==========
   subscriptionId                         String                     dcf66641-6312-4ee1-b296-723bb0a999ba
   resourceGroupName                      String                     rg-dapr-workshop-ussc-demo
   serviceBusName                         String                     sb-dapr-ussc-demo
   serviceBusEndpoint                     String                     https://sb-dapr-ussc-demo.servicebus.windows.net:443/
   redisCacheName                         String                     redis-dapr-ussc-demo
   redisCachePrimaryAccessKey             String                     qu4qw8bFmOOFakekeyFFC1YzVJvV7+v3raFBNA3M=
   keyVaultName                           String                     kv-dapr-ussc-demo
   logicAppName                           String                     logic-smtp-dapr-ussc-demo
   logicAppAccessEndpoint                 String                     https://prod-18.southcentralus.logic.azure.com:443/workflows/e76d81048c3941f18638ab0055bba68a
   containerRegistryName                  String                     crdaprusscdemo
   containerRegistryLoginServerName       String                     crdaprusscdemo.azurecr.io
   aksName                                String                     aks-dapr-ussc-demo
   aksFQDN                                String                     dapr-ussc-demo-5d6742a2.hcp.southcentralus.azmk8s.io
   aksazurePortalFQDN                     String                     dapr-ussc-demo-5d6742a2.portal.hcp.southcentralus.azmk8s.io
   aksNodeResourceGroupName               String                     MC_rg-dapr-workshop-ussc-demo_aks-dapr-ussc-demo_southcentralus
   aksManagedIdentityName                 String                     mi-aks-dapr-ussc-demo
   aksManagedIdentityResourceId           String
   /subscriptions/dcf66641-6312-4ee1-b296-723bb0a999ba/resourceGroups/rg-dapr-workshop-ussc-demo/providers/Microsoft.ManagedIdentity/userAssignedIdentities/mi-aks-dapr-ussc-demo
   aksManagedIdentityClientId             String                     de368b69-8ba7-4a29-9af4-fb513a09adf3
   iotHubName                             String                     iothub-dapr-ussc-demo
   eventHubNamespaceName                  String                     ehn-dapr-ussc-demo-trafficcontrol
   eventHubNamespaceHostName              String                     https://ehn-dapr-ussc-demo-trafficcontrol.servicebus.windows.net:443/
   eventHubEntryCamName                   String                     ehn-dapr-ussc-demo-trafficcontrol/entrycam
   eventHubExitCamName                    String                     ehn-dapr-ussc-demo-trafficcontrol/exitcam
   storageAccountName                     String                     sadaprusscdemo
   storageAccountEntryCamContainerName    String                     trafficcontrol-entrycam
   storageAccountExitCamContainerName     String                     trafficcontrol-exitcam
   storageAccountKey                      String                     IKJgQ4KAJOVDkwgaTLFakekeyAmi4zSz2ehm1btpQXZ+l68ol7wJmg8TA0ClQChRK7sWnvMEVexgg==
   appInsightsInstrumentationKey          String                     613bba51-Fake-4273-keys-ec9c40539b0f
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

8. Assign RBAC permissions for current user to Azure KeyVault.

   You need to grant yourself access to the KeyVault so you can add secrets. Modify the **$signInName**, **subscriptionId**, **resourceGroupName** and **$keyVaultName**

   ```powershell
   $signInName = "dwight.k.schrute@cfca.gov";
   $subscriptionId = "dcf66641-6312-4ee1-b296-723bb0a999ba";
   $resourceGroupName = "rg-dapr-workshop-ussc-demo";
   $keyVaultName = "kv-dapr-ussc-demo";
   New-AzRoleAssignment -RoleDefinitionName 'Key Vault Administrator' -SignInName $signInName -Scope "/subscriptions/$subscriptionId/resourcegroups/$resourceGroupName/providers/Microsoft.KeyVault/vaults/$keyVaultName"
   ```

9. Set up Application Insights so you can monitor all the services. 

   Navigate to the `src/VehicleRegistration` directory. Add the following NuGet package.

   ```
   dotnet add package Microsoft.ApplicationInsights.AspNetCore --version 2.17.0
   ```

   Add the `ApplicationInsights.InstrumentationKey` setting in the `src/VehicleRegistrationService/appsettings.json` file with the **appInsightsInstrumentationKey**. 

   ```json
   "ApplicationInsights": {
    "InstrumentationKey": "613bba51-Fake-4273-keys-ec9c40539b0f"
   }
   ```

   Add the Application Insights service to the application in the `src/VehicleRegistrationService/Startup.cs` file.

   ```csharp
   public void ConfigureServices(IServiceCollection services)
   {
      services.AddApplicationInsightsTelemetry();
      
      services.AddScoped<IVehicleInfoRepository, InMemoryVehicleInfoRepository>();

      services.AddControllers();
   }
   ```

   Repeat for the `FineCollectionService` and `TrafficControlService`.