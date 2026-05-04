FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 5091

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["TutoringAcademy.csproj", "./"]
RUN dotnet restore "TutoringAcademy.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "TutoringAcademy.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TutoringAcademy.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://0.0.0.0:5091
ENTRYPOINT ["dotnet", "TutoringAcademy.dll"]