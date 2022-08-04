# Clean Architecture With Blazor Server
This is a repository for creating a  Blazor Server application following the principles of Clean Architecture
## Live Demo
-  Blazor Server mode: https://mudblazor-s.dotnet6.cn/
-  (IP accelerate): http://106.52.105.140:6101/
## Screenshots and video
[![Everything Is AWESOME](doc/page.png)](https://www.youtube.com/embed/GyZJl_dG-Pg "Everything Is AWESOME")

## Development Enviroment
- Microsoft Visual Studio Community 2022 (64-bit) 
- Docker
- .NET 6.0

## Support database
- SQL Server(Express or localdb) - default
- MySql and mariadb refer to branch: mysql

## Docker compose https deployment
- Create self-signed development certificates for the project
    - cmd: dotnet dev-certs https -ep $env:USERPROFILE\.aspnet\https\Blazor.Server.UI.pfx -p Password@123
    - cmd: dotnet dev-certs https --trust
- Manage User secrets to save password 
    - cmd: dotnet user-secrets init 
    - cmd: dotnet user-secrets -p Blazor.Server.UI.csproj set "Kestrel:Certificates:Development:Password" "Password@123"

## Improved Code generator
- CleanArchitectureCodeGenerator 
- https://github.com/neozhu/CleanArchitectureCodeGenerator
- Generate all the required code from the template
    - Application Features Code
        - ![image](https://user-images.githubusercontent.com/1549611/181414766-84850a90-3a21-47ed-afcf-1b5cdd602edf.png)
    - UI Pages View
        - ![image](https://user-images.githubusercontent.com/1549611/181414818-5c1c2dfc-5560-4ab2-8773-dc7c816730d4.png)

## How to build solution templates
- run CLI: dotnet new --install sayedha.templates 
- create solution/project template
    - create .template.config folder 
    - run CLI: dotnet new templatejson
    - edit templatejson file
- install the project template
    - run CLI:  dotnet new --install ./
    - run CLI:  dotnet new --list
- create a solution with the template
    - run CLI: dotnet new ca-blazor-sln
![image](https://user-images.githubusercontent.com/1549611/182025444-04c9c8db-2b11-44b3-8091-acffcc37a899.png)

## Why develop with blazor server mode
- Develop fast
- Runing fast
- Keep simple

## Characteristic
- Clean principles
- Simple principles
- Easy to start with

## About
Coming up.



## License
MIT License
