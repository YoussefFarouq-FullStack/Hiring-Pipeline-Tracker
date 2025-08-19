-- Create the database
CREATE DATABASE HiringPipelineDB;
GO

USE HiringPipelineDB;
GO

-- Table: Requisition
CREATE TABLE Requisition (
    RequisitionId INT IDENTITY(1,1) PRIMARY KEY,
    Title NVARCHAR(200) NOT NULL,
    Department NVARCHAR(100) NOT NULL,
    JobLevel NVARCHAR(50) NULL,
    Status NVARCHAR(20) NOT NULL CHECK (Status IN ('Open', 'Closed')),
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);

-- Table: Candidate
CREATE TABLE Candidate (
    CandidateId INT IDENTITY(1,1) PRIMARY KEY,
    FirstName NVARCHAR(100) NOT NULL,
    LastName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(200) NOT NULL UNIQUE,
    Phone NVARCHAR(20) UNIQUE,
    LinkedInUrl NVARCHAR(500) NULL,
    Source NVARCHAR(100) NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);

-- Table: Application
CREATE TABLE Application (
    ApplicationId INT IDENTITY(1,1) PRIMARY KEY,
    CandidateId INT NOT NULL,
    RequisitionId INT NOT NULL,
    CurrentStage NVARCHAR(50) NOT NULL,
    Status NVARCHAR(50) NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    FOREIGN KEY (CandidateId) REFERENCES Candidate(CandidateId),
    FOREIGN KEY (RequisitionId) REFERENCES Requisition(RequisitionId)
);

-- Table: StageHistory
CREATE TABLE StageHistory (
    StageHistoryId INT IDENTITY(1,1) PRIMARY KEY,
    ApplicationId INT NOT NULL,
    FromStage NVARCHAR(50) NULL,
    ToStage NVARCHAR(50) NOT NULL,
    MovedBy NVARCHAR(100) NOT NULL,
    MovedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    FOREIGN KEY (ApplicationId) REFERENCES Application(ApplicationId)
);
