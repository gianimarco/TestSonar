
param(
    [Parameter(Mandatory=$true)]
    [string]
    $app_config_name,
    [Parameter(Mandatory=$true)]
    [string]
    $path,
    [Parameter(Mandatory=$true)]
    [string]
    $environment,
    [Parameter(Mandatory=$false)]
    [bool]
    $useenv = $false
)

$json_config = Get-Content "$path/appsettings.$environment.json" | Out-String | ConvertFrom-Json

if (-Not $useenv) {
    az login
} else {
    az login --service-principal -u "$env:ARM_CLIENT_ID" -p "$env:ARM_CLIENT_SECRET" --tenant "$env:ARM_TENANT_ID"
}
az appconfig kv set --name $app_config_name --key "Primary:EventHubs:ConnectionString" --label $environment --value $json_config.EventHubs.ConnectionString --yes
az appconfig kv set --name $app_config_name --key "Primary:EventHubs:EventHubName" --label $environment --value $json_config.EventHubs.EventHubName --yes
az appconfig kv set --name $app_config_name --key "Primary:ApplicationInsights:InstrumentationKey" --label $environment --value $json_config.ApplicationInsights.InstrumentationKey --yes
az appconfig kv set --name $app_config_name --key "DT.SBUS.beapipayat:RouteDetails:AccountNumberPrefix" --label $environment --value $json_config.RouteDetails.AccountNumberPrefix --yes
az appconfig kv set --name $app_config_name --key "DT.SBUS.beapipayat:RouteDetails:loginId" --label $environment --value $json_config.RouteDetails.loginId --yes
az appconfig kv set --name $app_config_name --key "DT.SBUS.beapipayat:RouteDetails:EndPointUrl" --label $environment --value $json_config.RouteDetails.EndPointUrl --yes