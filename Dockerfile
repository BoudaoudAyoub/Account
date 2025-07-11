FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /src
COPY ./src . 

WORKDIR "/src/Account.API/"
RUN dotnet publish --configuration Release --output /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final

USER root
WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Account.API.dll"]