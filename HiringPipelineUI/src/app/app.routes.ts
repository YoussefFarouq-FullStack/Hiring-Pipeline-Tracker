import { Routes } from '@angular/router';

export const routes: Routes = [
  { 
    path: 'dashboard', 
    loadComponent: () => import('./components/dashboard/dashboard').then(m => m.DashboardComponent)
  },
  { 
    path: 'requisitions', 
    loadComponent: () => import('./components/requisitions/requisitions').then(m => m.RequisitionsComponent)
  },
  { 
    path: 'candidates', 
    loadComponent: () => import('./components/candidates/candidates').then(m => m.CandidatesComponent)
  },
  { 
    path: 'applications', 
    loadComponent: () => import('./components/applications/applications').then(m => m.ApplicationsComponent)
  },
  { 
    path: 'stage-history', 
    loadComponent: () => import('./components/stage-history/stage-history').then(m => m.StageHistoryComponent)
  },
  { path: '', redirectTo: 'dashboard', pathMatch: 'full' }
];
