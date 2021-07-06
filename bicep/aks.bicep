param longName string
param adminUsername string
param publicSSHKey string

resource logAnalytics 'Microsoft.OperationalInsights/workspaces@2020-10-01' = {
  name: 'la-${longName}'
  location: resourceGroup().location  
}

resource aks 'Microsoft.ContainerService/managedClusters@2021-03-01' = {
  name: 'aks-${longName}'
  location: resourceGroup().location
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    kubernetesVersion: '1.19.11'
    dnsPrefix: longName
    enableRBAC: true
    agentPoolProfiles: [
      {
        name: 'agentpool'
        osDiskSizeGB: 0
        count: 3
        vmSize: 'Standard_DS2_v2'
        osType: 'Linux'
        mode: 'System'
      }
    ]
    linuxProfile: {
      adminUsername: adminUsername
      ssh: {
        publicKeys: [
          {
            keyData: publicSSHKey
          }
        ]
      }
    }
    addonProfiles: {
      httpApplicationRouting: {
        enabled: true
      }
      omsagent: {
        enabled: true
        config: {
          logAnalyticsWorkspaceResourceID: logAnalytics.id
        }
      }
    } 
  }
}

// var acrPullRoleId = '7f951dda-4ed3-4680-a7ca-43fe172d538d'

// resource aksGrantAcrPullRole 'Microsoft.Authorization/roleAssignments@2021-04-01-preview' = {
//   name: guid(resourceGroup().id, 'acrPullRoleAssignment')
//   properties: {
//     roleDefinitionId: acrPullRoleId
//     principalId: aks.properties.identityProfile.kubeletidentity.objectId 
//     principalType: 'ServicePrincipal'
//   }  
// }

// var keyVaultSecretsReaderRoleId = '4633458b-17de-408a-b874-0445c86b69e6'

// resource aksGrantKeyVaultSecretsReaderRole 'Microsoft.Authorization/roleAssignments@2021-04-01-preview' = {
//   name: guid(resourceGroup().id, 'keyVaultSecretsReaderRoleAssignment')
//   properties: {
//     roleDefinitionId: keyVaultSecretsReaderRoleId
//     principalId: aks.properties.identityProfile.kubeletidentity.objectId 
//     principalType: 'ServicePrincipal'
//   }  
// }

output aksName string = aks.name
output aksControlPlaneFQDN string = aks.properties.fqdn
output logAnalyticsName string = logAnalytics.name
