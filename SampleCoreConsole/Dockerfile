FROM mcr.microsoft.com/dotnet/core/runtime:2.2-stretch-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["SampleCoreConsole/SampleCoreConsole.csproj", "SampleCoreConsole/"]
COPY ["Tharga.Toolkit.Console/Tharga.Toolkit.Console.csproj", "Tharga.Toolkit.Console/"]
RUN dotnet restore "SampleCoreConsole/SampleCoreConsole.csproj"
COPY . .
WORKDIR "/src/SampleCoreConsole"
RUN dotnet build "SampleCoreConsole.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "SampleCoreConsole.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "SampleCoreConsole.dll"]