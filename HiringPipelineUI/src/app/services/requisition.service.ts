import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Requisition } from '../models/requisition.model';

@Injectable({
  providedIn: 'root'
})
export class RequisitionService {
  private apiUrl = 'https://localhost:5001/api/requisition';

  constructor(private http: HttpClient) {}

  getRequisitions(): Observable<Requisition[]> {
    return this.http.get<Requisition[]>(this.apiUrl);
  }

  getRequisition(id: number): Observable<Requisition> {
    return this.http.get<Requisition>(`${this.apiUrl}/${id}`);
  }

  addRequisition(requisition: Requisition): Observable<Requisition> {
    return this.http.post<Requisition>(this.apiUrl, requisition);
  }

  deleteRequisition(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}



