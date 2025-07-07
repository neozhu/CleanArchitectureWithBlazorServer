# Clean Architecture With Blazor Server

[![Build](https://github.com/neozhu/CleanArchitectureWithBlazorServer/actions/workflows/dotnet.yml/badge.svg)](https://github.com/neozhu/CleanArchitectureWithBlazorServer/actions/workflows/dotnet.yml)
[![CodeQL](https://github.com/neozhu/CleanArchitectureWithBlazorServer/actions/workflows/codeql-analysis.yml/badge.svg)](https://github.com/neozhu/CleanArchitectureWithBlazorServer/actions/workflows/codeql-analysis.yml)
[![Nuget](https://img.shields.io/nuget/v/CleanArchitecture.Blazor.Solution.Template?label=NuGet)](https://www.nuget.org/packages/CleanArchitecture.Blazor.Solution.Template)
[![Docker Image CI](https://github.com/neozhu/CleanArchitectureWithBlazorServer/actions/workflows/docker-image.yml/badge.svg)](https://github.com/neozhu/CleanArchitectureWithBlazorServer/actions/workflows/docker-image.yml)
[![Nuget](https://img.shields.io/nuget/dt/CleanArchitecture.Blazor.Solution.Template?label=Downloads)](https://www.nuget.org/packages/CleanArchitecture.Blazor.Solution.Template)

This repository hosts a Blazor Server application designed using Clean Architecture principles, featuring a sophisticated user interface and an efficient code generator. The application is built on .NET 9, which brings enhanced performance, improved developer productivity, and new features that make the development process smoother. Blazor Server, now supported on .NET 9, combines the power of C# with a modern web development experience, eliminating the need to switch between languages like JavaScript and C#. This setup simplifies development and enables fast, responsive, and highly interactive web applications. Leveraging Blazor’s real-time communication capabilities and .NET’s robust ecosystem, developers can rapidly create feature-rich, scalable applications with a seamless user experience.


## Explore the Live Demo

Explore the application's features and design through screenshots and a video walkthrough.
[![Everything Is AWESOME](doc/blazorstudio.png)](https://www.youtube.com/watch?v=hCsHSNAs-70 "Everything Is AWESOME")

Experience the application in action in Blazor Server mode by visiting:
- MS SQL Database: [architecture.blazorserver.com](https://architecture.blazorserver.com/)


### Additional Note:
If you're also interested in exploring a version built with Blazor WebAssembly, it's now available! Please visit [CleanAspire Repository](https://github.com/neozhu/cleanaspire) for more details.
[![](https://private-user-images.githubusercontent.com/1549611/386846455-013b167b-59fa-42d7-a2f7-ffec301c4e11.jpg?jwt=eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJnaXRodWIuY29tIiwiYXVkIjoicmF3LmdpdGh1YnVzZXJjb250ZW50LmNvbSIsImtleSI6ImtleTUiLCJleHAiOjE3MzMyMDgzNzUsIm5iZiI6MTczMzIwODA3NSwicGF0aCI6Ii8xNTQ5NjExLzM4Njg0NjQ1NS0wMTNiMTY3Yi01OWZhLTQyZDctYTJmNy1mZmVjMzAxYzRlMTEuanBnP1gtQW16LUFsZ29yaXRobT1BV1M0LUhNQUMtU0hBMjU2JlgtQW16LUNyZWRlbnRpYWw9QUtJQVZDT0RZTFNBNTNQUUs0WkElMkYyMDI0MTIwMyUyRnVzLWVhc3QtMSUyRnMzJTJGYXdzNF9yZXF1ZXN0JlgtQW16LURhdGU9MjAyNDEyMDNUMDY0MTE1WiZYLUFtei1FeHBpcmVzPTMwMCZYLUFtei1TaWduYXR1cmU9N2U0MjZjYTYxZTEyYjljZjhlOWRlNWNhY2VjNmE3NTRkYjE3NmQzMDljZGYxZjdkMWY1MDhkNTUyYjg4ODA2MyZYLUFtei1TaWduZWRIZWFkZXJzPWhvc3QifQ.XGF7ccu4iG-spz7w2lUk8QcD_y0WVFR92xp-nCkbY34)](https://github.com/neozhu/cleanaspire)

## My WebSite
Feel free to reach out to me for collaboration
[https://blazorserver.com](https://blazorserver.com)

## ❤️ Support This Project

If you find this project helpful, please consider supporting its development:
- **PayPal**: [paypal.me/hualinz](https://paypal.me/hualinz)
- **GitHub Sponsors**: Click the "Sponsor" button at the top of this repository



Your support helps maintain and improve this project. Thank you! 🙏

For more sponsorship information, see [SPONSORSHIP.md](docs/SPONSORSHIP.md).

## Projects Based on This Template


Here are some projects that have been developed based on this Clean Architecture Blazor Server template:

[![HSE Management System Screenshot](/doc/094346.png)](https://hse.blazorserver.com/)
1. **HSE Management System (Workflow Application)**
   - [GitHub Repository](https://github.com/neozhu/workflow)

[![The DPP Application Screenshot](/doc/094553.png)](https://materialpassport.blazorserver.com/)
2. **The DPP Application (EU Digital Product Passport)**
   - [Live Demo](https://materialpassport.blazorserver.com/)





## Development Setup

To get started with development, ensure you have the following tools and environments set up:


- [.NET 9.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
- [Microsoft Visual Studio 2022](https://visualstudio.microsoft.com/downloads/) or [Rider](https://www.jetbrains.com/rider/)
- [Docker](https://www.docker.com/)




## Authentication Setup

Use the following topics to configure your application to use the respective providers:

- [Facebook instructions](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/social/facebook-logins?view=aspnetcore-8.0)
- [Twitter instructions](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/social/twitter-logins?view=aspnetcore-8.0)
- [Google instructions](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/social/google-logins?view=aspnetcore-8.0)
- [Microsoft instructions](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/social/microsoft-logins?view=aspnetcore-8.0)
- [Other provider instructions](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/social/other-logins?view=aspnetcore-8.0)

https://learn.microsoft.com/en-us/aspnet/core/security/authentication/social/?view=aspnetcore-8.0&tabs=visual-studio

## Docker Setup for Blazor Server Application

### Pull the Docker Image

First, pull the latest version of the Blazor Server Docker image:

```bash
docker pull blazordevlab/cleanarchitectureblazorserver:latest
```

### Run the Docker Container

You can start the container in two modes: using an in-memory database for development purposes or connecting to an MSSQL
database for persistent storage and configuring SMTP for email functionalities.

For Development (In-Memory Database):

```bash
docker run -p 8443:443 -e UseInMemoryDatabase=true -e ASPNETCORE_ENVIRONMENT=Development -e ASPNETCORE_HTTPS_PORTS=443 blazordevlab/cleanarchitectureblazorserver:latest
```

For Production (Persistent Database and SMTP Configuration):

```bash

docker run -d -p 8443:443 \
-e UseInMemoryDatabase=false \
-e ASPNETCORE_ENVIRONMENT=Development \
-e ASPNETCORE_HTTP_PORTS=80 \
-e ASPNETCORE_HTTPS_PORTS=443 \
-e DatabaseSettings__DBProvider=mssql \
-e DatabaseSettings__ConnectionString="Server=127.0.0.1;Database=BlazorDashboardDb;User Id=sa;Password=<YourPassword>;MultipleActiveResultSets=true;Encrypt=false;TrustServerCertificate=false" \
-e SmtpClientOptions__User=<YourSMTPUser> \
-e SmtpClientOptions__Port=25 \
-e SmtpClientOptions__Server=<YourSMTPServer> \
-e SmtpClientOptions__Password=<YourSMTPPassword> \
-e Authentication__Microsoft__ClientId=<YourMicrosoftClientId> \
-e Authentication__Microsoft__ClientSecret=<YourMicrosoftClientSecret> \
-e Authentication__Google__ClientId=<YourGoogleClientId> \
-e Authentication__Google__ClientSecret=<YourGoogleClientSecret> \
-e Authentication__Facebook__AppId=<YourFacebookAppId> \
-e Authentication__Facebook__AppSecret=<YourFacebookAppSecret> \
blazordevlab/cleanarchitectureblazorserver:latest
```

Replace placeholder values (<Your...>) with your actual configuration details.

### Docker Compose Setup

For easier management, use a docker-compose.yml file:

```yml
```yaml
version: '3.8'
services:
  blazorserverapp:
    image: blazordevlab/cleanarchitectureblazorserver:latest
    environment:
      - UseInMemoryDatabase=false
      - ASPNETCORE_ENVIRONMENT=Development
      - ASPNETCORE_URLS=http://+:80;https://+:443
      - ASPNETCORE_HTTP_PORTS=80
      - ASPNETCORE_HTTPS_PORTS=443
      - DatabaseSettings__DBProvider=mssql
      - DatabaseSettings__ConnectionString=Server=127.0.0.1;Database=BlazorDashboardDb;User Id=sa;Password=***;MultipleActiveResultSets=true;Encrypt=false;TrustServerCertificate=false
      - SmtpClientOptions__User=<YourSMTPUser>
      - SmtpClientOptions__Port=25
      - SmtpClientOptions__Server=<YourSMTPServer>
      - SmtpClientOptions__Password=<YourSMTPPassword>
      - Authentication__Microsoft__ClientId=<YourMicrosoftClientId>
      - Authentication__Microsoft__ClientSecret=<YourMicrosoftClientSecret>
      - Authentication__Google__ClientId=<YourGoogleClientId>
      - Authentication__Google__ClientSecret=<YourGoogleClientSecret>
      - Authentication__Facebook__AppId=<YourFacebookAppId>
      - Authentication__Facebook__AppSecret=<YourFacebookAppSecret>
    ports:
      - "8443:443"
    volumes:
      - files_volume:/app/Files

  mssql:
    image: mcr.microsoft.com/mssql/server:2022-latest
    environment:
      - ACCEPT_EULA=Y
      - SA_PASSWORD=YourStrongPassword!
    ports:
      - "1433:1433"
    volumes:
      - mssql_data:/var/opt/mssql

volumes:
  files_volume:
  mssql_data:
```


### Notes:

Replace <Your...> placeholders with actual values from your environment.
The files_volume volume is used for persistent storage of application files. Adjust or extend volumes based on your
specific needs.
This optimized guide should help in setting up your Blazor Server application with either an in-memory or MSSQL
database, configured SMTP server for email functionalities, and OAuth authentication for Microsoft, Google, and
Facebook.

## Supported Databases

* PostgreSQL (Provider Name: `postgresql`)
* Microsoft SQL Server (Provider Name: `mssql`)
* SQLite (Provider Name: `sqlite`)

### How to select a specific Database?

1. Open the `appsettings.json` file located in the src directory of the `Server.UI` project.
2. Change the setting `DBProvider` to the desired provider name (See Supported Databases).
3. Change the `ConnectionString` to a connection string, which works for your selected database provider.

## Docker Compose HTTPS Deployment

- Create self-signed development certificates for the project
  -
  cmd: `dotnet dev-certs https -ep $env:USERPROFILE\.aspnet\https\CleanArchitecture.Blazor.Server.UI.pfx -p Password@123`
    - cmd: `dotnet dev-certs https --trust`
- Manage User secrets to save the password
    - cmd: `dotnet user-secrets init`
    - cmd: `dotnet user-secrets -p Server.UI.csproj set "Kestrel:Certificates:Development:Password" "Password@123"`

## Code Generator Extension for Visual Studio 2022

The [CleanArchitecture CodeGenerator for Blazor App](https://github.com/neozhu/CleanArchitectureCodeGenerator) is a powerful Visual Studio extension that helps developers streamline the creation of Blazor applications based on Clean Architecture principles.


<div><video controls src="https://user-images.githubusercontent.com/1549611/197116874-f28414ca-7fc1-463a-b887-0754a5bb3e01.mp4" muted="false"></video></div>

### Features of CodeGenerator

- Automatically generates standard code for:
  - Application Layer
  - Domain Events
  - Blazor UI Layer
- Simplifies the process of adding new entities and business logic.
- Enhances productivity by minimizing repetitive coding tasks.



### Install CleanArchitecture CodeGenerator For Blazor App

- Open Manage Extensions Search with `CleanArchitecture CodeGenerator For Blazor App`
  ![image](https://github.com/neozhu/CleanArchitectureWithBlazorServer/assets/1549611/555d274c-8f62-438b-ac7a-6d327e4c23c8)
- Download to Install

 

## How to install solution templates

- Install the project template
    - run CLI: `dotnet new install ./`
    - run CLI: `dotnet new list`

<img width="828" alt="image" src="https://github.com/neozhu/CleanArchitectureWithBlazorServer/assets/1549611/f23022e0-3fd6-475a-96ab-84b0d3328e4c">

- create a solution with the template
    - run CLI: `dotnet new ca-blazorserver-sln` or `dotnet new ca-blazorserver-sln -n NewProjectName(root namespaces)`


- build a project template with nuget.exe
    - run CLI: `.\nuget.exe add -Source .\ CleanArchitecture.Blazor.Solution.Template.1.0.0-preview.1.nupkg`
      <img width="1329" alt="image" src="https://github.com/neozhu/CleanArchitectureWithBlazorServer/assets/1549611/7c56dec5-5935-4fc5-ad81-8bcb65e9d0dc">
    - create a new project from Clean Architecture for Blazor Server Solution
      <img width="769" alt="image" src="https://github.com/neozhu/CleanArchitectureWithBlazorServer/assets/1549611/ed7eb20f-aec2-4f69-95b7-d47c2eb20428">


## Tutorial: Removing a cutomer Object from a Project
[![Everything Is AWESOME](doc/remove.png)](https://www.youtube.com/watch?v=i3p-3I95YqM "Everything Is AWESOME")


## Tutorial: Adding a contact Entity in the project
[![Everything Is AWESOME](doc/create.png)](https://www.youtube.com/watch?v=X1b4hFLs4vo "Everything Is AWESOME")


## Why I chose Blazor Server

I prefer Blazor Server because I dislike switching between C# and JavaScript during development. Blazor Server allows me to focus on C#.

## Characteristics
- Real-Time Updates
- Avoid repeating work
- Focus on story implementation
- Integration with the Hangfire dashboard
- Implementation of OCR image recognition
- org chart



https://github.com/neozhu/CleanArchitectureWithBlazorServer/assets/1549611/2b33254c-1022-472b-90c2-b0c0e068494f


![image](https://user-images.githubusercontent.com/1549611/185576711-31ab3081-ba22-43f3-b837-c8f1de981442.png)

![image](doc/orgchart.png)





## License

MIT License
