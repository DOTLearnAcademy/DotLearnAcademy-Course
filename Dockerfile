FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["DotLearn.Course/DotLearn.Course.csproj", "DotLearn.Course/"]
RUN dotnet restore "DotLearn.Course/DotLearn.Course.csproj"
COPY . .
WORKDIR "/src/DotLearn.Course"
RUN dotnet publish -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "DotLearn.Course.dll"]
