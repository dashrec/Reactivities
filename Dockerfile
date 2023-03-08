FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
WORKDIR /app
EXPOSE 8080

# copy .csproj and restore as distinct layers
COPY "Reactivities.sln" "Reactivities.sln"
COPY "API/API.csproj" "API/API.csproj"
COPY "Application/Application.csproj" "Application/Application.csproj"
COPY "Persistence/Persistence.csproj" "Persistence/Persistence.csproj"
COPY "Domain/Domain.csproj" "Domain/Domain.csproj"
COPY "Infrastructure/Infrastructure.csproj" "Infrastructure/Infrastructure.csproj"

RUN dotnet restore "Reactivities.sln" 

# copy everything else and build
COPY . .
WORKDIR /app
RUN dotnet publish -c Release -o out

# build a runtime image
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT [ "dotnet", "API.dll" ]


# WORKDIR /app location in docker container 
# EXPOSE 8080 If you don't do this, then the fly I o health checks will not be able to get into our app to see if s running correctly and will not deploy 
# COPY "Reactivities.sln" "Reactivities.sln"  it takes what we have inside our filesystem and copy into docker container
# restpre everything inside  "Reactivities.sln" in WORKDIR /app folder and it is gonna execute all of those copyed files
# So we'll say dotnet publish to create a build version of our dot net application inside aoo/out folder
# start up project name API.dll