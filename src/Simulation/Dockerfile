# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build

WORKDIR /source

COPY . ./
RUN dotnet publish -c release --self-contained --runtime linux-x64 --framework netcoreapp5.0 -o /app

# final stage/image
FROM mcr.microsoft.com/dotnet/runtime:5.0
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "Simulation.dll"]
