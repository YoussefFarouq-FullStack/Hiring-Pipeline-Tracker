import { Injectable } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { Observable } from 'rxjs';
import { ConfirmationDialogComponent, ConfirmationDialogData } from './confirmation-dialog.component';

@Injectable({
  providedIn: 'root'
})
export class ConfirmationDialogService {
  constructor(private dialog: MatDialog) {}

  /**
   * Show a confirmation dialog
   * @param data Dialog configuration
   * @returns Observable<boolean> - true if confirmed, false if cancelled
   */
  confirm(data: ConfirmationDialogData): Observable<boolean> {
    const dialogRef = this.dialog.open(ConfirmationDialogComponent, {
      data,
      width: '500px',
      maxWidth: '90vw',
      disableClose: true,
      panelClass: 'confirmation-dialog-container'
    });

    return dialogRef.afterClosed();
  }

  /**
   * Show a danger confirmation dialog (for deletions)
   * @param title Dialog title
   * @param message Dialog message
   * @param confirmText Confirm button text
   * @param cancelText Cancel button text
   * @returns Observable<boolean>
   */
  confirmDanger(
    title: string,
    message: string,
    confirmText: string = 'Delete',
    cancelText: string = 'Cancel'
  ): Observable<boolean> {
    return this.confirm({
      title,
      message,
      confirmText,
      cancelText,
      type: 'danger'
    });
  }

  /**
   * Show a warning confirmation dialog
   * @param title Dialog title
   * @param message Dialog message
   * @param confirmText Confirm button text
   * @param cancelText Cancel button text
   * @returns Observable<boolean>
   */
  confirmWarning(
    title: string,
    message: string,
    confirmText: string = 'Continue',
    cancelText: string = 'Cancel'
  ): Observable<boolean> {
    return this.confirm({
      title,
      message,
      confirmText,
      cancelText,
      type: 'warning'
    });
  }

  /**
   * Show an info confirmation dialog
   * @param title Dialog title
   * @param message Dialog message
   * @param confirmText Confirm button text
   * @param cancelText Cancel button text
   * @returns Observable<boolean>
   */
  confirmInfo(
    title: string,
    message: string,
    confirmText: string = 'OK',
    cancelText: string = 'Cancel'
  ): Observable<boolean> {
    return this.confirm({
      title,
      message,
      confirmText,
      cancelText,
      type: 'info'
    });
  }

  /**
   * Show a success confirmation dialog
   * @param title Dialog title
   * @param message Dialog message
   * @param confirmText Confirm button text
   * @param cancelText Cancel button text
   * @returns Observable<boolean>
   */
  confirmSuccess(
    title: string,
    message: string,
    confirmText: string = 'OK',
    cancelText: string = 'Cancel'
  ): Observable<boolean> {
    return this.confirm({
      title,
      message,
      confirmText,
      cancelText,
      type: 'success'
    });
  }
}
