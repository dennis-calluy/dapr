## Kubernetes Deployment

### Application Startup

build container images files:
```
./01-dapr-k3d-build-images.ps1
```

k3d and app infrastructure
```
./02-dapr-k3d-up.ps1
```

cleanup
```
./03-dapr-k3d-down.ps1
```

### Application Interaction

[http requests file](./requests.http)

or:

Interact via loadbalancer service from host (PS: Invoke-WebRequest - curl alias), or from within k8s pod (replace "localhost" with "actorsapi"):
```
curl -Method POST "http://localhost:8080/setstate?numberState=1" -v
curl -Method POST "http://localhost:8080/getstate" -v
curl -Method POST "http://localhost:8080/registerreminder" -v
curl -Method POST "http://localhost:8080/unregisterreminder" -v
```