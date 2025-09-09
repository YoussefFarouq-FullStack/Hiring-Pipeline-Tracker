-- Add Resume and Skills columns to Candidates table
ALTER TABLE [Candidates] 
ADD [Resume] nvarchar(max) NULL;

ALTER TABLE [Candidates] 
ADD [Skills] nvarchar(max) NULL;

-- Remove old columns if they exist
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Candidates' AND COLUMN_NAME = 'LinkedInUrl')
BEGIN
    ALTER TABLE [Candidates] DROP COLUMN [LinkedInUrl];
END

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Candidates' AND COLUMN_NAME = 'Source')
BEGIN
    ALTER TABLE [Candidates] DROP COLUMN [Source];
END

