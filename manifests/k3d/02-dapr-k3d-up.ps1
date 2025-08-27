# Define variables
$clusterName = "mycluster"

# Create k3d cluster
Write-Host "Creating k3d cluster..."
k3d cluster create $clusterName --config ./k3d/mycluster-config.yaml

# Import images
Write-Host "Importing images into k3d cluster..."
k3d image import controlapi:latest -c $clusterName
k3d image import actorsapi:latest -c $clusterName

# List images in the cluster
Write-Host "Listing images in the cluster..."
docker exec k3d-$clusterName-server-0 crictl images

# Apply Redis configuration
Write-Host "Applying Redis configuration..."
kubectl apply -f ./redis/redis.yaml

# Install Dapr using Helm
Write-Host "Adding Dapr Helm repo and updating..."
helm repo add dapr https://dapr.github.io/helm-charts/
helm repo update

Write-Host "Installing Dapr..."
helm install dapr dapr/dapr --namespace dapr-system --version=1.16.0-rc.3 --create-namespace --wait

# Apply Dapr resources
Write-Host "Applying Dapr resources..."
kubectl apply -f ./dapr/pubsub.yaml
kubectl apply -f ./dapr/statestore.yaml
kubectl apply -f ./dapr/config.yaml

# Deploy test application
Write-Host "Deploying test application..."
kubectl apply -f ./testserver/testserver.yaml

# Deploy application
Write-Host "Deploying application..."
kubectl apply -f ./actorsapi/actorsapi.yaml

# Deploy debug tools
Write-Host "Deploying debug tools..."
kubectl apply -f ./tools/ubuntu.yaml
kubectl apply -f ./tools/powershell.yaml

Write-Host "Script execution completed."
