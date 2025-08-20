import { Routes } from '@angular/router';
import { RequisitionsComponent } from './requisitions/requisitions';

export const routes: Routes = [
  { path: 'requisitions', component: RequisitionsComponent },
  { path: '', redirectTo: 'requisitions', pathMatch: 'full' }
];
