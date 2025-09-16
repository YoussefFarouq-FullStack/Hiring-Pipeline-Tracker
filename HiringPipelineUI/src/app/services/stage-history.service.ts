import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, retry } from 'rxjs/operators';
import { StageHistory, CreateStageHistoryDto } from '../models/stage-history.model';

@Injectable({
  providedIn: 'root'
})
export class StageHistoryService {
  private apiUrl = '/api/stagehistory';
  
  constructor(private http: HttpClient) {}

  private handleError(error: HttpErrorResponse) {
    let errorMessage = 'An error occurred';
    
    if (error.error instanceof ErrorEvent) {
      // Client-side error
      errorMessage = `Client Error: ${error.error.message}`;
      console.error('Client-side error:', error.error);
    } else {
      // Server-side error
      console.error('Server-side error:', error);
      console.error('Error status:', error.status);
      console.error('Error message:', error.message);
      console.error('Error body:', error.error);
      
      if (error.status === 0) {
        errorMessage = 'Unable to connect to server. Please check your internet connection.';
      } else if (error.status === 404) {
        errorMessage = 'Stage history not found.';
      } else if (error.status === 400) {
        // Try to extract more specific error message from response
        if (error.error && typeof error.error === 'object') {
          if (error.error.message) {
            errorMessage = error.error.message;
          } else if (error.error.errors) {
            errorMessage = `Validation errors: ${JSON.stringify(error.error.errors)}`;
          } else {
            errorMessage = `Bad Request: ${JSON.stringify(error.error)}`;
          }
        } else if (error.error && typeof error.error === 'string') {
          errorMessage = error.error;
        } else {
          errorMessage = 'Invalid data provided. Please check your input.';
        }
      } else if (error.status === 401) {
        errorMessage = 'Unauthorized. Please log in again.';
      } else if (error.status === 403) {
        errorMessage = 'Access denied. You do not have permission to perform this action.';
      } else if (error.status >= 500) {
        errorMessage = 'Server error. Please try again later.';
      } else {
        errorMessage = `Server Error: ${error.status} - ${error.message}`;
      }
    }
    
    return throwError(() => new Error(errorMessage));
  }

  getStageHistory(): Observable<StageHistory[]> {
    return this.http.get<StageHistory[]>(this.apiUrl).pipe(
      retry(1),
      catchError(this.handleError)
    );
  }

  // Method specifically for dashboard background calls
  getStageHistoryForDashboard(): Observable<StageHistory[]> {
    const headers = { 'X-Dashboard-Background': 'true' };
    return this.http.get<StageHistory[]>(this.apiUrl, { headers }).pipe(
      retry(1),
      catchError(this.handleError)
    );
  }

  getStageHistoryById(id: number): Observable<StageHistory> {
    return this.http.get<StageHistory>(`${this.apiUrl}/${id}`).pipe(
      retry(1),
      catchError(this.handleError)
    );
  }

  getStageHistoryByApplication(applicationId: number): Observable<StageHistory[]> {
    return this.http.get<StageHistory[]>(`${this.apiUrl}/application/${applicationId}`).pipe(
      retry(1),
      catchError(this.handleError)
    );
  }

  addStageHistory(stageHistory: CreateStageHistoryDto): Observable<StageHistory> {
    console.log('StageHistoryService.addStageHistory called with:', stageHistory);
    console.log('Request URL:', this.apiUrl);
    console.log('Request method: POST');
    
    // Validate the data before sending
    if (!stageHistory.applicationId || stageHistory.applicationId <= 0) {
      console.error('Invalid applicationId:', stageHistory.applicationId);
      return throwError(() => new Error('Invalid application ID'));
    }
    
    if (!stageHistory.toStage || stageHistory.toStage.trim() === '') {
      console.error('Invalid toStage:', stageHistory.toStage);
      return throwError(() => new Error('To stage is required'));
    }
    
    if (!stageHistory.movedBy || stageHistory.movedBy.trim() === '') {
      console.error('Invalid movedBy:', stageHistory.movedBy);
      return throwError(() => new Error('Moved by is required'));
    }
    
    // Validate movedBy format (only letters, spaces, hyphens, and periods)
    const movedByPattern = /^[a-zA-Z\s\-\.]+$/;
    if (!movedByPattern.test(stageHistory.movedBy.trim())) {
      console.error('Invalid movedBy format:', stageHistory.movedBy);
      return throwError(() => new Error('Moved by can only contain letters, spaces, hyphens, and periods'));
    }
    
    return this.http.post<StageHistory>(this.apiUrl, stageHistory).pipe(
      catchError(this.handleError)
    );
  }

  updateStageHistory(id: number, stageHistory: StageHistory): Observable<StageHistory> {
    return this.http.put<StageHistory>(`${this.apiUrl}/${id}`, stageHistory).pipe(
      catchError(this.handleError)
    );
  }

  deleteStageHistory(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`).pipe(
      catchError(this.handleError)
    );
  }
}



