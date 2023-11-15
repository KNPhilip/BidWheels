FROM mcr.microsoft.com/dotnet/sdk:7.0 as build
WORKDIR /app
EXPOSE 80

# Copy all .csproj and restore as distinct layers. Use of the same COPY command
# for every Dockerfile in the project to use Docker caching.
COPY BidWheels.sln BidWheels.sln
COPY src/AuctionService/AuctionService.csproj src/AuctionService/AuctionService.csproj
COPY src/SearchService/SearchService.csproj src/SearchService/SearchService.csproj
COPY src/GatewayService/GatewayService.csproj src/GatewayService/GatewayService.csproj
COPY src/IdentityService/IdentityService.csproj src/IdentityService/IdentityService.csproj

# Restore package deps
RUN dotnet restore BidWheels.sln

# Copy the app folders over
COPY src/AuctionService src/AuctionService
WORKDIR /app/src/AuctionService
RUN dotnet publish -c Release -o /app/src/out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build /app/src/out .
ENTRYPOINT [ "dotnet", "AuctionService.dll" ]