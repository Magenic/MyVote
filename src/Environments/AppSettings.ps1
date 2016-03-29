param (
  [string] $AzureWebsiteName,
  [hashtable] $appsettings
)
Set-AzureWebsite -Name $AzureWebsiteName -AppSettings $appsettings