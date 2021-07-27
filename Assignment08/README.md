# Assignment 8 - Deploy to Azure Kubernetes Service (AKS)

In this assignment, you're going to deploy the Dapr-enabled services you have written locally to an [Azure Kubernetes Service (AKS)](https://docs.microsoft.com/en-us/azure/aks/) cluster.

![architecture](./img/architecture.png)

## Assignment goals

To complete this assignment, you must reach the following goals:

- Successfully deploy all 3 services (VehicleRegistrationService, TrafficControlService & FineCollectionService) to an AKS cluster.
- Successfully run the Simulation service locally that connects to your AKS-hosted services

## DIY instructions

1. 	Navigate to each service under the `src` directory, create images based upon the services and deploy these images to your Azure Container Registry. Hint: use [ACR Tasks](https://docs.microsoft.com/en-us/azure/container-registry/container-registry-tasks-overview) to simplify creation & deployment of the images to the registry.

2. 	Navigate to each service under the `src` directory and use the `deploy/deploy.yaml` spec files to deploy to AKS. You will need to customize them with the
   	names of your specific Azure container registries, etc.

3.	Run the `Simulation` app and verify your services are running.

## Step by step instructions

To get step-by-step instructions to achieve this goals, open the [step-by-step instructions](step-by-step.md).
