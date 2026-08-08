FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["PACLEB_08102026.csproj", "."]
RUN dotnet restore "./PACLEB_08102026.csproj"

COPY . .
RUN dotnet publish "./PACLEB_08102026.csproj" \
    -c Release \
    -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "PACLEB_08102026.dll"]