Add-AzureAccount

# then, y'know, you'll be asked to log in

#so now, make sure you're in the azure3lhotka subscription
Select-AzureSubscription -SubscriptionId "d2201247-1c2f-44f5-9c4b-8d5a03cee6b5"

# this creates a new server and stores the resulting name away for us
$newDb = New-AzureSqlDatabaseServer -AdministratorLogin "myvote" -AdministratorLoginPassword "whateverUwant" -Location "Central US"
$newServerName = $newDb.ServerName

# create a prod environment 
# create two websites, one for API and one for Web
New-AzureWebsite -Location "Central US" -Hostname "myvote.azurewebsites.net" -Name "myvote" -Verbose
New-AzureWebsite -Location "Central US" -Hostname "myvoteapi.azurewebsites.net" -Name "myvoteapi" -Verbose
# create the databases - I think _data is the reporting one? Standard gets you more space. I'm not sure.
# here's where we use the server name, though. 
New-AzureSqlDatabase -DatabaseName "myvote_db" -ServerName $newServerName -Edition Basic
New-AzureSqlDatabase -DatabaseName "myvote_data_db" -ServerName $newServerName -Edition Standard
# okay, this part's fun.
# Azure Mobile Services aren't in Azure PowerShell. NEAT
# so this requires Azure CLI: https://azure.microsoft.com/en-us/documentation/articles/xplat-cli-install/
azure login # this results in another challenge / response - not sure if it can be automated or if we want that
azure mobile create -p nh "myvote-dev" "myvotesql" "whateverUwant"
# storage accounts
New-AzureStorageAccount -Location "Central US" -StorageAccountName "myvoteapp" 
# a little tricky: you can't tell it where to put a container. 
# you have to switch the context for the subscription so that the storage account you *want* is the *default*. 
Set-AzureSubscription -CurrentStorageAccountName myvoteapp -SubscriptionId "d2201247-1c2f-44f5-9c4b-8d5a03cee6b5"
New-AzureStorageContainer -Name "pollimages"

# create a dev environment 
# still up in the air about some of this tbh
# well, I know we need separate websites. that's a good start. 
New-AzureWebsite -Location "Central US" -Hostname "myvote-dev.azurewebsites.net" -Name "myvote-dev" -Verbose
New-AzureWebsite -Location "Central US" -Hostname "myvoteapi-dev.azurewebsites.net" -Name "myvoteapi-dev" -Verbose
# we don't create new database servers, just new databases. Identically provisioned. 
New-AzureSqlDatabase -DatabaseName "myvote_db_dev" -ServerName $newServerName -Edition Basic
New-AzureSqlDatabase -DatabaseName "myvote_data_db_dev" -ServerName $newServerName -Edition Standard
# definitely not sure about this one: we may only need one storage account.
# I can't imagine why you couldn't just have two containers in that account, but maybe. 
# one benefit is that the account is a little more "server"-like so we abstract that a bit higher. I don't know. 
New-AzureStorageAccount -Location "Central US" -StorageAccountName "myvoteappdev" 
Set-AzureSubscription -CurrentStorageAccountName myvoteappdev -SubscriptionId "d2201247-1c2f-44f5-9c4b-8d5a03cee6b5"
# identical name in the storage container. 
New-AzureStorageContainer -Name "pollimages"