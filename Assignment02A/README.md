# Assignment 2A - Deploy application to AKS

In this assignment, you're going to deploy the application, as is, to Azure Kubernetes Service (AKS). 
The idea here is not to teach you all that is possible with AKS, but to quickly create the resources in Azure to host the application from this point on.

Later in other assignments, you'll be adding other Azure services to the solution while it continues to run in AKS. 

## Dapr on AKS

TBD

## Assignment goals

To complete this assignment, you must achieve the following goals:

- Created an AKS cluster, linked to an ACR
- Enable Dapr on the AKS cluster
- Deploy the microservices to the cluster, with the changes applied to the application in the previous assginment
- Run the simulation and verify the microservices work ok in the cluster

There are two ways to approach this assignment: DIY (do it yourself) or with step-by-step instructions.

## DIY instructions

Open the `src\infrastructure\bicep` folder in this repo in VS Code. This folder has the templates you can leverage to deploy the components. If you need any hints, you may peek in the step-by-step approach.

## Step by step instructions

To leverage step-by-step instructions to achieve the goals, open the [step-by-step instructions](step-by-step.md).

## Next assignment

Once you've finished this assignment, stop all the running processes and close all the terminal windows in VS Code. Now proceed to the next assignment.

Go to [assignment 3](../Assignment03/README.md).
