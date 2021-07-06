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

output serviceBusName string = serviceBusModule.outputs.serviceBusName
output redisCacheName string = redisCacheModule.outputs.redisCacheName
output redisCachePrimaryAccessKey string = redisCacheModule.outputs.redisCachePrimaryAccessKey
output keyVaultName string = keyVaultModule.outputs.keyVaultName
output logicAppName string = logicAppModule.outputs.logicAppName
output containerRegistryName string = containerRegistryModule.outputs.containerRegistryName
output aksName string = aksModule.outputs.aksName
