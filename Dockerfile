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

# Install the agent
RUN apt-get update && apt-get install -y wget ca-certificates gnupg \
&& echo 'deb http://apt.newrelic.com/debian/ newrelic non-free' | tee /etc/apt/sources.list.d/newrelic.list \
&& wget https://download.newrelic.com/548C16BF.gpg \
&& apt-key add 548C16BF.gpg \
&& apt-get update \
&& apt-get install -y 'newrelic-dotnet-agent' \
&& rm -rf /var/lib/apt/lists/*

# Enable the agent
ENV CORECLR_ENABLE_PROFILING=1 \
CORECLR_PROFILER={36032161-FFC0-4B61-B559-F6C5D41BAE5A} \
CORECLR_NEWRELIC_HOME=/usr/local/newrelic-dotnet-agent \
CORECLR_PROFILER_PATH=/usr/local/newrelic-dotnet-agent/libNewRelicProfiler.so \
NEW_RELIC_LICENSE_KEY=7684869377f4b5d6e688fcc15ad9abd7FFFFNRAL \
NEW_RELIC_APP_NAME="fcg-ms-user-logs" \
NEW_RELIC_APPLICATION_LOGGING_ENABLED=true \
NEW_RELIC_APPLICATION_LOGGING_FORWARDING_ENABLED=true \
NEW_RELIC_APPLICATION_LOGGING_FORWARDING_MAX_SAMPLES_STORED=10000 \
NEW_RELIC_APPLICATION_LOGGING_LOCAL_DECORATING_ENABLED=false 

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
