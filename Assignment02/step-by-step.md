# Assignment 2 - Add Dapr service-to-service invocation

Welcome to the step-by-step instructions for assignment 2.

 > Be sure that Docker Desktop is running
## Step 1: Start the VehicleRegistrationService with Dapr


In assignment 1, you started all the services using `dotnet run` without using Dapr. The services then communicated among each other using direct HTTP calls. To evolve your application and use Dapr for service-to-service communication, you'll need the Dapr CLI to start Dapr sidecars for each service. The sidecars will then be responsible for cross-service communication. To do so, you'll need to provide values for a number of parameters:

- The service needs a unique id so that Dapr can find it. This is called the *app-id* (or application Id). You specify this with the `--app-id` flag on the command-line.

- Each service listens for requests on a different HTTP port to prevent port collisions on localhost. You'll run the VehicleRegistrationService on port `6002`. You'll inform Dapr using the `--app-port` flag on the command-line. Doing so enables the Dapr sidecar to communicate with the service.

- The service can communicate with the Dapr sidecar using  HTTP or gRPC. By default, Dapr sidecars listen on port `3500` and `50001`, respectively. But to prevent confusion, you'll use different port numbers in the lab assignments. To avoid port collisions on the local machine when running multiple services, you'll specify a unique HTTP and gRPC port per service. You specify this with the `--dapr-http-port` and `--dapr-grpc-port` flags on the command-line. Throughout the workshop, you will use the following ports:

  | Service                    | Application Port | Dapr sidecar HTTP port | Dapr sidecar gRPC port |
  | -------------------------- | ---------------- | ---------------------- | ---------------------- |
  | TrafficControlService      | 6000             | 3600                   | 60000                  |
  | FineCollectionService      | 6001             | 3601                   | 60001                  |
  | VehicleRegistrationService | 6002             | 3602                   | 60002                  |

- Finally you need to tell Dapr how to start the service. The services are .NET Core services which can be started with the command `dotnet run`.

You will use the Dapr CLI `run` command and specify all the options above on the command-line:

1. Make sure you have started Docker Desktop on your machine and the Dapr CLI and runtime are installed (see the [prerequisites](../README.md#prerequisites)).

2. Open the `src` folder in this repo in VS Code.

3. Open the [terminal window](https://code.visualstudio.com/docs/editor/integrated-terminal) in VS Code and make sure the current folder is `src/VehicleRegistrationService`.

4. Enter the following command to run the VehicleRegistrationService with a Dapr sidecar:

   ```console
   dapr run --app-id vehicleregistrationservice --app-port 6002 --dapr-http-port 3602 --dapr-grpc-port 60002 dotnet run
   ```

5. Check the logs for any errors. If running correctly, you'll see both Dapr and application logging in the output.

Now you're running a 'Daprized' version of the VehicleRegistrationService. It's important to note that there were **no code changes** for this to work. The VehicleRegistrationService is still just a web API listening for requests. But now, you've started it with a Dapr sidecar running next door. The Dapr side car knows about and can communicate with the VehicleRegistrationService. As well, other services can use Dapr to call the VehicleRegistrationService. This is what you'll do in the next step.

## Step 2: Call the VehicleRegistrationService using Dapr service invocation

In this step, you're going to change the code of the FineCollectionService so it uses Dapr service invocation to call the VehicleRegistrationService. The following figure depicts the update:

<img src="img/finecollection-to-vehicleregistration.png" style="padding-top: 25px;" />

First you're going to change the code so it calls the Dapr sidecar:

1. Open the file `src/FineCollectionService/Controllers/CollectionController.cs` using the in VS Code [Explorer](https://code.visualstudio.com/docs/getstarted/userinterface#_explorer).

1. Inspect the `CollectFine` method. Note how it contains a call to the VehicleRegistrationService to retrieve the vehicle info:

   ```csharp
   // get owner info
   var vehicleInfo = await _vehicleRegistrationService.GetVehicleInfo(speedingViolation.VehicleId);
   ```

   The `_vehicleRegistrationService` is an instance of a proxy (helper class) that uses the .NET `HttpClient` to call the VehicleRegistrationService. You are going to change that proxy so it uses Dapr service invocation.

1. Open the file `src/FineCollectionService/Proxies/VehicleRegistrationService.cs` in VS Code.

1. Inspect the `GetVehicleInfo` method. Note in the HTTP call how the URL of the VehicleRegistrationService (running on port 6002) is *hardcoded*.

1. The Dapr API template for calling the service invocation building block on a Dapr sidecar is:

   ```http
   http://localhost:<daprPort>/v1.0/invoke/<appId>/method/<method-name>
   ```

   > Note that the Dapr API calls are platform agnostic. Any development language that supports HTTP calls can invoke a Dapr sidecar using with this HTTP call template structure.

   You can substitute the placeholders in the template URL with values for the FineCollectionService. Here's how the specific URL will look:

   ```http
   http://localhost:3601/v1.0/invoke/vehicleregistrationservice/method/vehicleinfo/{licenseNumber}
   ```

   As you can see in this URL, the Dapr sidecar for the FineCollectionService will run on HTTP port `3601`.

1. Replace the URL in the code with the new Dapr service invocation URL. The code should now look like this:

   ```csharp
   public async Task<VehicleInfo> GetVehicleInfo(string licenseNumber)
   {
       return await _httpClient.GetFromJsonAsync<VehicleInfo>(
           $"http://localhost:3601/v1.0/invoke/vehicleregistrationservice/method/vehicleinfo/{licenseNumber}");
   }
   ```

   > It's important to grasp the [sidecar pattern](https://docs.dapr.io/concepts/overview/#sidecar-architecture) used by Dapr. The FineCollectionService calls the VehicleRegistrationService not directly, but **through its dapr sidecar**! The FineCollectionService no longer needs to know the endpoint of the VehicleRegistrationService. Its Dapr sidecar will locate the endpoint for VehicleRegistration based on the `app-id` specified in the URL. Once found, the sidecar for FineCollection will call the sidecar for VehicleRegistration. The VehicleRegistration sidecar will then invoke the vehicleInfo method on the VehicleRegistration service, passing in the license plate number.

1. With the VehicleRegistrationService up and running, you'll now start the FineCollectionService:

   <img src="img/start-finecollection-assignment02.png" style="padding-top: 25px;" /> 

1. Open a **new** terminal window in VS Code and make sure the current folder is set to `src/FineCollectionService`.

1. Check all your code-changes are correct by building the code:

   ```console
   dotnet build
   ```

   If you see any warnings or errors, review the previous steps to make sure the code is correct.

1. Enter the following command to run the FineCollectionService with a Dapr sidecar:

   ```console
   dapr run --app-id finecollectionservice --app-port 6001 --dapr-http-port 3601 --dapr-grpc-port 60001 dotnet run
   ```

1. Check the logs for any errors. As you can see, both Dapr and the application logging will be shown.

Now you're going to test the application from end-to-end:

1. First, start the TrafficControlService:

   <img src="img/start-trafficcontrol-assignment02.png" style="padding-top: 25px;" /> 

1. Open a **new** terminal window in VS Code and set the current folder to `src/TrafficControlService`.

1. Enter the following command to run the TrafficControlService with a Dapr sidecar:

1. ```console
   dotnet run
   ```

   > The TrafficControlService does not need to run with a Dapr sidecar in this assignment. This is because it will still call the FineCollectionService over HTTP as before.

1. Finally, you're going start the traffic simulation service:

   <img src="img/start-simulator-assignment02.png" style="padding-top: 25px;" /> 

1. Open a **new** terminal window in VS Code and set the current folder to `src/Simulation`.

1. Start the simulation:

   ```console
   dotnet run
   ```

Recycle back through all the services running in the terminal. You should see similar logging as before when you ran the application. So, the functionality works the same, but now you use Dapr service invocation to communicate between the FineCollectionService and the VehicleRegistrationService.

## Step 3: Use Dapr service invocation with the Dapr SDK for .NET

In step 2, you used the Dapr HTTP API to call the Dapr Service Invocation building block. In this step, you're going to simplify the application by using the **Dapr SDK for .NET** to call the VehicleRegistrationService. The SDK provides a simple and intuitive way to invoke the Dapr sidecar APIs that tightly integrates with the .NET Core platform.

First stop the simulation:

1. Open the terminal window in VS Code in which the Camera Simulation runs (the last service you started in the step 2).

1. Stop the simulation by pressing `Ctrl-C` and close the terminal window by clicking the trashcan icon in its title bar (or typing the `exit` command).

1. Open the terminal window in VS Code in which the FineCollectionService runs (the second service you started in step 2).

1. Stop the service by pressing `Ctrl-C`, but do not close the terminal window. Keep this terminal window open and focused.

1. In the FineCollectionService terminal window, add a reference to the Dapr ASP.NET Core integration library:

   ```console
   dotnet add package Dapr.AspNetCore
   ```

   > The `Dapr.AspNetCore` package contains the `DaprClient` class, which is used to directly invoke the Dapr API as well as additional integrations with ASP.NET Core. Because the services are all ASP.NET Core web APIs, we'll use this package throughout the workshop.

Now you'll change the code in the FineCollectionService to use the Dapr SDK `HttpClient` integration to call the VehicleRegistrationService. The `HttpClient` integration allows you to use the .NET Core `HttpClient` object to make service calls, while the SDK ensures that calls are routed through the Dapr sidecar.

1. Open the file `src/FineCollectionService/Startup.cs` in VS Code.

1. Add a using statement in this file to make sure you can use the Dapr client:

   ```csharp
   using Dapr.Client;
   ```

1. The `ConfigureServices` method, contains these two lines of code which register the .NET `HttpClient` and the `VehicleRegistrationService`  proxy (which uses the `HttpClient`) with dependency injection:

   ```csharp
   // add service proxies
   services.AddHttpClient();
   services.AddSingleton<VehicleRegistrationService>();
   ```

1. Replace these two lines with with the following lines:

   ```csharp
   // add service proxies
   services.AddSingleton<VehicleRegistrationService>(_ => 
       new VehicleRegistrationService(DaprClient.CreateInvokeHttpClient(
           "vehicleregistrationservice", "http://localhost:3601")));
   ```

   With this snippet, you use the `DaprClient` to create an `HttpClient` instance to implement service invocation. You specify the `app-id` of the service you want to communicate with. You also need to specify the address of the Dapr sidecar for the FineCollectionService as its not using the default Dapr HTTP port (3500). The `HttpClient` instance created by Dapr is explicitly passed into the constructor of the `VehicleRegistrationService` proxy.

   > This is an example of the deep integration of Dapr with ASP.NET Core when using the `Dapr.AspNetCore` library. You can still use the `HttpClient` (and its rich feature-set) in your code, but under the hood it uses the Dapr service invocation building block to communicate.

1. Open the file `src/FineCollectionService/Proxies/VehicleRegistrationService.cs` in VS Code.

1. Because the `HttpClient` passed into this class has already been created with a specific `app-id`, you can omit the host information from the request URL. Change the URL that is used in the `GetVehicleInfo` to `/vehicleinfo/{license-number}`. The method should now look like this:

   ```csharp
   public async Task<VehicleInfo> GetVehicleInfo(string licenseNumber)
   {
       return await _httpClient.GetFromJsonAsync<VehicleInfo>(
           $"/vehicleinfo/{licenseNumber}");
   }
   ```

Now the FineCollectionService is changed to use the Dapr SDK for service invocation. Let's test this.

1. If you followed the instructions in this assignment, the VehicleRegistration, TrafficControl, and Simulation services are still running.

1. Open the terminal window in VS Code in which the FineCollectionService was running.

1. Execute the `dotnet build` to make sure the code is correct.

1. Enter the following command to start the changed FineCollectionService again:

   ```console
   dapr run --app-id finecollectionservice --app-port 6001 --dapr-http-port 3601 --dapr-grpc-port 60001 dotnet run
   ```

The services are up & running. Now you're going to test this using the simulation.

1. Open a **new** terminal window in VS Code and change the current folder to `src/Simulation`.

1. Start the simulation:

   ```console
   dotnet run
   ```

You should see similar logging as before when you ran the application.

## Step 4: Use Dapr observability

So how can you check whether or not the call to the VehicleRegistrationService is handled by Dapr? Well, Dapr has some observability built in. You can look at Dapr traffic using Zipkin:

1. Open a browser and go the this URL: [http://localhost:9411/zipkin](http://localhost:9411/zipkin).

2. Click the `RUN QUERY` button in the top right of the screen to search for traces.

3. You should see the calls between the FineCollectionService and the VehicleRegistrationService. You can expand and collapse each trace and click the `SHOW` button to get more details:

   ![](img/zipkin-traces.png)

4. If you click the dependencies button and search, you will see the services and the traffic flowing between them:

   ![](img/zipkin-dependencies.gif)

## Step 5: Deploy to Azure Kubernetes Service

Use Azure Container Registry Tasks to have the Azure Container Registry build & store your container image.

1. Navigate to the src/VehicleRegistrationService directory.

```
az acr build --registry crdaprusscdemo --image vehicleregistrationservice:latest .
```

2. Deploy the VehicleRegistrationService image to the Azure Kubernetes Service. You will need to get the DNS name and 
Azure Container Registry login server/repository:tag name to match your deployed Azure resources. Use these to replace the
"Ingress.spec.rules.host" and the "Deployment.spec.template.spec.containers.image" in the ./deploy/deploy.yaml file before running the "apply" command.

```
//helm template ./deploy/vehicleregistrationservice `
//--set ingress.rules[0].host=vehicleregistrationservice.e13e6fb6d2534a41ae60.southcentralus.aksapp.io `
//--set image.repository=crdaprusscdemo.azurecr.io/vehicleregistrationservice

kubectl apply -f ./deploy/deploy.yaml
```


## Next assignment

Congratulations! You have successfully completed assignment 2.

Make sure you stop all running processes and close all the terminal windows in VS Code before proceeding to the next assignment.

Go to [assignment 3](../Assignment03/README.md).