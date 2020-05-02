#!/bin/sh
dotnet /app/IdentityServer.Migrations.dll -cs "$ConnectionString" && echo 'schema' > /var/output/migrator.txt