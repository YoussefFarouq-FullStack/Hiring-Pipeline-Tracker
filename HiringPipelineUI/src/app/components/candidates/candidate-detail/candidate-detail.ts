import { Component, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { Candidate } from '../../../models/candidate.model';

interface DialogData {
  candidate: Candidate;
}

@Component({
  selector: 'app-candidate-detail',
  standalone: true,
  imports: [CommonModule, MatButtonModule, MatIconModule],
  templateUrl: './candidate-detail.html',
  styleUrls: ['./candidate-detail.scss']
})
export class CandidateDetailComponent {
  constructor(
    private dialogRef: MatDialogRef<CandidateDetailComponent>,
    @Inject(MAT_DIALOG_DATA) public data: DialogData
  ) {}

  close(): void {
    this.dialogRef.close();
  }

  getStatusColor(status: string): string {
    switch (status.toLowerCase()) {
      case 'active': return 'text-green-600 bg-green-100';
      case 'inactive': return 'text-gray-600 bg-gray-100';
      case 'hired': return 'text-blue-600 bg-blue-100';
      case 'rejected': return 'text-red-600 bg-red-100';
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

  getApplicationCount(candidateId: number): number {
    // TODO: Replace with actual backend application count
    return 0; // Placeholder until backend provides actual count
  }

  getSkillsList(skills: string): string[] {
    if (!skills) return [];
    
    // Split by comma, semicolon, or newline and clean up
    return skills
      .split(/[,;\n]/)
      .map(skill => skill.trim())
      .filter(skill => skill.length > 0);
  }
}
