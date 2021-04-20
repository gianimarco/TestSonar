variable "client_name" {
  description = "An abbreviation of the client/company name that is used."
  default     = "DT"
}
variable "location" {
  description = "The region to deploy the resources in."
  default     = "southafricanorth"
}
variable "environment" {
  description = "The environment that the resources are deployed for."
  default     = "dev"
}
variable "base_name" {
  description = "The base name to use for all resources."
  default     = ""
}
