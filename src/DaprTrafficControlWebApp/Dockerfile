FROM mcr.microsoft.com/dotnet/sdk:5.0 As build-env
WORKDIR /app

COPY *.csproj ./
RUN dotnet restore

COPY . ./
RUN dotnet publish -c Release -o output

FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY --from=build-env /app/output .
ENTRYPOINT ["dotnet", "DaprTrafficControlWebApp.dll"]