# BaSYS
BaSYS - Business Applications System is a Low-Code constructor for creating applications for business automation.

# Disclaimer
The current pre-alpha version is not recommended for production use. We recommend using it for testing and evaluation purposes only.

# Docs

[English documentation](https://basysteam.github.io/BaSys.Docs/en/)
[Документация на русском](https://basysteam.github.io/BaSys.Docs/ru/)

# Getting Started

To install the **BaSYS** application, you need Docker and one of the supported database systems:
- PostgreSQL
- MS SQL

> **Note:** Currently, most testing is conducted on PostgreSQL. Therefore, it is recommended to use this database for evaluation purposes.

## Creating a `docker-compose.yml` File

Create a `docker-compose.yml` file using the following template:

```yml
version: '3'

services:
  basys:
    container_name: basys.app
    image: basysteam/basys:latest
    restart: always
    ports:
      - 13080:8080
    environment:
      # CurrentApp
      InitAppSettings:CurrentApp:Id: main
      InitAppSettings:CurrentApp:Title: Main
      # Sa. Parameters for creating the system database.
      # Super administrator login.
      InitAppSettings:Sa:Login: <super_admin_login>
      # Super administrator password.
      InitAppSettings:Sa:Password: <super_admin_password>
      # Database type: 0 - MS SQL, 1 - PG SQL.
      InitAppSettings:Sa:DbKind: 1
      # Connection string for the system database.
      InitAppSettings:Sa:ConnectionString: <super_admin_base_connections_string>
      # MainDb. Parameters for creating the working database.
      # Database name (can contain lowercase Latin letters, numbers, and underscores)
      InitAppSettings:MainDb:Name: <db_name>
      # Database type: 0 - MS SQL, 1 - PG SQL
      InitAppSettings:MainDb:DbKind: 1
      # Connection string for the working database.
      InitAppSettings:MainDb:ConnectionString: <work_base_connection_string>
      # Administrator login.
      InitAppSettings:MainDb:AdminLogin: <admin_login>
      # Administrator password.
      InitAppSettings:MainDb:AdminPassword: <admin_password>
      # Default language for the administrator: ru | en
      InitAppSettings:MainDb:Culture: <admin_culture>
    extra_hosts:
      - "host.docker.internal:host-gateway"
```

## Example of docker-compose.yml

Here is an example configuration for a local installation of BaSYS using PostgreSQL. Note that the server name host.docker.internal corresponds to the locally installed database. Be sure to replace \<your_sql_server_password\> with your database password.

```yml
version: '3'

services:
  basys:
    container_name: basys.app
    image: basysteam/basys:latest
    restart: always
    ports:
      - 13080:8080
    environment:
      # CurrentApp
      InitAppSettings:CurrentApp:Id: main
      InitAppSettings:CurrentApp:Title: Main
      # Sa. Parameters for creating the system database.
      # Super administrator login.
      InitAppSettings:Sa:Login: sa@mydomain.com
      # Super administrator password.
      InitAppSettings:Sa:Password: 111111
      # Database type: 0 - MS SQL, 1 - PG SQL.
      InitAppSettings:Sa:DbKind: 1
      # Connection string for the system database.
      InitAppSettings:Sa:ConnectionString: Server=host.docker.internal;Port=5432;Database=basys_system;User ID=postgres;Password=<your_sql_server_password>;Timeout=60;
      # MainDb. Parameters for creating the working database.
      # Database name (can contain lowercase Latin letters, numbers, and underscores)
      InitAppSettings:MainDb:Name: basys_work_1
      # Database type: 0 - MS SQL, 1 - PG SQL
      InitAppSettings:MainDb:DbKind: 1
      # Connection string for the working database.
      InitAppSettings:MainDb:ConnectionString: Server=host.docker.internal;Port=5432;Database=basys_work_1;User ID=postgres;Password=<your_sql_server_password>;Timeout=60;
      # Administrator login.
      InitAppSettings:MainDb:AdminLogin: admin@mydomain.com
      # Administrator password.
      InitAppSettings:MainDb:AdminPassword: 111111
      # Default language for the administrator: ru | en
      InitAppSettings:MainDb:Culture: ru
    extra_hosts:
      - "host.docker.internal:host-gateway"

```

## Running the Application

1. Navigate to the folder containing the created docker-compose.yml file.
2. Execute the following command:

```
docker-compose up -d
```

After running the command, the BaSYS application will be available in your browser at http://localhost:13080/.


