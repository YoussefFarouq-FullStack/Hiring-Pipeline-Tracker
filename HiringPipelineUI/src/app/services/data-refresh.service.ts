import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';

export interface DataRefreshEvent {
  type: 'audit-logs-cleared' | 'hiring-data-cleared' | 'table-cleared';
  tableName?: string;
  tableNames?: string[];
}

@Injectable({
  providedIn: 'root'
})
export class DataRefreshService {
  private refreshSubject = new Subject<DataRefreshEvent>();

  // Observable for components to subscribe to
  refresh$ = this.refreshSubject.asObservable();

  // Method to emit refresh events
  emitRefresh(event: DataRefreshEvent): void {
    this.refreshSubject.next(event);
  }

  // Convenience methods for specific events
  notifyAuditLogsCleared(): void {
    this.emitRefresh({ type: 'audit-logs-cleared' });
  }

  notifyHiringDataCleared(): void {
    this.emitRefresh({ type: 'hiring-data-cleared' });
  }

  notifyTableCleared(tableName: string): void {
    this.emitRefresh({ type: 'table-cleared', tableName });
  }

  notifyTablesCleared(tableNames: string[]): void {
    this.emitRefresh({ type: 'table-cleared', tableNames });
  }
}
