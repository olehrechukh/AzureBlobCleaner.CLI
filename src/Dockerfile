FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["BlobCleaner/BlobCleaner.csproj", "BlobCleaner/"]
RUN dotnet restore "BlobCleaner/BlobCleaner.csproj"
COPY . .
WORKDIR "/src/BlobCleaner"
RUN dotnet build "BlobCleaner.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BlobCleaner.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BlobCleaner.dll"]
