FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY . .
RUN dotnet restore ./IdentityServer.Migrations
RUN dotnet publish ./IdentityServer.Migrations -c Release -o out --no-restore

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-alpine
WORKDIR /app
COPY --from=build-env /app/out .
COPY ./IdentityServer.Migrations/docker-entrypoint.sh /docker-entrypoint.sh

ENV DOTNET_RUNNING_IN_CONTAINER=true
ENTRYPOINT ["sh"]
CMD ["docker-entrypoint.sh"]

ENV DOCKERIZE_VERSION v0.6.1
RUN wget https://github.com/jwilder/dockerize/releases/download/$DOCKERIZE_VERSION/dockerize-alpine-linux-amd64-$DOCKERIZE_VERSION.tar.gz \
    && tar -C /usr/local/bin -xzvf dockerize-alpine-linux-amd64-$DOCKERIZE_VERSION.tar.gz \
    && rm dockerize-alpine-linux-amd64-$DOCKERIZE_VERSION.tar.gz