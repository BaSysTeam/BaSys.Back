# BaSys.Back
Backend for BaSys application

# Docker
Start docker compose from current source code
```
docker-compose up --build --force-recreate
```
Start docker compose full (app + postgressql):
```
docker-compose -f docker-compose.full.yml up
```

Start docker compose app (postgressql on your localhost):
```
docker-compose -f docker-compose.local.yml up
```
