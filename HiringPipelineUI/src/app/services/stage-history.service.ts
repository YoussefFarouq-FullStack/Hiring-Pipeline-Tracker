import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { StageHistory } from '../models/stage-history.model';

@Injectable({
  providedIn: 'root'
})
export class StageHistoryService {
  private apiUrl = 'https://localhost:5001/api/stagehistory';

  constructor(private http: HttpClient) {}

  getStageHistories(): Observable<StageHistory[]> {
    return this.http.get<StageHistory[]>(this.apiUrl);
  }

  getStageHistory(id: number): Observable<StageHistory> {
    return this.http.get<StageHistory>(`${this.apiUrl}/${id}`);
  }

  getByApplication(applicationId: number): Observable<StageHistory[]> {
    return this.http.get<StageHistory[]>(`${this.apiUrl}/application/${applicationId}`);
  }

  addStageHistory(stageHistory: StageHistory): Observable<StageHistory> {
    return this.http.post<StageHistory>(this.apiUrl, stageHistory);
  }

  deleteStageHistory(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}



