# Hiring Pipeline Tracker

A comprehensive full-stack application for managing the hiring pipeline process, built with .NET 8 Web API and Angular 17.

## 🏗️ Architecture

This project follows **Clean Architecture** principles with a 3-layer backend structure:

```
Hiring Pipeline Tracker/
├── HiringPipelineUI/           # Frontend (Angular 17)
├── HiringPipelineCore/         # Domain Layer
│   ├── DTOs/                   # Data transfer objects (consolidated)
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
│   ├── Services/               # API services (DTO mapping)
│   ├── Validators/             # FluentValidation rules
│   ├── Mappings/               # AutoMapper profiles
│   ├── Middleware/             # Global exception handling & audit logging
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
   API Documentation: `http://localhost:5192/swagger`

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
- **Authentication**: `POST /api/auth/login`, `POST /api/auth/refresh` (JWT token management)
- **User Management**: `GET/POST/PUT/DELETE /api/users` (Admin only)
- **Role Management**: `GET/POST/PUT/DELETE /api/roles` (Admin only)
- **Audit Logs**: `GET /api/auditlogs` (Activity tracking)
- **Database Management**: `POST /api/database/clear-*` (Data management)
- **Analytics**: `GET /api/analytics/*` (Dashboard metrics)

### Frontend Features
- **Dashboard**: Interactive overview with clickable metric cards and hired candidates modal
- **Candidate Management**: Add, edit, and track candidates with file uploads and descriptions
- **Requisition Management**: Create and manage job openings with modern dialog styling
- **Application Tracking**: Monitor application progress with proper validation
- **Stage Progression**: Move applications through hiring stages
- **File Upload**: Resume/CV file upload with drag-and-drop interface
- **Role-Based Access Control (RBAC)**: Secure access control with role-based permissions
- **Authentication**: JWT-based authentication with role-based authorization
- **Modern UI**: Consistent dialog styling with animations and loading states
- **Audit Logging**: Comprehensive activity tracking and user action monitoring

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
- **Tailwind CSS** - Utility-first CSS framework
- **Angular Material** - UI components
- **RxJS** - Reactive programming

### Architecture
- **Clean Architecture** - Separation of concerns
- **Repository Pattern** - Data access abstraction
- **Dependency Injection** - Service management
- **CORS** - Cross-origin resource sharing
- **JWT Authentication** - Secure token-based authentication
- **Audit Logging** - Comprehensive activity tracking
- **Middleware Pipeline** - Request/response processing

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

### 🗄️ Database Backup, Restore, and Schema
- **Location**: Local development uses SQL Server LocalDB via the connection string in `HiringPipelineAPI/appsettings.Development.json`.
- **Backup (.bak)**:
  - In SSMS: right-click database → Tasks → Back Up… → Full → save `.bak` under `Database/` (not committed).
  - Restore on another machine: Databases → Restore Database… → From device → select `.bak`.
- **Portable export (.bacpac)**:
  - In SSMS/Azure Data Studio: Tasks → Export Data-tier Application… → produce `.bacpac` (store outside git or via Git LFS).
- **Schema script (`Database/schema.sql`)**:
  - Generate with: Tasks → Generate Scripts… → Advanced → Types of data to script = Schema (or Schema and data if needed).
  - Apply by opening in SSMS and executing against an empty database.
- **Detach/Attach (.mdf/.ldf)**:
  - Only when needed: Tasks → Detach… → copy `.mdf/.ldf` → Attach… (avoid committing these files).
- **Pointing to a fresh DB**:
  - Change `Database=...` in `appsettings.json` (e.g., `HiringPipelineDB_New2`) and apply EF migrations.

### 📦 Git Guidance for Database Artifacts
- Commit `Database/schema.sql`.
- Ignore backups and database binaries. Example `.gitignore` entries:
  
  ```gitignore
  # Database artifacts
  Database/*.bak
  *.bak
  *.mdf
  *.ldf
  *.ndf
  ```

### 🎨 UI/UX Enhancements
- **✅ Modern Dialog Styling**: Consistent gradient headers, rounded corners, and shadow effects across all dialogs
- **✅ Smooth Animations**: Added slide-in animations for all dialog openings
- **✅ Loading States**: Implemented reusable loading spinner component with multiple variants
- **✅ Interactive Dashboard**: Made metric cards clickable with hover effects and navigation
- **✅ Hired Candidates Modal**: Added expandable modal showing detailed hired candidate information
- **✅ Confirmation Dialogs**: Replaced native browser alerts with styled confirmation dialogs
- **✅ Responsive Design**: Enhanced mobile and desktop compatibility

### 🏗️ Backend Architecture Improvements
- **✅ DTO Consolidation**: Moved all DTOs to Core project following Clean Architecture principles
- **✅ Duplicate File Cleanup**: Removed duplicate DTO files and empty folders
- **✅ Audit Logging System**: Comprehensive activity tracking with middleware-based logging
- **✅ Database Management**: Added granular database clearing options (business data, hiring data, or all data)
- **✅ JWT Authentication**: Enhanced with refresh tokens and proper token rotation
- **✅ LINQ Translation Fixes**: Resolved Entity Framework computed property translation issues

### 🔐 Security & Authentication
- **✅ JWT Token Management**: Secure access and refresh token implementation
- **✅ Password Security**: BCrypt hashing for secure password storage
- **✅ Role-Based Authorization**: Granular permissions based on user roles
- **✅ Token Refresh**: Automatic token renewal without re-authentication
- **✅ Session Management**: Proper logout and token invalidation

### 📊 Dashboard & Analytics
- **✅ Interactive Metrics**: Clickable cards for navigation to detailed views
- **✅ Real-time Data**: Live updates of candidate, requisition, and application counts
- **✅ Hired Candidates View**: Detailed modal with expandable candidate information
- **✅ Recent Activity**: Timeline of hiring pipeline activities
- **✅ Background Data Fetching**: Optimized API calls to prevent duplicate audit logs

### 🛠️ Developer Experience
- **✅ Demo Credentials**: Added collapsible login credentials section for easy testing
- **✅ Clean Project Structure**: Removed unused folders and consolidated duplicate files
- **✅ Build Optimization**: Improved build performance and reduced warnings
- **✅ Error Handling**: Enhanced error messages and validation feedback
- **✅ Code Documentation**: Comprehensive XML documentation for API endpoints

### 🗄️ Database & Data Management
- **✅ Safe Database Clearing**: Multiple clearing options to preserve important system data
- **✅ Audit Trail**: Complete activity logging for compliance and debugging
- **✅ Data Integrity**: Proper foreign key relationships and constraints
- **✅ Migration Management**: Clean migration history and schema updates

## 👥 Test Users & Credentials

The application comes with pre-seeded test users for different roles. **Demo credentials are available on the login page** for easy access:

| Username | Password | Role | Access Level |
|----------|----------|------|--------------|
| `admin` | `Admin123!` | Admin | **Full system control** - Manage users, roles, requisitions, candidates, applications |
| `recruiter1` | `Recruiter123!` | Recruiter | Create/manage requisitions, add candidates, move them through stages |
| `manager1` | `Manager123!` | Manager | Review candidates, give feedback, move to next stages |
| `interviewer1` | `Interviewer123!` | Interviewer | Limited access - view assigned candidates, submit interview feedback |
| `readonly1` | `ReadOnly123!` | Read-only | View only - can only view requisitions or candidate profiles, no edits |

> 💡 **Tip**: Click "Show Demo Credentials" on the login page to see all available test accounts with one-click form filling.

### 🔑 Role-Based Permissions

**Admin Role:**
- ✅ Manage all users and roles
- ✅ Create, edit, archive requisitions
- ✅ Add, update, delete candidates
- ✅ Move applications through all stages
- ✅ View analytics and system configuration

**Recruiter Role:**
- ✅ Create and manage requisitions
- ✅ Add and update candidates
- ✅ Assign candidates to requisitions
- ✅ Move applications through stages
- ❌ Cannot manage users or system settings

**Hiring Manager Role:**
- ✅ Review candidate applications
- ✅ Provide feedback on candidates
- ✅ Move applications to next stages
- ✅ View requisition details
- ❌ Cannot create requisitions or manage users

**Interviewer Role:**
- ✅ View assigned candidates
- ✅ Submit interview feedback
- ✅ View application details
- ❌ Cannot move applications or manage data

**Read-only Role:**
- ✅ View requisitions and candidate profiles
- ❌ Cannot make any edits or changes

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

## 🏆 Project Status

### ✅ Completed Features
- **Full CRUD Operations** for all entities (Candidates, Requisitions, Applications, Stage History)
- **Complete Authentication System** with JWT tokens and refresh token rotation
- **Role-Based Access Control** with granular permissions
- **File Upload System** for resume/CV management
- **Audit Logging** for comprehensive activity tracking
- **Modern UI/UX** with consistent styling and animations
- **Interactive Dashboard** with real-time metrics
- **Database Management** with safe clearing options
- **Clean Architecture** implementation with proper separation of concerns

### 🎯 Key Achievements
- **Zero Duplicate Code**: Consolidated all DTOs and removed duplicate files
- **Production-Ready**: Comprehensive error handling and validation
- **Scalable Architecture**: Clean separation allows easy feature additions
- **Security-First**: JWT authentication with proper token management
- **User-Friendly**: Intuitive interface with helpful demo credentials
- **Maintainable**: Well-documented code with clear project structure

### 🚀 Ready for Production
This application is production-ready with:
- Secure authentication and authorization
- Comprehensive audit logging
- Clean, maintainable codebase
- Modern, responsive UI
- Proper error handling and validation
- Database management capabilities

---

**Built with Clean Architecture principles and modern development practices** 🏗️✨
