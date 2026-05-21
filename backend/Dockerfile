FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

COPY GateWise.Api/GateWise.Api.csproj ./GateWise.Api/
WORKDIR /app/GateWise.Api
RUN dotnet restore

WORKDIR /app

COPY GateWise.Api/. ./GateWise.Api/

WORKDIR /app/GateWise.Api
RUN dotnet publish -c Release -o /app/out

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/out .
ENV ASPNETCORE_URLS=http://+:5353
EXPOSE 5353

ENTRYPOINT ["dotnet", "GateWise.Api.dll"]

