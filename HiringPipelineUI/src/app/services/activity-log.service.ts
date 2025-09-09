import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { AuthService } from './auth.service';

export interface ActivityLog {
  id: number;
  userId: number;
  userName: string;
  action: string;
  entityType: string;
  entityId: number;
  entityName: string;
  details: string;
  timestamp: string;
  ipAddress?: string;
  userAgent?: string;
}

export interface CreateActivityLogDto {
  action: string;
  entityType: string;
  entityId: number;
  entityName: string;
  details: string;
}

@Injectable({
  providedIn: 'root'
})
export class ActivityLogService {
  private apiUrl = '/api/activity-logs';

  constructor(
    private http: HttpClient,
    private authService: AuthService
  ) {}

  private handleError(error: HttpErrorResponse) {
    let errorMessage = 'An error occurred while logging activity';
    
    if (error.error instanceof ErrorEvent) {
      errorMessage = `Client Error: ${error.error.message}`;
      console.error('Client-side error:', error.error);
    } else {
      errorMessage = `Server Error: ${error.status} - ${error.message}`;
      console.error('Server-side error:', error);
      
      if (error.status === 0) {
        errorMessage = 'Unable to connect to server. Please check your internet connection.';
      } else if (error.status === 401) {
        errorMessage = 'Unauthorized access.';
      } else if (error.status === 403) {
        errorMessage = 'Access denied.';
      } else if (error.status === 404) {
        errorMessage = 'Activity log service not found.';
      }
    }

    return throwError(() => new Error(errorMessage));
  }

  // Log an activity
  logActivity(activityData: CreateActivityLogDto): Observable<ActivityLog> {
    return this.http.post<ActivityLog>(this.apiUrl, activityData).pipe(
      catchError(this.handleError)
    );
  }

  // Get activity logs for a specific entity
  getEntityActivityLogs(entityType: string, entityId: number): Observable<ActivityLog[]> {
    return this.http.get<ActivityLog[]>(`${this.apiUrl}/entity/${entityType}/${entityId}`).pipe(
      catchError(this.handleError)
    );
  }

  // Get recent activity logs
  getRecentActivityLogs(limit: number = 10): Observable<ActivityLog[]> {
    return this.http.get<ActivityLog[]>(`${this.apiUrl}/recent?limit=${limit}`).pipe(
      catchError(this.handleError)
    );
  }

  // Get user activity logs
  getUserActivityLogs(userId: number): Observable<ActivityLog[]> {
    return this.http.get<ActivityLog[]>(`${this.apiUrl}/user/${userId}`).pipe(
      catchError(this.handleError)
    );
  }

  // Helper method to log common actions
  logCreate(entityType: string, entityId: number, entityName: string, details?: string): void {
    const currentUser = this.authService.getCurrentUser();
    if (!currentUser) return;

    this.logActivity({
      action: 'CREATE',
      entityType,
      entityId,
      entityName,
      details: details || `Created new ${entityType.toLowerCase()}`
    }).subscribe({
      next: () => console.log(`Activity logged: Created ${entityType}`),
      error: (error) => console.error('Failed to log activity:', error)
    });
  }

  logUpdate(entityType: string, entityId: number, entityName: string, details?: string): void {
    const currentUser = this.authService.getCurrentUser();
    if (!currentUser) return;

    this.logActivity({
      action: 'UPDATE',
      entityType,
      entityId,
      entityName,
      details: details || `Updated ${entityType.toLowerCase()}`
    }).subscribe({
      next: () => console.log(`Activity logged: Updated ${entityType}`),
      error: (error) => console.error('Failed to log activity:', error)
    });
  }

  logDelete(entityType: string, entityId: number, entityName: string, details?: string): void {
    const currentUser = this.authService.getCurrentUser();
    if (!currentUser) return;

    this.logActivity({
      action: 'DELETE',
      entityType,
      entityId,
      entityName,
      details: details || `Deleted ${entityType.toLowerCase()}`
    }).subscribe({
      next: () => console.log(`Activity logged: Deleted ${entityType}`),
      error: (error) => console.error('Failed to log activity:', error)
    });
  }

  logStageChange(entityType: string, entityId: number, entityName: string, fromStage: string, toStage: string): void {
    const currentUser = this.authService.getCurrentUser();
    if (!currentUser) return;

    this.logActivity({
      action: 'STAGE_CHANGE',
      entityType,
      entityId,
      entityName,
      details: `Moved from ${fromStage} to ${toStage}`
    }).subscribe({
      next: () => console.log(`Activity logged: Stage change for ${entityType}`),
      error: (error) => console.error('Failed to log activity:', error)
    });
  }
}
