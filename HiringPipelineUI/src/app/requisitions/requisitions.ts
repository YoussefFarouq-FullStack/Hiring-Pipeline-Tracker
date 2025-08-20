import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { RequisitionService } from '../services/requisition.service';
import { RequisitionDialogComponent } from './requisition-dialog/requisition-dialog';


@Component({
  selector: 'app-requisitions',
  standalone: true,
  imports: [CommonModule, MatTableModule, MatButtonModule, MatDialogModule],
  templateUrl: './requisitions.html',
  styleUrl: './requisitions.scss'
})
export class RequisitionsComponent implements OnInit {
  requisitions: any[] = [];
  displayedColumns: string[] = ['id', 'title', 'department', 'status', 'actions'];

  constructor(private requisitionService: RequisitionService, public dialog: MatDialog) {}

  ngOnInit(): void {
    this.loadRequisitions();
  }

  loadRequisitions() {
    this.requisitionService.getRequisitions().subscribe(data => {
      this.requisitions = data;
    });
  }
  addRequisition() {
    const dialogRef = this.dialog.open(RequisitionDialogComponent, {
      width: '400px'
    });
  
    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.requisitionService.createRequisition(result).subscribe(() => {
          this.loadRequisitions();
        });
      }
    });
  }

  deleteRequisition(id: number) {
    if (confirm("Are you sure you want to delete this requisition?")) {
      this.requisitionService.deleteRequisition(id).subscribe(() => {
        this.loadRequisitions();
      });
    }
  }
}
