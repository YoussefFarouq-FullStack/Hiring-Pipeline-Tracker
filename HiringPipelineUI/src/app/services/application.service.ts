import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, retry } from 'rxjs/operators';
import { Application } from '../models/application.model';
import { Candidate } from '../models/candidate.model';
import { Requisition } from '../models/requisition.model';

@Injectable({
  providedIn: 'root'
})
export class ApplicationService {
  private apiUrl = '/api/applications';
  
  constructor(private http: HttpClient) {}

  private handleError(error: HttpErrorResponse) {
    let errorMessage = 'An error occurred';
    
    if (error.error instanceof ErrorEvent) {
      errorMessage = `Client Error: ${error.error.message}`;
      console.error('Client-side error:', error.error);
    } else {
      errorMessage = `Server Error: ${error.status} - ${error.message}`;
      console.error('Server-side error:', error);
      
      if (error.status === 0) {
        errorMessage = 'Unable to connect to server. Please check your internet connection.';
      } else if (error.status === 404) {
        errorMessage = 'Application not found.';
      } else if (error.status === 400) {
        // Try to extract validation errors from the response
        if (error.error && error.error.errors) {
          const validationErrors = Object.values(error.error.errors).flat();
          errorMessage = `Validation errors: ${validationErrors.join(', ')}`;
        } else {
          errorMessage = 'Invalid data provided. Please check your input.';
        }
      } else if (error.status >= 500) {
        errorMessage = 'Server error. Please try again later.';
      }
    }
    
    return throwError(() => new Error(errorMessage));
  }

  getApplications(): Observable<Application[]> {
    return this.http.get<Application[]>(this.apiUrl).pipe(
      retry(1),
      catchError(this.handleError)
    );
  }

  getApplicationsWithDetails(): Observable<Application[]> {
    return this.http.get<Application[]>(this.apiUrl).pipe(
      retry(1),
      catchError(this.handleError)
    );
  }

  getApplication(id: number): Observable<Application> {
    return this.http.get<Application>(`${this.apiUrl}/${id}`).pipe(
      retry(1),
      catchError(this.handleError)
    );
  }

  createApplication(application: Partial<Application>): Observable<Application> {
    return this.http.post<Application>(this.apiUrl, application).pipe(
      catchError(this.handleError)
    );
  }

  updateApplication(id: number, application: Partial<Application>): Observable<Application> {
    return this.http.put<Application>(`${this.apiUrl}/${id}`, application).pipe(
      catchError(this.handleError)
    );
  }

  deleteApplication(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`).pipe(
      catchError(this.handleError)
    );
  }

  // Helpers for dropdowns
  getCandidates(): Observable<Candidate[]> {
    return this.http.get<Candidate[]>('/api/candidates').pipe(
      catchError(this.handleError)
    );
  }

  getRequisitions(): Observable<Requisition[]> {
    return this.http.get<Requisition[]>('/api/requisitions').pipe(
      catchError(this.handleError)
    );
  }
}
