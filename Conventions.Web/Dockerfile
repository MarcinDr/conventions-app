FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mhart/alpine-node:14 AS node
WORKDIR Conventions.Web/ClientApp
RUN npm build
WORKDIR app
COPY /Conventions.Web/ClientApp/build /app/build

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Conventions.Web/Conventions.Web.csproj", "Conventions.Web/"]
RUN dotnet restore "Conventions.Web/Conventions.Web.csproj"
COPY . .

WORKDIR "/src/Conventions.Web" 
RUN dotnet build "Conventions.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Conventions.Web.csproj" -c Release -o /app/publish


FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=node /app .

# in 'real' life these should be provided from outside of this file
ENV ASPNETCORE_ENVIRONMENT "local"
ENV SERVICE_NAME "conventions"
ENV OAUTH_CLIENT_ID ""
ENV OAUTH_CLIENT_SECRET ""
ENV OAUTH_ALLOWED_SCOPES "offline_access openid venues.admin venues.read events.admin events.read talks.admin talks.read attendance.admin"
ENV OAUTH_AUTHORITY_URL "dev-xhrc91y0.eu.auth0.com"

ENTRYPOINT ["dotnet", "Conventions.Web.dll"]
