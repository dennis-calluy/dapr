# Define variables
$clusterName = "mycluster"

# Create k3d cluster
Write-Host "Deleting k3d cluster..."
k3d cluster delete $clusterName