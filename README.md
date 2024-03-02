# Clean Architecture With Blazor Server

[![Build](https://github.com/neozhu/CleanArchitectureWithBlazorServer/actions/workflows/dotnet.yml/badge.svg)](https://github.com/neozhu/CleanArchitectureWithBlazorServer/actions/workflows/dotnet.yml)
[![CodeQL](https://github.com/neozhu/CleanArchitectureWithBlazorServer/actions/workflows/codeql-analysis.yml/badge.svg)](https://github.com/neozhu/CleanArchitectureWithBlazorServer/actions/workflows/codeql-analysis.yml)
[![Nuget](https://img.shields.io/nuget/v/CleanArchitecture.Blazor.Solution.Template?label=NuGet)](https://www.nuget.org/packages/CleanArchitecture.Blazor.Solution.Template)
[![Docker Image CI](https://github.com/neozhu/CleanArchitectureWithBlazorServer/actions/workflows/docker-image.yml/badge.svg)](https://github.com/neozhu/CleanArchitectureWithBlazorServer/actions/workflows/docker-image.yml)
[![Nuget](https://img.shields.io/nuget/dt/CleanArchitecture.Blazor.Solution.Template?label=Downloads)](https://www.nuget.org/packages/CleanArchitecture.Blazor.Solution.Template)

This is a repository for creating a Blazor Server application following the principles of Clean Architecture. It has a
nice user interface, and an efficient code generator that allows you to quickly build amazing web application with .net
Blazor technology.

## Live Demo

- Blazor Server mode:https://architecture.blazorserver.com/

## Screenshots and video

[![Everything Is AWESOME](doc/page.png)](https://www.youtube.com/embed/GyZJl_dG-Pg "Everything Is AWESOME")

## Development Environment

- Microsoft Visual Studio Community 2022 (64-bit)
- Docker
- .NET 8.0
- Unit Test

## Docker Container
```bash
# Docker Pull Command
docker pull dockerksjscn/cleanarchitectureblazorserver:latest
```

```bash
# Docker Run
# default container port:8080

# default without database
docker run -p 8080:8080 -e UseInMemoryDatabase=true -e ASPNETCORE_ENVIRONMENT=Development dockerksjscn/cleanarchitectureblazorserver:latest

# set database connection
# set SMPT Server
docker run -d -p 8080:8080 -e UseInMemoryDatabase=false \
-e ASPNETCORE_ENVIRONMENT=Development \
-e DatabaseSettings__DBProvider=mssql \
-e DatabaseSettings__ConnectionString=Server=10.33.1.xxx;Database=BlazorDashboardDb;User Id=sa;Password=***;MultipleActiveResultSets=true;Encrypt=false;TrustServerCertificate=false \
-e SmtpClientOptions__User=*** \
-e SmtpClientOptions__Port=25 \
-e SmtpClientOptions__Server=*** \
-e SmtpClientOptions__Password=*** \
dockerksjscn/cleanarchitectureblazorserver:latest
```
## docker-compose.yml

```bash
version: '3.8'
services:
  blazorserverapp:
    image: dockerksjscn/cleanarchitectureblazorserver:latest
    user: "1000:1000"
    environment:
      - UseInMemoryDatabase=false
      - ASPNETCORE_ENVIRONMENT=Development
      - DatabaseSettings__DBProvider=mssql
      - DatabaseSettings__ConnectionString=Server=10.33.1.xx;Database=BlazorDashboardDb;User Id=sa;Password=***;MultipleActiveResultSets=true;Encrypt=false;TrustServerCertificate=false
      - SmtpClientOptions__User=***
      - SmtpClientOptions__Port=25
      - SmtpClientOptions__Server=***
      - SmtpClientOptions__Password=***
    ports:
      - "8014:8080"
    volumes:
      - files_volume:/app/Files

volumes:
  files_volume:
```

To run a Docker Compose setup as defined in your docker-compose.yaml file and ensure the Docker volume files_volume is accessible with read and write permissions by a user group with the ID 1000, follow these steps:

```bash
# Change Ownership: Use the chown command to change the owner and/or group of the volume directory. If you want the user group with group ID 1000 to own the directory, you can execute the following command:
sudo chown -R :1000 /var/lib/docker/volumes/blazorserverapp_files_volume
# Set Read and Write Permissions: Use the chmod command to change the permissions of files and directories. To grant read and write permissions to the group members, execute:
sudo chmod -R g+rw /var/lib/docker/volumes/blazorserverapp_files_volume
```

![image](https://user-images.githubusercontent.com/1549611/183799080-380e1f01-ef80-4568-80d2-517514aa59e5.png)

## Supported Databases

* PostgreSQL (Provider Name: `postgresql`)
* Microsoft SQL Server (Provider Name: `mssql`)
* SQLite (Provider Name: `sqlite`)

### How to select a specific Database?

1. Open the `appsettings.json` file located in the src directory of the `Server.UI` project.
2. Change the setting `DBProvider` to the desired provider name (See Supported Databases).
3. Change the `ConnectionString` to a connection string, which works for your selected database provider.

## Docker compose https deployment

- Create self-signed development certificates for the project
  -
  cmd: `dotnet dev-certs https -ep $env:USERPROFILE\.aspnet\https\CleanArchitecture.Blazor.Server.UI.pfx -p Password@123`
    - cmd: `dotnet dev-certs https --trust`
- Manage User secrets to save the password
    - cmd: `dotnet user-secrets init`
    - cmd: `dotnet user-secrets -p Server.UI.csproj set "Kestrel:Certificates:Development:Password" "Password@123"`

## Code Generator Extension for visual studio.net 2022

<div><video controls src="https://user-images.githubusercontent.com/1549611/197116874-f28414ca-7fc1-463a-b887-0754a5bb3e01.mp4" muted="false"></video></div>

### Install CleanArchitecture CodeGenerator For Blazor App

- Open Manage Extensions Search with `CleanArchitecture CodeGenerator For Blaozr App`
  ![image](https://github.com/neozhu/CleanArchitectureWithBlazorServer/assets/1549611/555d274c-8f62-438b-ac7a-6d327e4c23c8)
- Download to Install

### CleanArchitecture CodeGenerator For Blazor App Repo

- https://github.com/neozhu/CleanArchitectureCodeGenerator
- The code generator can automatically generate the standard code
    - Application Layer Code

        - ![image](https://user-images.githubusercontent.com/1549611/181414766-84850a90-3a21-47ed-afcf-1b5cdd602edf.png)
    - Domain Event

        - ![image](https://user-images.githubusercontent.com/1549611/183537303-058d6f49-fc45-4b77-8924-cc2e8266cad7.png)
    - Blazor UI Layer Code

        - ![image](https://user-images.githubusercontent.com/1549611/181414818-5c1c2dfc-5560-4ab2-8773-dc7c816730d4.png)
    - Task List

        - ![image](https://user-images.githubusercontent.com/1549611/183537444-3d1b2980-b131-4e9d-bfe1-7b475f760b57.png)

## How to install solution templates

- install the project template
    - run CLI: `dotnet new install ./`
    - run CLI: `dotnet new list`

<img width="828" alt="image" src="https://github.com/neozhu/CleanArchitectureWithBlazorServer/assets/1549611/f23022e0-3fd6-475a-96ab-84b0d3328e4c">

- create a solution with the template
    - run CLI: `dotnet new ca-blazorserver-sln` or `dotnet new ca-blazorserver-sln -n NewProjectName(root namesapces)`


- build a project template with nuget.exe
    - run CLI: `.\nuget.exe add -Source .\ CleanArchitecture.Blazor.Solution.Template.1.0.0-preview.1.nupkg`
      <img width="1329" alt="image" src="https://github.com/neozhu/CleanArchitectureWithBlazorServer/assets/1549611/7c56dec5-5935-4fc5-ad81-8bcb65e9d0dc">
    - create a new project from Clean Architecture for Blazor Server Solution
      <img width="769" alt="image" src="https://github.com/neozhu/CleanArchitectureWithBlazorServer/assets/1549611/ed7eb20f-aec2-4f69-95b7-d47c2eb20428">

## Why I chose Blazor Server

- I hate switching between C# and JavaScript at the same time in order to develop a project, which is why I opted for
  Blazor Server.

## Characteristic

- Avoid repeating work
- Focus on story implementation
- Integration Hangfire dashboard
- Implementation OCR image recognition

  ![image](https://user-images.githubusercontent.com/1549611/185576711-31ab3081-ba22-43f3-b837-c8f1de981442.png)
- org chart

  ![image](doc/orgchart.png)

## About

Coming up.

## License

MIT License
