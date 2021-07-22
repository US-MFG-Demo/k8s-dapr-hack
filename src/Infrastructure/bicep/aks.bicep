param longName string
param adminUsername string
param publicSSHKey string
param keyVaultName string

resource logAnalytics 'Microsoft.OperationalInsights/workspaces@2020-10-01' = {
  name: 'la-${longName}'
  location: resourceGroup().location  
}

resource aksUserAssignedManagedIdentity 'Microsoft.ManagedIdentity/userAssignedIdentities@2018-11-30' = {
  name: 'mi-aks-${longName}'
  location: resourceGroup().location
}

resource aksAzurePolicy 'Microsoft.Authorization/policyAssignments@2019-09-01' = {
  name: 'aksAzurePolicy'
  scope: resourceGroup()
  properties: {
    policyDefinitionId: '/providers/Microsoft.Authorization/policyDefinitions/c26596ff-4d70-4e6a-9a30-c2506bd2f80c'
  }  
}

resource keyVault 'Microsoft.KeyVault/vaults@2021-04-01-preview' existing = {
  name: keyVaultName  
}

resource aksUserAssignedManagedIdentityKeyVaultSecretsUserRoleAssignment 'Microsoft.Authorization/roleAssignments@2021-04-01-preview' = {
  name: guid(resourceGroup().name, aksUserAssignedManagedIdentity.id, 'Key Vault Secrets User')
  scope: keyVault
  properties: {
    principalId: aksUserAssignedManagedIdentity.properties.principalId
    roleDefinitionId: subscriptionResourceId(subscription().subscriptionId, 'Microsoft.Authorization/roleDefinitions', '4633458b-17de-408a-b874-0445c86b69e6')
  }  
}

resource aks 'Microsoft.ContainerService/managedClusters@2021-03-01' = {
  name: 'aks-${longName}'
  location: resourceGroup().location
  dependsOn: [
    aksAzurePolicy
  ]
  identity: {
    type: 'UserAssigned'
    userAssignedIdentities: {
      '${aksUserAssignedManagedIdentity.id}': {}
    }
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
    podIdentityProfile: {
      enabled: true
      allowNetworkPluginKubenet: true
    }
  }
}

output aksName string = aks.name
output aksfqdn string = aks.properties.fqdn
output aksazurePortalFQDN string = aks.properties.azurePortalFQDN
output aksNodeResourceGroupName string = aks.properties.nodeResourceGroup
output logAnalyticsName string = logAnalytics.name
output aksManagedIdentityName string = aksUserAssignedManagedIdentity.name
output aksManagedIdentityResourceId string = aksUserAssignedManagedIdentity.id
output aksManagedIdentityClientId string = aksUserAssignedManagedIdentity.properties.clientId
