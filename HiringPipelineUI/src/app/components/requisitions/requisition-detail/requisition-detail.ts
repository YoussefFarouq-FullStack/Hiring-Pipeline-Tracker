import { Component, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { Requisition } from '../../../models/requisition.model';

interface DialogData {
  requisition: Requisition;
}

@Component({
  selector: 'app-requisition-detail',
  standalone: true,
  imports: [CommonModule, MatButtonModule, MatIconModule],
  templateUrl: './requisition-detail.html',
  styleUrls: ['./requisition-detail.scss']
})
export class RequisitionDetailComponent {
  constructor(
    private dialogRef: MatDialogRef<RequisitionDetailComponent>,
    @Inject(MAT_DIALOG_DATA) public data: DialogData
  ) {}

  close(): void {
    this.dialogRef.close();
  }

  getPriorityColor(priority: string): string {
    switch (priority.toLowerCase()) {
      case 'high': return 'text-red-600 bg-red-100';
      case 'medium': return 'text-yellow-600 bg-yellow-100';
      case 'low': return 'text-green-600 bg-green-100';
      default: return 'text-gray-600 bg-gray-100';
    }
  }

  getDraftStatusColor(isDraft: boolean): string {
    return isDraft ? 'text-orange-600 bg-orange-100' : 'text-blue-600 bg-blue-100';
  }

  getStatusColor(status: string): string {
    switch (status.toLowerCase()) {
      case 'open': return 'text-green-600 bg-green-100';
      case 'on hold': return 'text-yellow-600 bg-yellow-100';
      case 'closed': return 'text-gray-600 bg-gray-100';
      case 'cancelled': return 'text-red-600 bg-red-100';
      default: return 'text-gray-600 bg-gray-100';
    }
  }

  formatDate(dateString: string): string {
    const date = new Date(dateString);
    return date.toLocaleDateString('en-US', { 
      year: 'numeric', 
      month: 'long', 
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  }

  getApplicationCount(requisitionId: number): number {
    // TODO: Replace with actual backend application count
    return 0; // Placeholder until backend provides actual count
  }
}
