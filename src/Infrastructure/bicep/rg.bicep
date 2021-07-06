param p_subscriptionGuid = subscription().subscriptionId
param p_rgName string = 'rg${uniqueString(p_subscriptionGuid)}'
param p_location string = 'westus'

targetScope = 'subscription'

resource rg 'Microsoft.Resources/resourceGroups@2021-04-01' = {
  name: p_rgName
  location: p_location
}
