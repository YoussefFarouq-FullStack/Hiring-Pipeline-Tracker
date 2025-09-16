import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, retry } from 'rxjs/operators';
import { Requisition, CreateRequisitionRequest, UpdateRequisitionRequest } from '../models/requisition.model';

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

  // Method specifically for dashboard background calls
  getRequisitionsForDashboard(): Observable<Requisition[]> {
    const headers = { 'X-Dashboard-Background': 'true' };
    return this.http.get<Requisition[]>(this.apiUrl, { headers }).pipe(
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

  createRequisition(requisition: CreateRequisitionRequest): Observable<Requisition> {
    if (!requisition || !requisition.title || !requisition.department || !requisition.priority) {
      return throwError(() => new Error('Invalid requisition data. Title, department, and priority are required.'));
    }
    
    return this.http.post<Requisition>(this.apiUrl, requisition).pipe(
      catchError(this.handleError)
    );
  }

  updateRequisition(id: number, requisition: UpdateRequisitionRequest): Observable<Requisition> {
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

  // Helper method to get priority color for UI
  getPriorityColor(priority: string): string {
    switch (priority.toLowerCase()) {
      case 'high': return 'text-red-600 bg-red-100';
      case 'medium': return 'text-yellow-600 bg-yellow-100';
      case 'low': return 'text-green-600 bg-green-100';
      default: return 'text-gray-600 bg-gray-100';
    }
  }

  // Helper method to get draft status color for UI
  getDraftStatusColor(isDraft: boolean): string {
    return isDraft ? 'text-orange-600 bg-orange-100' : 'text-blue-600 bg-blue-100';
  }

  // Helper method to get status color for UI
  getStatusColor(status: string): string {
    switch (status.toLowerCase()) {
      case 'open': return 'text-green-600 bg-green-100';
      case 'on hold': return 'text-yellow-600 bg-yellow-100';
      case 'closed': return 'text-gray-600 bg-gray-100';
      case 'cancelled': return 'text-red-600 bg-red-100';
      default: return 'text-gray-600 bg-gray-100';
    }
  }
}
