# Clean Architecture With Blazor Server
This is a repository for creating a  Blazor Server application following the principles of Clean Architecture
## Live Demo
-  Blazor Server mode: https://mudblazor-s.dotnet6.cn/
-  (IP accelerate): http://106.52.105.140:6101/
## Screenshots and video
[![Everything Is AWESOME](doc/main_screenshot.png)](https://www.youtube.com/embed/GyZJl_dG-Pg "Everything Is AWESOME")

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

## Code generation
- CleanArchitectureCodeGenerator 
- https://github.com/neozhu/CleanArchitectureCodeGenerator
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
