#For additional help and examples of .NET Dockerfiles go here: https://go.microsoft.com/fwlink/?linkid=867640
FROM microsoft/aspnetcore-build:FrameworkVersion AS build
WORKDIR /src
COPY Company.WebApplication1.csproj .
RUN dotnet restore
COPY . .
RUN dotnet build -c Release -o /app

FROM build AS publish
RUN dotnet publish -c Release -o /app

FROM microsoft/aspnetcore:FrameworkVersion AS final
WORKDIR /app
EXPOSE 80
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Company.WebApplication1.dll"]
