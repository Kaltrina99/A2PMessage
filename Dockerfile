# Use the official .NET 9 SDK image for build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj and restore as distinct layers
COPY ["MessagingQueueApp/MessagingQueueApp.csproj", "MessagingQueueApp/"]
RUN dotnet restore "MessagingQueueApp/MessagingQueueApp.csproj"

# Copy the rest of the source code
COPY . .

WORKDIR "/src/MessagingQueueApp"
RUN dotnet publish "MessagingQueueApp.csproj" -c Release -o /app/publish

# Use the official .NET 9 runtime image for the app
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Expose port 8080 for the app
EXPOSE 8080

# Set environment variable for ASP.NET Core to listen on port 8080
ENV ASPNETCORE_URLS=http://+:8080

# Start the application
ENTRYPOINT ["dotnet", "MessagingQueueApp.dll"]