# Use the official .NET SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy the project file and restore dependencies
COPY src/Line10.Sales.Api/Line10.Sales.Api.csproj ./src/Line10.Sales.Api/
RUN dotnet restore ./src/Line10.Sales.Api/Line10.Sales.Api.csproj

# Copy the remaining files and build the application
COPY src/ ./src/
RUN dotnet publish ./src/Line10.Sales.Api/Line10.Sales.Api.csproj -c Release -o out

# Use the official .NET runtime image to run the application
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .

# Specify the entry point for the application
ENTRYPOINT ["dotnet", "Line10.Sales.Api.dll"]