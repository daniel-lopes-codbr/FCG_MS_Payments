FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["FCG_MS_Payments.Api/FCG_MS_Payments.Api.csproj", "FCG_MS_Payments.Api/"]
COPY ["FCG_MS_Payments.Application/FCG_MS_Payments.Application.csproj", "FCG_MS_Payments.Application/"]
COPY ["FCG_MS_Payments.Domain/FCG_MS_Payments.Domain.csproj", "FCG_MS_Payments.Domain/"]
COPY ["FCG_MS_Payments.Infrastructure/FCG_MS_Payments.Infrastructure.csproj", "FCG_MS_Payments.Infrastructure/"]
RUN dotnet restore "FCG_MS_Payments.Api/FCG_MS_Payments.Api.csproj"
COPY . .
WORKDIR "/src/FCG_MS_Payments.Api"
RUN dotnet build "FCG_MS_Payments.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FCG_MS_Payments.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FCG_MS_Payments.Api.dll"] 