# This script will dynamically update the service URL on the Azure APIM API
# Output Terraform Output to File
$current_folder = Get-Location
$config_file_name = New-TemporaryFile
Set-Location "$env:SOURCE_DIR/terrakube"
terraform output -json > $config_file_name.FullName
Set-Location $current_folder

# Grab required variables from terraform output.
$json_config = Get-Content $config_file_name | Out-String | ConvertFrom-Json
$k8s_ip = $json_config.k8s_ip.value

Import-Module -Name Az.ApiManagement
$context = New-AzApiManagementContext -ResourceGroupName "$env:RG_NAME" -ServiceName "$env:SERVICE_NAME"

# Update Service URL
#Set-AzAPiManagementApi -Context $context -ApiId "$env:API_PATH" -ServiceUrl "https://$k8s_ip/$env:API_PATH"
Set-AzAPiManagementApi -Context $context -ApiId "$env:API_PATH$env:API_VERSION_SUFFIX" -ServiceUrl "https://$k8s_ip/$env:API_PATH"
