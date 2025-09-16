import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface AuditLog {
  id: number;
  userId: number;
  username: string;
  userRole: string;
  ipAddress?: string;
  action: string;
  entity: string;
  entityId?: number;
  changes?: string;
  details?: string;
  timestamp: string;
  userAgent?: string;
  logType: string;
}

export interface AuditLogFilter {
  userId?: number;
  entity?: string;
  entityId?: number;
  action?: string;
  fromDate?: string;
  toDate?: string;
  logType?: string;
  skip?: number;
  take?: number;
}

export interface AuditLogResponse {
  auditLogs: AuditLog[];
  totalCount: number;
  skip: number;
  take: number;
}

@Injectable({
  providedIn: 'root'
})
export class AuditLogService {
  private apiUrl = '/api/auditlogs';

  constructor(private http: HttpClient) {}

  getAuditLogs(filter?: AuditLogFilter): Observable<AuditLogResponse> {
    let params = new HttpParams();
    
    if (filter) {
      if (filter.userId) params = params.set('userId', filter.userId.toString());
      if (filter.entity) params = params.set('entity', filter.entity);
      if (filter.entityId) params = params.set('entityId', filter.entityId.toString());
      if (filter.action) params = params.set('action', filter.action);
      if (filter.fromDate) params = params.set('fromDate', filter.fromDate);
      if (filter.toDate) params = params.set('toDate', filter.toDate);
      if (filter.logType) params = params.set('logType', filter.logType);
      if (filter.skip) params = params.set('skip', filter.skip.toString());
      if (filter.take) params = params.set('take', filter.take.toString());
    }

    // Add cache-busting parameter to prevent browser caching
    params = params.set('_t', Date.now().toString());

    console.log('API Request URL:', this.apiUrl);
    console.log('API Request Params:', params.toString());

    return this.http.get<AuditLogResponse>(this.apiUrl, { params });
  }

  getEntityAuditLogs(entity: string, entityId: number): Observable<AuditLog[]> {
    return this.http.get<AuditLog[]>(`${this.apiUrl}/entity/${entity}/${entityId}`);
  }

  getUserAuditLogs(userId: number, skip: number = 0, take: number = 100): Observable<AuditLogResponse> {
    let params = new HttpParams()
      .set('skip', skip.toString())
      .set('take', take.toString());

    return this.http.get<AuditLogResponse>(`${this.apiUrl}/user/${userId}`, { params });
  }

  clearAllAuditLogs(): Observable<any> {
    return this.http.delete(`${this.apiUrl}/clear`);
  }

  // Log a user action explicitly (for frontend-initiated actions)
  logUserAction(action: string, entity: string, entityId?: number, details?: string): Observable<any> {
    const logData = {
      action,
      entity,
      entityId,
      details,
      logType: 'UserAction'
    };
    
    return this.http.post(`${this.apiUrl}/log`, logData);
  }
}
