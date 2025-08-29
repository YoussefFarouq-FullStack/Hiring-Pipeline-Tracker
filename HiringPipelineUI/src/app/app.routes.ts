import { Routes } from '@angular/router';
import { RequisitionsComponent } from './components/requisitions/requisitions';
import { CandidatesComponent } from './components/candidates/candidates';
import { ApplicationsComponent } from './components/applications/applications';
import { StageHistoryComponent } from './components/stage-history/stage-history';

export const routes: Routes = [
  { path: 'requisitions', component: RequisitionsComponent },
  { path: 'candidates', component: CandidatesComponent },
  { path: 'applications', component: ApplicationsComponent },
  { path: 'stage-history', component: StageHistoryComponent },
  { path: '', redirectTo: 'candidates', pathMatch: 'full' }
];
