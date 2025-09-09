import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';

export interface FileUploadResult {
  fileName: string;
  filePath: string;
  fileSize: number;
  contentType: string;
}

@Injectable({
  providedIn: 'root'
})
export class FileUploadService {
  private apiUrl = '/api/fileupload';
  
  constructor(private http: HttpClient) {}

  private handleError(error: HttpErrorResponse) {
    let errorMessage = 'An error occurred';
    
    if (error.error instanceof ErrorEvent) {
      errorMessage = `Client Error: ${error.error.message}`;
    } else {
      errorMessage = `Server Error: ${error.status} - ${error.message}`;
      
      if (error.status === 0) {
        errorMessage = 'Unable to connect to server. Please check your internet connection.';
      } else if (error.status === 400) {
        errorMessage = error.error?.message || 'Invalid file. Please check file type and size.';
      } else if (error.status === 413) {
        errorMessage = 'File too large. Please select a smaller file.';
      } else if (error.status >= 500) {
        errorMessage = 'Server error. Please try again later.';
      }
    }
    
    return throwError(() => new Error(errorMessage));
  }

  uploadResume(file: File): Observable<FileUploadResult> {
    const formData = new FormData();
    formData.append('file', file);

    return this.http.post<FileUploadResult>(`${this.apiUrl}/resume`, formData).pipe(
      catchError(this.handleError)
    );
  }

  downloadResume(filePath: string): Observable<Blob> {
    return this.http.get(`${this.apiUrl}/resume/${filePath}`, {
      responseType: 'blob'
    }).pipe(
      catchError(this.handleError)
    );
  }
}
