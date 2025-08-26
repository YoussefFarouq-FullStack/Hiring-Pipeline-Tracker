import { Routes } from '@angular/router';
import { RequisitionsComponent } from './components/requisitions/requisitions';
import { CandidatesComponent } from './components/candidates/candidates';
import { ApplicationsComponent } from './components/applications/applications';

export const routes: Routes = [
  { path: 'requisitions', component: RequisitionsComponent },
  { path: 'candidates', component: CandidatesComponent },
  { path: 'applications', component: ApplicationsComponent },
  { path: '', redirectTo: 'candidates', pathMatch: 'full' }
];
