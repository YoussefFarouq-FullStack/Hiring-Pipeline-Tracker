import { Component, Inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import { Candidate } from '../../../models/candidate.model';
import { Application } from '../../../models/application.model';
import { Requisition } from '../../../models/requisition.model';

export interface HiredCandidateData {
  candidate: Candidate;
  application: Application;
  requisition: Requisition;
  hiredDate: string;
}

@Component({
  selector: 'app-hired-candidates-modal',
  standalone: true,
  imports: [CommonModule, MatDialogModule],
  templateUrl: './hired-candidates-modal.component.html',
  styleUrls: ['./hired-candidates-modal.component.scss']
})
export class HiredCandidatesModalComponent implements OnInit {
  hiredCandidates: HiredCandidateData[] = [];
  selectedCandidate: HiredCandidateData | null = null;
  isLoading = true;

  constructor(
    public dialogRef: MatDialogRef<HiredCandidatesModalComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { hiredCandidates: HiredCandidateData[] }
  ) {}

  ngOnInit(): void {
    this.hiredCandidates = this.data.hiredCandidates || [];
    this.isLoading = false;
  }

  selectCandidate(candidate: HiredCandidateData): void {
    this.selectedCandidate = this.selectedCandidate === candidate ? null : candidate;
  }

  closeModal(): void {
    this.dialogRef.close();
  }

  getTimeAgo(dateString: string): string {
    try {
      if (!dateString) return 'Unknown date';
      const date = new Date(dateString);
      if (isNaN(date.getTime())) return 'Invalid date';
      
      const now = new Date();
      const diffInSeconds = Math.floor((now.getTime() - date.getTime()) / 1000);
      
      if (diffInSeconds < 60) return 'Just now';
      if (diffInSeconds < 3600) return `${Math.floor(diffInSeconds / 60)}m ago`;
      if (diffInSeconds < 86400) return `${Math.floor(diffInSeconds / 3600)}h ago`;
      if (diffInSeconds < 2592000) return `${Math.floor(diffInSeconds / 86400)}d ago`;
      if (diffInSeconds < 31536000) return `${Math.floor(diffInSeconds / 2592000)}mo ago`;
      return `${Math.floor(diffInSeconds / 31536000)}y ago`;
    } catch (error) {
      console.error('Error in getTimeAgo:', error);
      return 'Unknown date';
    }
  }

  getCandidateInitials(firstName: string, lastName: string): string {
    return `${firstName.charAt(0)}${lastName.charAt(0)}`.toUpperCase();
  }

  trackByCandidate(index: number, item: HiredCandidateData): number {
    return item.candidate.candidateId;
  }
}
