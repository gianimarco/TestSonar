current-context: deployment
apiVersion: v1
clusters:
  - cluster:
      certificate-authority-data: ${cluster_ca}
      server: ${cluster_host}
    name: deployment-cluster
contexts:
  - context:
      cluster: deployment-cluster
      user: deployment-user
    name: deployment
kind: Config
users:
  - name: deployment-user
    user:
      client-certificate-data: ${client_certificate}
      client-key-data: ${client_key}
