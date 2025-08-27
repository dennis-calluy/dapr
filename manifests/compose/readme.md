## Docker Compose Deployment

### Application Startup (debug)

```
docker compose -f .\docker-compose-debug.yml build --no-cache
docker compose -f .\docker-compose-debug.yml up -d

docker compose -f .\docker-compose-debug.yml down
```
After compose up, debug the connect.actors.api project in visual studio


### Application Startup

```
docker compose build --no-cache
docker compose up

docker compose down
```

### Application Interaction

[http requests file](./requests.http)

or:

```
curl -Method POST "http://localhost:8080/setstate?numberState=1" -v
curl -Method POST "http://localhost:8080/getstate" -v
curl -Method POST "http://localhost:8080/registerreminder" -v
curl -Method POST "http://localhost:8080/unregisterreminder" -v
```