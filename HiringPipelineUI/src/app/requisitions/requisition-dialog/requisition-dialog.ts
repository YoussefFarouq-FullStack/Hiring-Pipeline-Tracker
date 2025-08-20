import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatDialogRef } from '@angular/material/dialog';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-requisition-dialog',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule, MatFormFieldModule, MatInputModule, MatButtonModule],
  templateUrl: './requisition-dialog.html',
  styleUrls: ['./requisition-dialog.scss']
})
export class RequisitionDialogComponent {
  requisitionForm: FormGroup;

  constructor(
    private fb: FormBuilder,
    private dialogRef: MatDialogRef<RequisitionDialogComponent>
  ) {
    this.requisitionForm = this.fb.group({
      title: ['', Validators.required],
      department: ['', Validators.required],
      status: ['Open', Validators.required]
    });
  }

  save() {
    if (this.requisitionForm.valid) {
      this.dialogRef.close(this.requisitionForm.value);
    }
  }

  cancel() {
    this.dialogRef.close();
  }
}
