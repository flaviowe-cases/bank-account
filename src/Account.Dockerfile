FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /App

# Copy everything
COPY . ./
# Restore as distinct layers
RUN dotnet restore
# Build and publish a release
RUN dotnet publish ./Bank.Accounts.Api  -o out 

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /App
COPY --from=build /App/out .
ENTRYPOINT ["dotnet", "Bank.Accounts.Api.dll"]