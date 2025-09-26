# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY ["FCG.MS.Payments.API/FCG.MS.Payments.API.csproj", "FCG.MS.Payments.API/"]
COPY ["FCG.MS.Payments.Application/FCG.MS.Payments.Application.csproj", "FCG.MS.Payments.Application/"]
COPY ["FCG.MS.Payments.Domain/FCG.MS.Payments.Domain.csproj", "FCG.MS.Payments.Domain/"]
COPY ["FCG.MS.Payments.Infrastructure/FCG.MS.Payments.Infrastructure.csproj", "FCG.MS.Payments.Infrastructure/"]

# Restore packages
RUN dotnet restore "FCG.MS.Payments.API/FCG.MS.Payments.API.csproj"

# Copy the rest of the code
COPY . .

# Build the project
RUN dotnet build "FCG.MS.Payments.API/FCG.MS.Payments.API.csproj" -c Release

# Build and publish
FROM build AS publish
RUN dotnet publish "FCG.MS.Payments.API/FCG.MS.Payments.API.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Set environment variables
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:80

# Copy the published app
COPY --from=publish /app/publish .

# Create a non-root user
RUN adduser --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

# Expose port 80
EXPOSE 80

# Set the entry point
ENTRYPOINT ["dotnet", "FCG.MS.Payments.API.dll"]
