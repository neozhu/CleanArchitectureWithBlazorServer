# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [2.1.0] - 2025-07-31

### 🚀 Added
- **User Context Accessor System**: Comprehensive user context management with FusionCache integration
  - `UserContextAccessor`: New service for accessing user context throughout the application
  - `UserContextLoader`: Efficient user data loading with FusionCache integration
  - `UserContextHubFilter`: SignalR hub filter for user context management
  - `UserSessionCircuitHandler`: Blazor circuit handler for user session management
- **FusionCache Integration**: Implemented caching for user context data with default configuration
- **SignalR Support**: Added hub filter for real-time user context updates
- **Circuit Management**: Enhanced Blazor circuit handling for user sessions
- **State Management**: Modernized UserProfile state management with immutable patterns

### 🔧 Changed
- **Method Naming**: Renamed `Initial` to `Initialize` in IOnlineUserTracker for consistency
- **Cache Configuration**: Optimized UserContextLoader cache configuration using default settings
- **UserProfile Logic**: Enhanced UserProfile initialization logic in AppLayout component

### 🧹 Cleaned Up
- Removed test code and debug logs
- Cleaned up temporary files and unused code

### 🔐 Security
- Enhanced user session management with proper circuit handling
- Improved authentication flow with context-aware user data

### 📊 Performance
- Optimized user context loading with FusionCache
- Reduced memory usage with immutable state patterns
- Improved real-time communication with SignalR hub filters

### 🧪 Testing
- Added UserContext test page for validation
- Comprehensive integration testing for all new components

## [2.0.0] - 2024-12-15

### 🚀 Major Features
- **.NET 9 Support**: Upgraded to .NET 9 for latest features and performance
- **Enhanced AI Integration**: Improved Cursor AI rules and development workflow
- **Multi-Database Support**: Full support for SQL Server, PostgreSQL, and SQLite
- **Advanced Security**: Multi-factor authentication and role-based access control
- **Real-time Features**: SignalR integration for live updates
- **Progressive Web App**: PWA capabilities for mobile experience

### 🔧 Architecture Improvements
- **Clean Architecture**: Strict layer separation with dependency inversion
- **CQRS Pattern**: Command Query Responsibility Segregation implementation
- **MediatR Integration**: Event-driven architecture with MediatR
- **FluentValidation**: Comprehensive input validation
- **FusionCache**: Advanced caching with Redis support

### 🎨 UI/UX Enhancements
- **MudBlazor**: Modern, responsive UI components
- **Advanced Data Grid**: Sorting, filtering, pagination, and export
- **Multi-tenancy**: Built-in tenant isolation and management
- **Responsive Design**: Mobile-first approach

### 🐳 DevOps
- **Docker Support**: Complete containerization
- **GitHub Actions**: Automated CI/CD pipeline
- **CodeQL Analysis**: Security scanning
- **NuGet Package**: Template distribution

### 📚 Documentation
- **Comprehensive Guides**: Architecture, development, and deployment
- **Video Tutorials**: Step-by-step implementation guides
- **API Documentation**: Complete API reference
- **Contributing Guidelines**: Community contribution process

## [1.0.0] - 2023-06-15

### 🎉 Initial Release
- **Blazor Server Template**: Clean Architecture implementation
- **Basic CRUD Operations**: Entity management with code generation
- **Authentication**: ASP.NET Core Identity integration
- **Database Support**: Entity Framework Core with SQL Server
- **Basic UI**: MudBlazor components and layouts

---

## Version History

- **v2.1.0**: User Context Accessor with FusionCache integration
- **v2.0.0**: .NET 9 upgrade with enhanced AI integration
- **v1.0.0**: Initial Blazor Server template release

## Migration Guides

### Upgrading from v2.0.0 to v2.1.0

1. **Update Dependencies**: Ensure all NuGet packages are updated to latest versions
2. **User Context Integration**: The new UserContextAccessor system is backward compatible
3. **Cache Configuration**: FusionCache integration uses default settings
4. **Circuit Handling**: Enhanced circuit management is automatic

### Upgrading from v1.x to v2.x

1. **.NET 9 Migration**: Update target framework to net9.0
2. **Package Updates**: Update all NuGet packages to .NET 9 compatible versions
3. **Breaking Changes**: Review API changes in detailed migration guide
4. **Database Migration**: Run EF Core migrations for schema updates

---

## Support

- **Documentation**: [docs/](docs/)
- **Issues**: [GitHub Issues](https://github.com/neozhu/CleanArchitectureWithBlazorServer/issues)
- **Discussions**: [GitHub Discussions](https://github.com/neozhu/CleanArchitectureWithBlazorServer/discussions)
- **Live Demo**: [architecture.blazorserver.com](https://architecture.blazorserver.com/)