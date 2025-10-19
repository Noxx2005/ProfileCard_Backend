# Stage 1: Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy csproj and restore dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy all source code and build
COPY . ./
RUN dotnet publish -c Release -o out

# Stage 2: Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/out ./

# Expose port 8080 (Railway automatically maps it)
EXPOSE 8080

# Start the API
ENTRYPOINT ["dotnet", "ProfileApi.dll"]
