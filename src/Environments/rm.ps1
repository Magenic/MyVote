Login-AzureRmAccount

# the sql server and database
New-AzureRmResourceGroupDeployment -Name "orl-16-02" -ResourceGroupName "MyVote" -TemplateFile .\database\template.json -TemplateParameterFile .\database\parameters.json
# it will ask you for the SQL admin password - make sure it's one you gave to everyone else!

# the website
New-AzureRmResourceGroupDeployment -Name "orl-16-02" -ResourceGroupName "MyVote" -TemplateFile .\myvote\template.json -TemplateParameterFile .\myvote\parameters.json -nameFromTemplate "myvote"

# the webservice
New-AzureRmResourceGroupDeployment -Name "orl-16-02" -ResourceGroupName "MyVote" -TemplateFile .\myvoteapi\template.json -TemplateParameterFile .\myvoteapi\parameters.json -nameFromTemplate "myvoteapi"

# the mobile app
New-AzureRmResourceGroupDeployment -Name "orl-16-02" -ResourceGroupName "MyVote" -TemplateFile .\mobileapp\template.json -TemplateParameterFile .\mobileapp\parameters.json -nameFromTemplate "mbl-myvote"

# the storage account
New-AzureRmResourceGroupDeployment -Name "orl-16-02" -ResourceGroupName "MyVote" -TemplateFile .\filestorage\template.json -TemplateParameterFile .\filestorage\parameters.json -nameFromTemplate "myvoteapp"

# the blob container
New-AzureStorageContainer -Name "pollimages" -Context ((Get-AzureRmStorageAccount -ResourceGroupName "MyVote" -StorageAccountName "myvoteapp").Context)
# you will need to provide a key here - there's a possibility it was created in the storage account step. 