FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["src/GymDogs.Presentation/GymDogs.Presentation.csproj", "src/GymDogs.Presentation/"]
COPY ["src/GymDogs.Application/GymDogs.Application.csproj", "src/GymDogs.Application/"]
COPY ["src/GymDogs.Infrastructure/GymDogs.Infrastructure.csproj", "src/GymDogs.Infrastructure/"]
COPY ["src/GymDogs.Domain/GymDogs.Domain.csproj", "src/GymDogs.Domain/"]

RUN dotnet restore "src/GymDogs.Presentation/GymDogs.Presentation.csproj"

COPY . .
WORKDIR "/src/src/GymDogs.Presentation"
RUN dotnet build "GymDogs.Presentation.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GymDogs.Presentation.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Install dotnet ef tool in publish stage (has SDK)
RUN dotnet tool install --global dotnet-ef --version 10.0.0

FROM base AS final
WORKDIR /app

# Copy dotnet tools from publish stage
COPY --from=publish /root/.dotnet/tools /root/.dotnet/tools
ENV PATH="${PATH}:/root/.dotnet/tools"

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GymDogs.Presentation.dll"]
