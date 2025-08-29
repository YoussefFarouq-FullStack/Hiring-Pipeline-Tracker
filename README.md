# Hiring Pipeline Tracker

A comprehensive web application for managing the entire hiring process, from job requisitions to candidate applications and stage tracking.

## 🚀 Features

### Core Functionality
- **Requisition Management**: Create, edit, and track job openings
- **Candidate Management**: Manage candidate profiles and applications
- **Application Tracking**: Monitor candidates through various hiring stages
- **Stage History**: Complete audit trail of candidate progression
- **Real-time Updates**: Live data synchronization between frontend and backend

### Hiring Pipeline Stages
- **Applied** → **Screening** → **Interviewing** → **Technical Assessment** → **Reference Check** → **Offer** → **Hired**
- **Rejected** and **Withdrawn** statuses for closed applications

### Dashboard & Analytics
- **Real-time Statistics**: Total candidates, active pipeline, hired count
- **Status Distribution**: Visual breakdown of candidates by stage
- **Performance Metrics**: Hiring pipeline efficiency tracking

## 🏗️ Architecture

### Frontend (Angular 20.1.6)
- **Framework**: Angular with standalone components
- **Styling**: Tailwind CSS for modern, responsive design
- **UI Components**: Angular Material for consistent interface
- **State Management**: Reactive services with RxJS
- **Proxy Configuration**: Properly configured for API routing

### Backend (ASP.NET Core 8)
- **Framework**: .NET 8 Web API
- **Database**: Entity Framework Core with SQL Server
- **Architecture**: Clean, RESTful API design
- **CORS**: Configured for secure frontend-backend communication
- **DTOs**: Structured data transfer objects for API communication

### Database
- **SQL Server**: Robust relational database
- **Schema**: Normalized design with proper relationships
- **Migrations**: Code-first approach with Entity Framework
- **Identity Management**: Automatic seed reset for optimal ID management

## 📋 Prerequisites

### Development Environment
- **.NET 8 SDK** - [Download here](https://dotnet.microsoft.com/download/dotnet/8.0)
- **Node.js 18+** - [Download here](https://nodejs.org/)
- **SQL Server** - LocalDB, Express, or Developer Edition
- **Visual Studio 2022** or **VS Code** (recommended)

### Required Software
- **Git** for version control
- **SQL Server Management Studio** (optional, for database management)

## 🛠️ Installation & Setup

### 1. Clone the Repository
```bash
git clone <your-repository-url>
cd HiringPipelineTracker
```

### 2. Backend Setup
```bash
cd HiringPipelineAPI

# Restore NuGet packages
dotnet restore

# Update database connection string in appsettings.json
# Default: "Server=localhost;Database=HiringPipelineDB;Trusted_Connection=True;TrustServerCertificate=True;"

# Run database migrations
dotnet ef database update

# Start the API
dotnet run
```

**Backend will be available at:** `https://localhost:5192`

### 3. Frontend Setup
```bash
cd HiringPipelineUI

# Install dependencies
npm install

# Start development server
npm start
```

**Frontend will be available at:** `http://localhost:4200`

### 4. Database Setup
The application will automatically create the database and tables on first run. Alternatively, you can run the schema manually:

```sql
-- Run the schema.sql file in your SQL Server instance
-- Located in: Database/schema.sql
```

## 🔧 Configuration

### Backend Configuration
- **Database Connection**: Update `appsettings.json` with your SQL Server details
- **CORS**: Configured for `http://localhost:4200` (Angular dev server)
- **Environment**: Development/Production settings in `appsettings.Development.json`
- **Identity Management**: Automatic seed reset for optimal database performance

### Frontend Configuration
- **API Proxy**: Configured to route `/api` requests to backend via `proxy.conf.json`
- **Build Settings**: Optimized for development and production
- **Tailwind CSS**: Custom configuration for consistent design system
- **Angular Configuration**: Properly configured serve options for development

## 📱 Usage

### Getting Started
1. **Launch the application** - Both frontend and backend should be running
2. **Navigate to** `http://localhost:4200` in your browser
3. **Create your first requisition** - Add a job opening
4. **Add candidates** - Start building your talent pipeline
5. **Track progress** - Move candidates through hiring stages

### Key Workflows

#### Managing Requisitions
1. Click **"Add Requisition"** button
2. Fill in job details (title, department, level, status)
3. Save and manage throughout the hiring process
4. Close when position is filled

#### Managing Candidates
1. Click **"Add Candidate"** button
2. Enter candidate information (name, email, phone, source)
3. Assign to requisitions as applications
4. Track through hiring stages
5. Update status as candidates progress

#### Application Tracking
1. **View applications** in the main dashboard
2. **Update candidate status** as they move through stages
3. **Monitor pipeline** with real-time statistics
4. **Track history** of all stage changes

## 🗄️ Database Schema

### Core Tables
- **Requisitions**: Job openings and requirements
- **Candidates**: Applicant profiles and information
- **Applications**: Candidate-requisition relationships
- **StageHistory**: Complete audit trail of status changes

### Key Relationships
- One requisition can have multiple applications
- One candidate can apply to multiple requisitions
- Each application tracks current stage and status
- Stage history maintains complete progression timeline

### Recent Database Updates
- **New Migrations**: Added initial database setup migration
- **Identity Management**: Automatic seed reset for optimal ID management
- **Data Integrity**: Enhanced foreign key constraints and validation

## 🔌 API Endpoints

### Candidates
- `GET /api/candidate` - List all candidates
- `GET /api/candidate/{id}` - Get specific candidate
- `POST /api/candidate` - Create new candidate
- `PUT /api/candidate/{id}` - Update candidate
- `DELETE /api/candidate/{id}` - Delete candidate
- `GET /api/candidate/next-id` - Get next available ID
- `DELETE /api/candidate/delete-all` - Delete all candidates and reset seed

### Requisitions
- `GET /api/requisitions` - List all requisitions
- `GET /api/requisitions/{id}` - Get specific requisition
- `POST /api/requisitions` - Create new requisition
- `PUT /api/requisitions/{id}` - Update requisition
- `DELETE /api/requisitions/{id}` - Delete requisition
- `GET /api/requisitions/next-id` - Get next available ID
- `DELETE /api/requisitions/delete-all` - Delete all requisitions and reset seed

### Applications
- `GET /api/applications` - List all applications
- `GET /api/applications/{id}` - Get specific application
- `POST /api/applications` - Create new application
- `PUT /api/applications/{id}` - Update application
- `DELETE /api/applications/{id}` - Delete application
- `DELETE /api/applications/delete-all` - Delete all applications and reset seed

### Stage Histories
- `GET /api/stagehistory` - List all stage histories
- `GET /api/stagehistory/{id}` - Get specific stage history
- `GET /api/stagehistory/application/{applicationId}` - Get history for specific application
- `POST /api/stagehistory` - Create new stage history
- `DELETE /api/stagehistory/{id}` - Delete stage history
- `DELETE /api/stagehistory/delete-all` - Delete all stage histories and reset seed

### Health Check
- `GET /api/health` - API health status
- `POST /api/health/reset-identity-seeds` - Reset all identity seeds

## 🎨 UI Components

### Material Design Integration
- **Tables**: Sortable, filterable data grids
- **Dialogs**: Modal forms for data entry
- **Cards**: Information display and statistics
- **Buttons**: Consistent action controls
- **Snackbars**: User feedback and notifications

### Responsive Design
- **Mobile-first** approach
- **Grid layouts** that adapt to screen size
- **Touch-friendly** interface elements
- **Accessible** design patterns

### New Components
- **Applications Component**: Complete application management interface
- **Application Dialog**: Modal for creating and editing applications
- **Enhanced Navigation**: Improved routing and component organization

## 🚀 Development

### Project Structure
```
HiringPipelineTracker/
├── HiringPipelineAPI/          # Backend API
│   ├── Controllers/            # API endpoints
│   ├── Models/                 # Data models
│   ├── DTOs/                   # Data transfer objects
│   ├── Data/                   # Database context
│   └── Migrations/             # Database migrations
├── HiringPipelineUI/           # Frontend application
│   ├── src/app/                # Angular components
│   ├── src/app/components/     # Feature components
│   │   ├── applications/       # Applications management
│   │   ├── candidates/         # Candidate management
│   │   └── requisitions/       # Requisition management
│   ├── src/app/services/       # API services
│   └── src/app/models/         # TypeScript interfaces
└── Database/                   # Database schema and scripts
```

### Development Commands

#### Backend
```bash
cd HiringPipelineAPI

# Run in development mode
dotnet run

# Run with hot reload
dotnet watch run

# Create new migration
dotnet ef migrations add MigrationName

# Update database
dotnet ef database update
```

#### Frontend
```bash
cd HiringPipelineUI

# Start development server
npm start

# Build for production
npm run build

# Run tests
npm test

# Lint code
npm run lint
```

## 🧪 Testing

### Backend Testing
- **Unit Tests**: Component and service testing
- **Integration Tests**: API endpoint testing
- **Database Tests**: Entity Framework testing

### Frontend Testing
- **Unit Tests**: Component and service testing
- **E2E Tests**: End-to-end user workflow testing
- **Linting**: Code quality and style enforcement

## 🔒 Security Features

- **CORS Configuration**: Secure cross-origin requests
- **Input Validation**: Server-side data validation
- **SQL Injection Protection**: Entity Framework parameterized queries
- **HTTPS**: Secure communication in production

## 📊 Performance

### Optimization Features
- **Lazy Loading**: Angular component loading
- **Database Indexing**: Optimized query performance
- **Caching**: Strategic data caching strategies
- **Compression**: Response compression for faster loading
- **Identity Management**: Automatic database seed optimization

### Monitoring
- **Health Checks**: API availability monitoring
- **Performance Metrics**: Response time tracking
- **Error Logging**: Comprehensive error tracking

## 🚀 Deployment

### Production Build
```bash
# Backend
cd HiringPipelineAPI
dotnet publish -c Release

# Frontend
cd HiringPipelineUI
npm run build
```

### Deployment Options
- **Azure**: App Service and SQL Database
- **AWS**: EC2 and RDS
- **Docker**: Containerized deployment
- **On-Premises**: Traditional server deployment

## 🤝 Contributing

### Development Guidelines
1. **Fork the repository**
2. **Create a feature branch**
3. **Follow coding standards**
4. **Write tests for new features**
5. **Submit a pull request**

### Code Standards
- **C#**: Follow Microsoft C# coding conventions
- **TypeScript**: Use strict mode and proper typing
- **CSS**: Follow Tailwind CSS utility-first approach
- **Git**: Use conventional commit messages

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🆘 Support

### Getting Help
- **Documentation**: Check this README and inline code comments
- **Issues**: Report bugs and feature requests via GitHub Issues
- **Discussions**: Use GitHub Discussions for questions and ideas

### Common Issues
- **Database Connection**: Verify SQL Server is running and accessible
- **Port Conflicts**: Ensure ports 4200 and 5192 are available
- **Dependencies**: Run `npm install` and `dotnet restore` if needed
- **Proxy Issues**: Ensure Angular proxy configuration is correct

### Recent Fixes
- **Proxy Configuration**: Fixed Angular proxy configuration conflicts
- **API Routing**: Resolved 404 errors for application endpoints
- **Component Organization**: Improved frontend component structure

## 🔮 Roadmap

### Planned Features
- **Advanced Analytics**: Hiring pipeline performance metrics
- **Email Integration**: Automated candidate communication
- **Calendar Integration**: Interview scheduling
- **Mobile App**: Native mobile application
- **Multi-tenancy**: Support for multiple organizations

### Technical Improvements
- **Real-time Updates**: SignalR integration for live updates
- **Advanced Search**: Full-text search and filtering
- **API Versioning**: Proper API version management
- **Performance Monitoring**: Application performance insights
- **Enhanced Identity Management**: Further database optimization

---

**Built with ❤️ using Angular 20.1.6, .NET Core 8, and SQL Server**

*For more information, visit the project repository or contact the development team.*

## 📋 Recent Updates (Latest)

### Latest Technical Improvements
- **Fixed Angular Proxy Configuration**: Resolved conflicting proxy settings causing 404 errors
- **Added Applications Component**: Complete frontend interface for application management
- **Enhanced DTOs**: Added structured data transfer objects for better API communication
- **Database Migrations**: Added initial database setup and identity management improvements
- **Component Organization**: Improved frontend component structure and routing

### Bug Fixes
- **API Routing Issues**: Fixed 404 errors when calling application endpoints
- **Proxy Configuration**: Resolved Angular development server proxy conflicts
- **Component Loading**: Improved component initialization and error handling

### New Features
- **Applications Management**: Full CRUD operations for job applications
- **Enhanced Identity Management**: Automatic database seed reset for optimal performance
- **Improved Error Handling**: Better error messages and user feedback
- **Component Testing**: Added comprehensive test coverage for new components
