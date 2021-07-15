param appName string
param region string
param environment string
param adminUsername string
param publicSSHKey string

var longName = '${appName}-${region}-${environment}'

module keyVaultModule 'keyVault.bicep' = {
  name: 'keyVaultDeploy'
  params: {
    longName: longName
  }
}

module serviceBusModule 'serviceBus.bicep' = {
  name: 'serviceBusDeploy'
  params: {
    longName: longName
  }
}

module logicAppModule 'logicApp.bicep' = {
  name: 'logicAppDeploy'
  params: {
    longName: longName
  }  
}

module containerRegistryModule 'containerRegistry.bicep' = {
  name: 'containerRegistryDeploy'
  params: {
    longName: longName
  }
}

module aksModule 'aks.bicep' = {
  name: 'aksDeploy'
  params: {
    longName: longName
    adminUsername: adminUsername
    publicSSHKey: publicSSHKey
  }  
}

module redisCacheModule 'redisCache.bicep' = {
  name: 'redisCacheDeploy'
  params: {
    longName: longName
  }  
}

module mqttModule 'mqtt.bicep' = {
  name: 'mqttDeploy'
  params: {
    longName: longName
  }  
}

module storageAccountModule 'storage.bicep' = {
  name: 'storageAccountDeploy'
  params: {
    longName: longName
  }  
}

output serviceBusName string = serviceBusModule.outputs.serviceBusName
output serviceBusEndpoint string = serviceBusModule.outputs.serviceBusEndpoint
output redisCacheName string = redisCacheModule.outputs.redisCacheName
output redisCachePrimaryAccessKey string = redisCacheModule.outputs.redisCachePrimaryAccessKey
output keyVaultName string = keyVaultModule.outputs.keyVaultName
output logicAppName string = logicAppModule.outputs.logicAppName
output logicAppAccessEndpoint string = logicAppModule.outputs.logicAppAccessEndpoint
output containerRegistryName string = containerRegistryModule.outputs.containerRegistryName
output containerRegistryLoginServerName string = containerRegistryModule.outputs.containerRegistryLoginServerName
output aksName string = aksModule.outputs.aksName
output aksFQDN string = aksModule.outputs.aksfqdn
output aksazurePortalFQDN string = aksModule.outputs.aksazurePortalFQDN
output iotHubName string = mqttModule.outputs.iotHubName
output eventHubNamespaceName string = mqttModule.outputs.eventHubNamespaceName
output eventHubNamespaceHostName string = mqttModule.outputs.eventHubNamespaceHostName
output eventHubEntryCamName string = mqttModule.outputs.eventHubEntryCamName
output eventHubExitCamName string = mqttModule.outputs.eventHubExitCamName
output storageAccountName string = storageAccountModule.outputs.storageAccountName
output storageAccountContainerName string = storageAccountModule.outputs.storageAccountContainerName
output storageAccountKey string = storageAccountModule.outputs.storageAccountKey
