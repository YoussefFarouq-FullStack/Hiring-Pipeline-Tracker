# Hiring Pipeline Tracker

A comprehensive full-stack application for managing the hiring pipeline process, built with .NET 8 Web API and Angular 17.

## 🏗️ Architecture

This project follows **Clean Architecture** principles with a 3-layer backend structure:

```
Hiring Pipeline Tracker/
├── HiringPipelineUI/           # Frontend (Angular 17)
├── HiringPipelineCore/         # Domain Layer
│   ├── Entities/               # Business models
│   ├── Interfaces/             # Service & Repository contracts
│   └── Exceptions/             # Domain exceptions
├── HiringPipelineInfrastructure/ # Infrastructure Layer
│   ├── Data/                   # DbContext & migrations
│   ├── Repositories/           # Data access implementations
│   ├── Services/              # Business logic implementations
│   └── Migrations/             # EF Core migrations
├── HiringPipelineAPI/          # API Layer (entry point)
│   ├── Controllers/            # HTTP endpoints
│   ├── DTOs/                   # Data transfer objects
│   ├── Services/               # API services (DTO mapping)
│   ├── Validators/             # FluentValidation rules
│   ├── Mappings/               # AutoMapper profiles
│   ├── Middleware/             # Global exception handling
│   ├── Filters/                # API filters
│   ├── Properties/             # Launch settings
│   └── wwwroot/                # Static files
├── Database/                   # Database files
└── HiringPipelineTracker.sln   # Unified solution
```

## 🚀 Getting Started

### Prerequisites

- **.NET 8 SDK**
- **Node.js 18+** and **npm**
- **SQL Server** (LocalDB or full instance)
- **Visual Studio 2022** or **VS Code**

### Backend Setup

1. **Restore dependencies:**
```bash
   dotnet restore
```

2. **Build the solution:**
```bash
   dotnet build
   ```

3. **Run the API:**
```bash
   dotnet run --project HiringPipelineAPI
   ```
   
   The API will be available at: `http://localhost:5192`
   API Documentation: `http://localhost:5192/api-docs`

### Frontend Setup

1. **Navigate to the UI folder:**
   ```bash
   cd HiringPipelineUI
   ```

2. **Install dependencies:**
   ```bash
   npm install
   ```

3. **Start the development server:**
```bash
   npm start
   ```
   
   The frontend will be available at: `http://localhost:4200`

## 📋 Features

### Core Entities
- **Candidates**: Manage job applicants with contact information, skills, resume file uploads, and detailed descriptions
- **Requisitions**: Track job openings with department and level details
- **Applications**: Link candidates to job requisitions with stage tracking
- **Stage History**: Monitor progression through the hiring pipeline

### API Endpoints
- **Candidates**: `GET/POST/PUT/DELETE /api/candidates`
- **Requisitions**: `GET/POST/PUT/DELETE /api/requisitions`
- **Applications**: `GET/POST/PUT/DELETE /api/applications`
- **Stage History**: `GET/POST/PUT/DELETE /api/stagehistory`
- **File Upload**: `POST/GET/DELETE /api/fileupload` (for resume files)

### Frontend Features
- **Dashboard**: Overview of hiring pipeline metrics
- **Candidate Management**: Add, edit, and track candidates with file uploads and descriptions
- **Requisition Management**: Create and manage job openings
- **Application Tracking**: Monitor application progress with proper validation
- **Stage Progression**: Move applications through hiring stages
- **File Upload**: Resume/CV file upload with drag-and-drop interface

## 🛠️ Technology Stack

### Backend
- **.NET 8** - Web API framework
- **Entity Framework Core** - ORM for database operations
- **SQL Server** - Database
- **AutoMapper** - Object mapping
- **FluentValidation** - Input validation
- **Swagger/OpenAPI** - API documentation

### Frontend
- **Angular 17** - Frontend framework
- **TypeScript** - Type-safe JavaScript
- **SCSS** - Styling
- **Angular Material** - UI components
- **RxJS** - Reactive programming

### Architecture
- **Clean Architecture** - Separation of concerns
- **Repository Pattern** - Data access abstraction
- **Dependency Injection** - Service management
- **CORS** - Cross-origin resource sharing

## 🔧 Development

### Project Structure Benefits
- **✅ Clean Architecture**: Clear separation of concerns
- **✅ Maintainable**: Each layer has a single responsibility
- **✅ Scalable**: Easy to add new features
- **✅ Testable**: Well-defined interfaces and dependencies
- **✅ Professional**: Production-ready structure

### Key Files
- **Solution**: `HiringPipelineTracker.sln` - Main solution file
- **API Entry**: `HiringPipelineAPI/Program.cs` - Application startup
- **Database**: `HiringPipelineInfrastructure/Data/HiringPipelineDbContext.cs`
- **Frontend Entry**: `HiringPipelineUI/src/main.ts`

## 🚀 Deployment

### Backend Deployment
1. Build the solution: `dotnet build --configuration Release`
2. Publish the API: `dotnet publish HiringPipelineAPI --configuration Release`
3. Deploy to your hosting platform (Azure, AWS, etc.)

### Frontend Deployment
1. Build the Angular app: `ng build --configuration production`
2. Deploy the `dist` folder to your web server

## 🔄 Recent Updates

### Candidate Management Enhancements
- **✅ File Upload Support**: Added resume/CV file upload functionality with drag-and-drop interface
- **✅ Description Field**: Added detailed description field for candidate notes and additional information
- **✅ Skills Validation**: Made skills field required with proper validation
- **✅ Compact UI**: Optimized dialog layout for better user experience
- **✅ Database Migration**: Updated schema to support new file upload and description fields

### Application Management Improvements
- **✅ Validation Fixes**: Fixed 400 Bad Request errors in application creation
- **✅ Status Alignment**: Aligned frontend status options with backend validation rules
- **✅ Stage Management**: Updated current stage options to match backend requirements
- **✅ Error Handling**: Enhanced error messages with detailed validation feedback

### Code Quality Improvements
- **✅ Build Warnings**: Reduced backend build warnings by 42 (19% improvement)
- **✅ XML Documentation**: Added comprehensive API documentation for Swagger
- **✅ Type Safety**: Improved TypeScript type safety and validation
- **✅ Clean Code**: Removed debugging code and unnecessary comments

## 📚 API Documentation

Once the API is running, visit `http://localhost:5192/api-docs` for interactive API documentation.

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Test thoroughly
5. Submit a pull request

## 📄 License

This project is licensed under the MIT License.

---

**Built with ❤️ using Clean Architecture principles**
