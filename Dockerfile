FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["STApp/STApp.csproj", "STApp/"]
COPY ["STApp.Client/STApp.Client.csproj", "STApp.Client/"]
RUN dotnet restore "STApp/STApp.csproj"

# Copy the rest of the files and build
COPY . .
RUN dotnet build "STApp/STApp.csproj" -c Release -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish "STApp/STApp.csproj" -c Release -o /app/publish

# Final stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 80
EXPOSE 443
ENTRYPOINT ["dotnet", "STApp.dll"]