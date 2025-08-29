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
      errorMessage = `Server Error: ${error.status} - ${error.message}`;
      console.error('Server-side error:', error);
      
      if (error.status === 0) {
        errorMessage = 'Unable to connect to server. Please check your internet connection.';
      } else if (error.status === 404) {
        errorMessage = 'Stage history not found.';
      } else if (error.status === 400) {
        errorMessage = 'Invalid data provided. Please check your input.';
      } else if (error.status >= 500) {
        errorMessage = 'Server error. Please try again later.';
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



