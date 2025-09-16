import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface TableCounts {
  [key: string]: number;
}

export interface SeedDataRequest {
  candidatesCount: number;
  requisitionsCount: number;
  applicationsCount: number;
  stageHistoryCount: number;
  createUsers: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class DatabaseService {
  private apiUrl = '/api/database';

  constructor(private http: HttpClient) { }

  /**
   * Gets the count of records in each table
   */
  getTableCounts(): Observable<TableCounts> {
    return this.http.get<TableCounts>(`${this.apiUrl}/counts`);
  }

  /**
   * Clears all hiring pipeline data while preserving user accounts and system data
   */
  clearHiringPipelineData(): Observable<any> {
    return this.http.post(`${this.apiUrl}/clear-hiring-data`, {});
  }

  /**
   * Clears selected tables from the database
   */
  clearSelectedTables(tableNames: string[]): Observable<any> {
    return this.http.post(`${this.apiUrl}/clear-selected`, { tableNames });
  }

  /**
   * Seeds the database with initial test data
   */
  seedDatabase(): Observable<any> {
    return this.http.post(`${this.apiUrl}/seed`, {});
  }

  /**
   * Seeds the database with customizable amounts of test data
   */
  seedDatabaseCustom(request: SeedDataRequest): Observable<any> {
    return this.http.post(`${this.apiUrl}/seed-custom`, request);
  }
}
