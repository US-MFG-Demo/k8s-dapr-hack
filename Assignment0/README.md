# Assignment 0 - Install tools and pre-requisites

## Assignment goals

In this assignment, you'll configure and make sure you have all the pre-requisites installed on your machine.

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
- Install Azure PowerShell ([instructions](https://docs.microsoft.com/en-us/powershell/azure/install-az-ps?view=azps-6.2.1))
- Install PowerShell [Bicep](https://docs.microsoft.com/en-us/azure/azure-resource-manager/bicep/overview) tools (this will be used to deploy Azure resources)
- Install Azure CLI ([instructions]())
  - Linux ([instructions](https://docs.microsoft.com/en-us/azure/azure-resource-manager/bicep/install#linux))
  - macOS ([instructions](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli-macos))
  - Windows ([instructions](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli-windows?tabs=azure-cli))
- Install Azure CLI Bicep tools ([instructions](https://docs.microsoft.com/en-us/azure/azure-resource-manager/bicep/install#azure-cli))
- Install Bicep extension for VS Code ([instructions](https://marketplace.visualstudio.com/items?itemName=ms-azuretools.vscode-bicep))


All scripts in the instructions are Powershell scripts. If you're working on a Mac, it is recommended to install Powershell for Mac:

- Powershell for Mac ([instructions](https://docs.microsoft.com/nl-nl/powershell/scripting/install/installing-powershell-core-on-macos?view=powershell-7.1))

Make sure you have at least the following versions installed. This workshop has been tested with the following versions:

| Attribute            | Details |
| -------------------- | ------- |
| Dapr runtime version | v1.0.0  |
| Dapr.NET SDK version | v1.0.0  |
| Dapr CLI version     | v1.0.0  |
| Platform             | .NET 5  |
| Powershell           | >7.0.0  |

2. Clone the Github repository to a local folder on your machine:

   ```console
   git clone https://github.com/robvet/dapr-workshop.git
   ```

3. Review the source code of the different services. You can open the `src` folder in this repo in VS Code. All folders used in the assignments are specified relative to the root of the folder where you have cloned the dapr-workshop repository.

4. Deploy the Azure resources if you intend to deploy these Dapr services to Azure. Some of them can take some time to deploy, so let them deploy in the background.
   Follow the instructions in the src/Infrastructure/bicep/README.md file.

4. Go to [assignment 1](Assignment01/README.md).

