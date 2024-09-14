FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# install python
RUN apt-get update && \
    apt-get install -y python3 && \
    rm -rf /var/lib/apt/lists/*
ENV PATH="/usr/bin/python3:${PATH}"

#COPY ["GUI/Directory.Build.props", "GUI/"]
COPY ["GUI/GUI.Browser/GUI.Browser.csproj", "GUI/GUI.Browser/"]
#COPY ["GUI/GUI/GUI.csproj", "GUI/GUI/"]

RUN dotnet workload install wasm-tools
RUN dotnet restore "./GUI/GUI.Browser/./GUI.Browser.csproj"
COPY . .
WORKDIR "/src/GUI/GUI.Browser"
RUN dotnet build "./GUI.Browser.csproj" -c $BUILD_CONFIGURATION -o /app/build

RUN ls -la /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./GUI.Browser.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

RUN ls -la /app/publish

# Stage 2: Serve the application with Nginx
FROM nginx:alpine

# Remove default nginx website
RUN rm -rf /usr/share/nginx/html/*

# Copy the built application into Nginx's html directory
COPY --from=publish /app/publish/wwwroot /usr/share/nginx/html

# Expose port 80
EXPOSE 80

# Start nginx
CMD ["nginx", "-g", "daemon off;"]