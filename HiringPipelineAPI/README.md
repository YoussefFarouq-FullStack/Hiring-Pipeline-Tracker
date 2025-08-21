# Hiring Pipeline API

A .NET Web API for managing hiring pipeline data including candidates, requisitions, applications, and stage history.

## Features

- **Candidates Management**: CRUD operations for candidate information with status tracking
- **Requisitions Management**: CRUD operations for job requisitions
- **Applications Tracking**: Track candidate applications through the hiring pipeline
- **Stage History**: Maintain audit trail of application stage changes
- **Identity Reset**: Automatic and manual reset of database identity seeds for ALL entities

## Identity Management

The API now includes comprehensive automatic identity seed reset functionality to ensure IDs start from 1 when there are no existing records in ANY table.

### Automatic Reset

**During Creation**: When creating the first record in any table, the identity seed is automatically reset to 0, ensuring the new record gets ID = 1.

**During Deletion**: When deleting the last record from any table, the identity seed is automatically reset to 0.

### Manual Reset

You can manually reset all identity seeds using the health endpoint:

```http
POST /health/reset-identity-seeds
```

### Delete All Records

To delete all records and reset the identity seed for each entity:

```http
DELETE /api/candidate/delete-all
DELETE /api/requisitions/delete-all
DELETE /api/application/delete-all
DELETE /api/stagehistory/delete-all
```

## Data Models

### Candidate Status

Candidates now have a `Status` field that tracks their position in the hiring pipeline, using the same string-based approach as requisitions:

```csharp
public string Status { get; set; } = "Applied";
```

**Available Status Values:**
- `"Applied"` - Initial application received (default)
- `"Screening"` - Under review/initial screening
- `"Interview"` - Interview scheduled/completed
- `"TechnicalAssessment"` - Technical skills evaluation
- `"ReferenceCheck"` - Reference verification
- `"Offer"` - Job offer extended
- `"Hired"` - Successfully hired
- `"Rejected"` - Application rejected
- `"Withdrawn"` - Candidate withdrew

### Requisition Status

Requisitions use a simple string-based status:

```csharp
public string Status { get; set; } = "Open";
```

**Available Status Values:**
- `"Open"` - Position is open for applications
- `"Closed"` - Position is closed

## API Endpoints

### Health
- `GET /health` - Check API status
- `POST /health/reset-identity-seeds` - Reset all identity seeds

### Candidates
- `GET /api/candidate` - Get all candidates
- `GET /api/candidate/{id}` - Get candidate by ID
- `POST /api/candidate` - Create new candidate
- `PUT /api/candidate/{id}` - Update candidate
- `DELETE /api/candidate/{id}` - Delete candidate
- `DELETE /api/candidate/delete-all` - Delete all candidates and reset seed

**Candidate Creation Example:**
```json
{
  "firstName": "John",
  "lastName": "Doe",
  "email": "john.doe@example.com",
  "phone": "1234567890",
  "status": "Applied"
}
```

### Requisitions
- `GET /api/requisitions` - Get all requisitions
- `GET /api/requisitions/{id}` - Get requisition by ID
- `POST /api/requisitions` - Create new requisition
- `PUT /api/requisitions/{id}` - Update requisition
- `DELETE /api/requisitions/{id}` - Delete requisition
- `DELETE /api/requisitions/delete-all` - Delete all requisitions and reset seed

### Applications
- `GET /api/application` - Get all applications
- `GET /api/application/{id}` - Get application by ID
- `POST /api/application` - Create new application
- `PUT /api/application/{id}` - Update application
- `DELETE /api/application/{id}` - Delete application
- `DELETE /api/application/delete-all` - Delete all applications and reset seed

### Stage Histories
- `GET /api/stagehistory` - Get all stage histories
- `GET /api/stagehistory/{id}` - Get stage history by ID
- `GET /api/stagehistory/application/{applicationId}` - Get history for specific application
- `POST /api/stagehistory` - Create new stage history
- `DELETE /api/stagehistory/{id}` - Delete stage history
- `DELETE /api/stagehistory/delete-all` - Delete all stage histories and reset seed

## Database

The API uses Entity Framework Core with SQL Server. Identity columns are configured with `IDENTITY(1, 1)` and are automatically reset when:
- Creating the first record in an empty table
- Deleting the last record from a table
- Manually calling the reset endpoint

## Testing

Use the provided HTTP files to test the API endpoints:

- `HiringPipelineAPI.http` - Basic CRUD operations and identity reset
- `test-complete-reset.http` - Comprehensive testing workflow for all entities
- `test-identity-reset.http` - Focused identity reset testing

## Example Workflow

1. **Check current state**: All tables have existing records with IDs > 1
2. **Delete all records**: Use delete-all endpoints (in correct order due to foreign keys)
3. **Create new records**: All new records will get ID = 1
4. **Verify**: Frontend displays sequential IDs starting from 1

## Foreign Key Considerations

When deleting all records, follow this order to avoid constraint violations:
1. Stage Histories (depends on Applications)
2. Applications (depends on Candidates and Requisitions)
3. Candidates and Requisitions (independent)

## Benefits

- **Frontend Consistency**: IDs always start from 1 when tables are empty
- **User Experience**: Sequential numbering makes data easier to understand
- **Status Tracking**: Comprehensive candidate status management using string-based approach
- **Code Consistency**: Both candidates and requisitions use the same status pattern
- **Testing**: Clean slate for development and testing scenarios
- **Maintenance**: Easy to reset all data and start fresh
