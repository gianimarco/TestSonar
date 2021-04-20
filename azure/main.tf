###############################################################################
# IMPORTANT
# =========
# This will not work yet. See the following link for more information:
# https://github.com/terraform-providers/terraform-provider-azurerm/issues/3203
###############################################################################
variable "api_name" {}
variable "api_display_name" {}
variable "api_path" {}
variable "product_name" {}
variable "product_description" {}
variable "product_id" {}
variable "product_terms" {}
variable "version_name" {}
variable "version_display_name" {}

# Azure Provider
provider "azurerm" {
  version = "=1.44.0"
}

# Default to using a storage account to hold state. This will make it easier to work as a team.
terraform {
  backend "azurerm" {
    resource_group_name  = "state-storage"
    storage_account_name = "tfstoragetest123"
    container_name       = "tfstate"
    key                  = "${var.api_name}-azure.terraform.tfstate"
  }
}

locals {
  apim_name = "${var.client_name}-APIM-${var.location}-${var.environment}-${var.base_name}"
  rg_name   = "${var.client_name}-RSG-${var.location}-${var.environment}-${var.base_name}"
}

# Get the APIM instance
data "azurerm_api_management" "apim" {
  name                = local.apim_name
  resource_group_name = local.rg_name
}

# APIM Product
resource "azurerm_api_management_product" "product" {
  api_management_name   = data.azurerm_api_management.apim.name
  display_name          = var.product_name
  description           = var.product_description
  terms                 = var.product_terms
  product_id            = var.product_id
  published             = true
  resource_group_name   = local.rg_name
  subscription_required = false
}

# APIM Product Groups
data "azurerm_api_management_group" "admins" {
  api_management_name = local.apim_name
  name                = "administrators"
  resource_group_name = local.rg_name
}
resource "azurerm_api_management_product_group" "admins" {
  api_management_name = local.apim_name
  group_name          = data.azurerm_api_management_group.admins.name
  product_id          = var.product_id
  resource_group_name = local.rg_name
}

data "azurerm_api_management_group" "devs" {
  api_management_name = local.apim_name
  name                = "developers"
  resource_group_name = local.rg_name
}
resource "azurerm_api_management_product_group" "admins" {
  api_management_name = local.apim_name
  group_name          = data.azurerm_api_management_group.devs.name
  product_id          = var.product_id
  resource_group_name = local.rg_name
}

data "azurerm_api_management_group" "guests" {
  api_management_name = local.apim_name
  name                = "guests"
  resource_group_name = local.rg_name
}
resource "azurerm_api_management_product_group" "admins" {
  api_management_name = local.apim_name
  group_name          = data.azurerm_api_management_group.guests.name
  product_id          = var.product_id
  resource_group_name = local.rg_name
}

# API Version Set
resource "azurerm_api_management_api_version_set" "version" {
  api_management_name = local.apim_name
  display_name        = var.version_display_name
  name                = var.version_name
  resource_group_name = local.rg_name
  versioning_scheme   = "Segment"
}

# API
resource "azurerm_api_management_api" "api" {
  api_management_name = local.apim_name
  display_name        = var.api_display_name
  name                = var.api_name
  path                = var.api_path
  protocols           = ["http", "https"]
  resource_group_name = local.rg_name
  revision            = "1"

  import {
    content_format = "openapi-yaml"
    content_value  = file("src/api-specification.yml")
  }
}
