param longName string

resource storageAccount 'Microsoft.Storage/storageAccounts@2021-04-01' = {
  name: toLower('sa${replace(longName, '-', '')}')
  location: resourceGroup().location
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'BlobStorage'
  properties: {
    accessTier: 'Hot'
  }
}

var storageAccountContainerName = 'trafficcontrol'

resource storageAccountContainer 'Microsoft.Storage/storageAccounts/blobServices/containers@2021-04-01' = {
  name: '${storageAccount.name}/default/${storageAccountContainerName}'
}

output storageAccountName string = storageAccount.name
output storageAccountContainerName string = storageAccountContainerName
output storageAccountKey string = listKeys(storageAccount.id, storageAccount.apiVersion).keys[0].value
