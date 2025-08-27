# Create k3d cluster
Write-Host "Building container images files ..."
docker build --no-cache -t actorsapi:latest  -f ../../src/actorsapi/Dockerfile ../..