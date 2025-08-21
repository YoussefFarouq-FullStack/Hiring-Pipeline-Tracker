import { Routes } from '@angular/router';
import { RequisitionsComponent } from './components/requisitions/requisitions';
import { CandidatesComponent } from './components/candidates/candidates';


export const routes: Routes = [
  { path: 'requisitions', component: RequisitionsComponent },
  { path: 'candidates', component: CandidatesComponent },
  { path: '', redirectTo: 'candidates', pathMatch: 'full' }
];
