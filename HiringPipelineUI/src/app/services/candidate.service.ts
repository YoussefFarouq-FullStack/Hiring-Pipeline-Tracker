import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, retry } from 'rxjs/operators';
import { Candidate } from '../models/candidate.model';

@Injectable({
  providedIn: 'root'
})
export class CandidateService {
  private apiUrl = '/api/candidates';
  
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
        errorMessage = 'Candidate not found.';
      } else if (error.status === 400) {
        errorMessage = 'Invalid data provided. Please check your input.';
      } else if (error.status === 409) {
        errorMessage = 'Candidate already exists with this email.';
      } else if (error.status >= 500) {
        errorMessage = 'Server error. Please try again later.';
      }
    }
    
    return throwError(() => new Error(errorMessage));
  }

  getCandidates(): Observable<Candidate[]> {
    return this.http.get<Candidate[]>(this.apiUrl).pipe(
      retry(1),
      catchError(this.handleError)
    );
  }

  // Method specifically for dashboard background calls
  getCandidatesForDashboard(): Observable<Candidate[]> {
    const headers = { 'X-Dashboard-Background': 'true' };
    return this.http.get<Candidate[]>(this.apiUrl, { headers }).pipe(
      retry(1),
      catchError(this.handleError)
    );
  }

  getCandidate(id: number): Observable<Candidate> {
    return this.http.get<Candidate>(`${this.apiUrl}/${id}`).pipe(
      retry(1),
      catchError(this.handleError)
    );
  }

  createCandidate(candidate: Omit<Candidate, 'candidateId'>): Observable<Candidate> {
    return this.http.post<Candidate>(this.apiUrl, candidate).pipe(
      catchError(this.handleError)
    );
  }

  updateCandidate(id: number, candidate: Candidate): Observable<Candidate> {
    console.log('Service: Updating candidate', id, 'with data:', candidate);
    console.log('Service: Status being sent:', candidate.status);
    console.log('Service: Full candidate object being sent:', JSON.stringify(candidate, null, 2));
    
    // Ensure status is included
    if (!candidate.status || candidate.status === '') {
      console.warn('Service: Warning - No status provided for update, setting default to "Applied"');
      candidate.status = 'Applied';
    }
    
    return this.http.put<Candidate>(`${this.apiUrl}/${id}`, candidate).pipe(
      catchError(this.handleError)
    );
  }

  deleteCandidate(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`).pipe(
      catchError(this.handleError)
    );
  }
}
