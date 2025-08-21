import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, retry } from 'rxjs/operators';
import { Requisition } from '../models/requisition.model';

@Injectable({
  providedIn: 'root'
})
export class RequisitionService {
  private apiUrl = '/api/requisitions';
  
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
        errorMessage = 'Requisition not found.';
      } else if (error.status === 400) {
        errorMessage = 'Invalid data provided. Please check your input.';
      } else if (error.status === 409) {
        errorMessage = 'Requisition already exists with this title.';
      } else if (error.status >= 500) {
        errorMessage = 'Server error. Please try again later.';
      }
    }
    
    return throwError(() => new Error(errorMessage));
  }

  getRequisitions(): Observable<Requisition[]> {
    return this.http.get<Requisition[]>(this.apiUrl).pipe(
      retry(1),
      catchError(this.handleError)
    );
  }

  getRequisition(id: number): Observable<Requisition> {
    return this.http.get<Requisition>(`${this.apiUrl}/${id}`).pipe(
      retry(1),
      catchError(this.handleError)
    );
  }

  createRequisition(requisition: Omit<Requisition, 'requisitionId'>): Observable<Requisition> {
    if (!requisition || !requisition.title || !requisition.department || !requisition.status) {
      return throwError(() => new Error('Invalid requisition data. Title, department, and status are required.'));
    }
    
    return this.http.post<Requisition>(this.apiUrl, requisition).pipe(
      catchError(this.handleError)
    );
  }

  updateRequisition(id: number, requisition: Partial<Requisition>): Observable<Requisition> {
    if (!id || isNaN(id)) {
      return throwError(() => new Error('Invalid requisition ID.'));
    }
    
    if (!requisition || Object.keys(requisition).length === 0) {
      return throwError(() => new Error('No update data provided.'));
    }
    
    return this.http.put<Requisition>(`${this.apiUrl}/${id}`, requisition).pipe(
      catchError(this.handleError)
    );
  }

  deleteRequisition(id: number): Observable<void> {
    if (!id || isNaN(id)) {
      return throwError(() => new Error('Invalid requisition ID.'));
    }
    
    return this.http.delete<void>(`${this.apiUrl}/${id}`).pipe(
      catchError(this.handleError)
    );
  }
}
