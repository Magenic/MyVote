param (
    $AdministratorLogin,
    $AdministratorLoginPassword,
    $AzureWebsiteName,
    $ConnectionStringName,
    $DatabaseName,
    $ServerName
)

Set-AzureWebsite -Name $AzureWebsiteName -ConnectionStrings @{
    "Name"=$ConnectionStringName; 
    "ConnectionString" = "metadata=res://*/Model.csdl|res://*/Model.ssdl|res://*/Model.msl;`
                        provider=System.Data.SqlClient;`
                        provider connection string='`
                            Server=tcp:" + $ServerName + ".database.windows.net,1433;`
                            Database=" + $DatabaseName + ";`
                            User ID=" + $AdministratorLogin + "@" + $ServerName + ";`
                            Password=" + $AdministratorLoginPassword + ";`
                            Trusted_Connection=False;`
                            Encrypt=True;'"; 
    "Type"="Custom"}