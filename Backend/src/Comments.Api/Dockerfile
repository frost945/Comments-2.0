FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

COPY ["src/Comments.csproj", "./"]
RUN dotnet restore
 
COPY src/. .
RUN dotnet publish Comments.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/publish/ .

EXPOSE 5000

ENTRYPOINT ["dotnet", "Comments.dll"]
