#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY [".", "MockAuthProvider/"]
RUN dotnet restore "MockAuthProvider/MockAuthProvider/MockAuthProvider.csproj"
COPY . .
WORKDIR "/src/MockAuthProvider"
RUN dotnet build "MockAuthProvider/MockAuthProvider.csproj" -c Release -o /app/build
RUN dotnet dev-certs https

FROM build AS publish
RUN dotnet publish "MockAuthProvider/MockAuthProvider.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=build /root/.dotnet/corefx/cryptography/x509stores/my/* /root/.dotnet/corefx/cryptography/x509stores/my/

ENTRYPOINT ["dotnet", "MockAuthProvider.dll"]