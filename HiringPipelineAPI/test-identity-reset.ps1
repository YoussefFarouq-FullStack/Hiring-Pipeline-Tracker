# PowerShell Script to Test Identity Reset Functionality
# This script will test all the new endpoints to ensure IDs start from 1

param(
    [string]$BaseUrl = "http://localhost:5192"
)

Write-Host "🧪 Testing Identity Reset Functionality" -ForegroundColor Green
Write-Host "=====================================" -ForegroundColor Green
Write-Host ""

# Function to make HTTP requests
function Invoke-ApiRequest {
    param(
        [string]$Method,
        [string]$Endpoint,
        [string]$Body = $null
    )
    
    $headers = @{
        "Content-Type" = "application/json"
    }
    
    try {
        if ($Body) {
            $response = Invoke-RestMethod -Uri "$BaseUrl$Endpoint" -Method $Method -Headers $headers -Body $Body
        } else {
            $response = Invoke-RestMethod -Uri "$BaseUrl$Endpoint" -Method $Method -Headers $headers
        }
        return $response
    }
    catch {
        Write-Host "❌ Error calling $Method $Endpoint`: $($_.Exception.Message)" -ForegroundColor Red
        return $null
    }
}

# Function to display results
function Show-Results {
    param(
        [string]$Entity,
        [object]$Data
    )
    
    Write-Host "📊 $Entity:" -ForegroundColor Cyan
    if ($Data -and $Data.Count -gt 0) {
        foreach ($item in $Data) {
            $idProperty = $item.PSObject.Properties | Where-Object { $_.Name -like "*Id" } | Select-Object -First 1
            if ($idProperty) {
                Write-Host "   ID: $($idProperty.Value)" -ForegroundColor Yellow
            }
        }
    } else {
        Write-Host "   No records found" -ForegroundColor Gray
    }
    Write-Host ""
}

# Phase 1: Check Current State
Write-Host "🔍 Phase 1: Checking Current State" -ForegroundColor Blue
Write-Host "--------------------------------" -ForegroundColor Blue

$candidates = Invoke-ApiRequest -Method "GET" -Endpoint "/api/candidate"
$requisitions = Invoke-ApiRequest -Method "GET" -Endpoint "/api/requisitions"
$applications = Invoke-ApiRequest -Method "GET" -Endpoint "/api/application"
$stageHistories = Invoke-ApiRequest -Method "GET" -Endpoint "/api/stagehistory"

Show-Results -Entity "Candidates" -Data $candidates
Show-Results -Entity "Requisitions" -Data $requisitions
Show-Results -Entity "Applications" -Data $applications
Show-Results -Entity "Stage Histories" -Data $stageHistories

# Phase 2: Delete All Records
Write-Host "🗑️  Phase 2: Deleting All Records" -ForegroundColor Blue
Write-Host "-------------------------------" -ForegroundColor Blue

Write-Host "Deleting stage histories..." -ForegroundColor Yellow
Invoke-ApiRequest -Method "DELETE" -Endpoint "/api/stagehistory/delete-all"

Write-Host "Deleting applications..." -ForegroundColor Yellow
Invoke-ApiRequest -Method "DELETE" -Endpoint "/api/application/delete-all"

Write-Host "Deleting candidates..." -ForegroundColor Yellow
Invoke-ApiRequest -Method "DELETE" -Endpoint "/api/candidate/delete-all"

Write-Host "Deleting requisitions..." -ForegroundColor Yellow
Invoke-ApiRequest -Method "DELETE" -Endpoint "/api/requisitions/delete-all"

Write-Host "✅ All records deleted" -ForegroundColor Green
Write-Host ""

# Phase 3: Create New Records
Write-Host "✨ Phase 3: Creating New Records" -ForegroundColor Blue
Write-Host "-------------------------------" -ForegroundColor Blue

Write-Host "Creating candidate..." -ForegroundColor Yellow
$newCandidate = Invoke-ApiRequest -Method "POST" -Endpoint "/api/candidate" -Body '{"firstName":"John","lastName":"Doe","email":"john.doe@example.com","phone":"1234567890","status":"Applied"}'

Write-Host "Creating requisition..." -ForegroundColor Yellow
$newRequisition = Invoke-ApiRequest -Method "POST" -Endpoint "/api/requisitions" -Body '{"title":"Software Developer","department":"Engineering","jobLevel":"Mid-Level","status":"Open"}'

Write-Host "Creating application..." -ForegroundColor Yellow
$newApplication = Invoke-ApiRequest -Method "POST" -Endpoint "/api/application" -Body '{"candidateId":1,"requisitionId":1,"currentStage":"Applied","status":"Active"}'

Write-Host "Creating stage history..." -ForegroundColor Yellow
$newStageHistory = Invoke-ApiRequest -Method "POST" -Endpoint "/api/stagehistory" -Body '{"applicationId":1,"fromStage":null,"toStage":"Applied","movedBy":"System","movedAt":"2025-08-21T09:00:00Z"}'

Write-Host "✅ New records created" -ForegroundColor Green
Write-Host ""

# Phase 4: Verify Results
Write-Host "🔍 Phase 4: Verifying Results" -ForegroundColor Blue
Write-Host "----------------------------" -ForegroundColor Blue

$finalCandidates = Invoke-ApiRequest -Method "GET" -Endpoint "/api/candidate"
$finalRequisitions = Invoke-ApiRequest -Method "GET" -Endpoint "/api/requisitions"
$finalApplications = Invoke-ApiRequest -Method "GET" -Endpoint "/api/application"
$finalStageHistories = Invoke-ApiRequest -Method "GET" -Endpoint "/api/stagehistory"

Show-Results -Entity "Final Candidates" -Data $finalCandidates
Show-Results -Entity "Final Requisitions" -Data $finalRequisitions
Show-Results -Entity "Final Applications" -Data $finalApplications
Show-Results -Entity "Final Stage Histories" -Data $finalStageHistories

# Phase 5: Test Manual Reset
Write-Host "🔧 Phase 5: Testing Manual Reset" -ForegroundColor Blue
Write-Host "--------------------------------" -ForegroundColor Blue

Write-Host "Resetting all identity seeds manually..." -ForegroundColor Yellow
$resetResult = Invoke-ApiRequest -Method "POST" -Endpoint "/health/reset-identity-seeds"

if ($resetResult) {
    Write-Host "✅ Manual reset successful: $resetResult" -ForegroundColor Green
} else {
    Write-Host "❌ Manual reset failed" -ForegroundColor Red
}

Write-Host ""
Write-Host "🎉 Test Complete!" -ForegroundColor Green
Write-Host "All new records should now have IDs starting from 1." -ForegroundColor Green
