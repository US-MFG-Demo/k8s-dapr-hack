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
    dnsPrefix: longName
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

output aksName string = aks.name
output aksControlPlaneFQDN string = aks.properties.fqdn
output logAnalyticsName string = logAnalytics.name
