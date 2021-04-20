# This script creates an API Management Product, imports the API specification, and lastly configures the policies.
#
# ENV Required:
# - RG_NAME
# - SERVICE_NAME
# - PRODUCT_ID
# - PRODUCT_NAME
# - PRODUCT_DESCRIPTION
# - API_ID
# - API_VERSION
# - API_VERSION_SUFFIX
# - API_PATH
# - API_SPECIFICATION_PATH
# - API_VERSIONSET_ID
# - API_VERSIONSET_NAME
# - API_VERSIONSET_SCHEME
# - API_VERSIONSET_HEADER_NAME
# - API_VERSIONSET_QUERY_NAME
# - SOURCE_DIR

Import-Module -Name Az.ApiManagement
$context = New-AzApiManagementContext -ResourceGroupName "$env:RG_NAME" -ServiceName "$env:SERVICE_NAME"

# Create APIM product.
try
{
    Get-AzApiManagementProduct -Context $context -ProductId "$env:PRODUCT_ID" -ErrorAction SilentlyContinue
    if (!$?)
    {
        $tsandcs = "Free for all"
        $fileexists = Test-Path "$env:SOURCE_DIR/policies/termsandconditions.txt" -PathType Leaf
        if ($fileexists)
        {
            $tsandcs = Get-Content -Path "$env:SOURCE_DIR/policies/termsandconditions.txt" -Raw
        }

        New-AzApiManagementProduct -Context $context -ProductId "$env:PRODUCT_ID" -Title "$env:PRODUCT_NAME" -Description "$env:PRODUCT_DESCRIPTION" -LegalTerms $tsandcs -SubscriptionRequired $False -State "Published"

        Add-AzApiManagementProductToGroup -Context $context -GroupId "administrators" -ProductId "$env:PRODUCT_ID"
        Add-AzApiManagementProductToGroup -Context $context -GroupId "developers" -ProductId "$env:PRODUCT_ID"
        Add-AzApiManagementProductToGroup -Context $context -GroupId "guests" -ProductId "$env:PRODUCT_ID"
    }
    else
    {
        Write-Host "No need to create product!"
    }
}
catch
{
    Write-Host $_.Exception.Message
    throw "Could not create product!"
}

# Import API
try
{
    Get-AzApiManagementApiVersionSet -Context $context -ApiVersionSetId "$env:API_VERSIONSET_ID" -ErrorAction SilentlyContinue
    if (!$?)
    {
        New-AzApiManagementApiVersionSet -Context $context -ApiVersionSetId "$env:API_VERSIONSET_ID" -Name "$env:API_VERSIONSET_NAME" `
             -Scheme "$env:API_VERSIONSET_SCHEME" -HeaderName "$env:API_VERSIONSET_HEADER_NAME" -QueryName "$env:API_VERSIONSET_QUERY_NAME"
    }
    else
    {
        Set-AzApiManagementApiVersionSet -Context $context -ApiVersionSetId "$env:API_VERSIONSET_ID" -Name "$env:API_VERSIONSET_NAME" `
             -Scheme "$env:API_VERSIONSET_SCHEME" -HeaderName "$env:API_VERSIONSET_HEADER_NAME" -QueryName "$env:API_VERSIONSET_QUERY_NAME"
    }

    Write-Host "Import API"
    Import-AzApiManagementApi -Context $context -ApiId "$env:API_PATH$env:API_VERSION_SUFFIX" -SpecificationFormat "OpenApi" -Path "$env:API_PATH" `
         -SpecificationPath "$env:SOURCE_DIR/$env:API_SPECIFICATION_PATH" -ApiVersionSetId "$env:API_VERSIONSET_ID" `
         -Protocol @('http', 'https')

    Write-Host "Set API Version"
    $api = Get-AzApiManagementApi -Context $context -ApiId "$env:API_PATH$env:API_VERSION_SUFFIX"
    $api.ApiVersion = "$env:API_VERSION$env:API_VERSION_SUFFIX"
    Set-AzApiManagementApi -InputObject $api -PassThru

    Write-Host "Add API To Product"
    Add-AzApiManagementApiToProduct -Context $context -ProductId "$env:PRODUCT_ID" -ApiId "$env:API_PATH$env:API_VERSION_SUFFIX"
}
catch
{
    Write-Host $_.Exception.Message
    throw "Could not import API!"
}

# Apply Policies
try
{
    $fileexists = Test-Path "$env:SOURCE_DIR/policies/product-policies.xml" -PathType Leaf
    if ($fileexists)
    {
        Set-AzApiManagementPolicy -Context $context -ProductId "$env:PRODUCT_ID" -PolicyFilePath "$env:SOURCE_DIR/policies/product-policies.xml"
    }

    $fileexists = Test-Path "$env:SOURCE_DIR/policies/api-policies.xml" -PathType Leaf
    if ($fileexists)
    {
        Set-AzApiManagementPolicy -Context $context -ApiId "$env:API_PATH$env:API_VERSION_SUFFIX" -PolicyFilePath "$env:SOURCE_DIR/policies/api-policies.xml"
    }
}
catch
{
    Write-Host $_.Exception.Message
    throw "Could not Apply policies!"
}
