variable "cluster_name" {}
variable "cluster_rg" {}
variable "ip_name" {}
variable "ip_rg" {}

# Azure Provider
provider "azurerm" {
  version = "=2.2.0"
  features {}
}

# Get public IP
data "azurerm_public_ip" "ip" {
  name                = var.ip_name
  resource_group_name = var.ip_rg
}

# Get existing cluster details
data "azurerm_kubernetes_cluster" "aks" {
  name                = var.cluster_name
  resource_group_name = var.cluster_rg
}

# Create Template
data "template_file" "config-template" {
  template = file("kube-config.tpl")
  vars = {
    cluster_host = data.azurerm_kubernetes_cluster.aks.kube_config.0.host
    cluster_ca = data.azurerm_kubernetes_cluster.aks.kube_config.0.cluster_ca_certificate
    client_certificate = data.azurerm_kubernetes_cluster.aks.kube_config.0.client_certificate
    client_key = data.azurerm_kubernetes_cluster.aks.kube_config.0.client_key
  }
}

# Save Config
resource "local_file" "kube-config" {
  filename = "kube-config.yml"
  content = data.template_file.config-template.rendered
}
