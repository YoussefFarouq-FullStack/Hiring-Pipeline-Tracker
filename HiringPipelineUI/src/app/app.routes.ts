import { Routes } from '@angular/router';
import { AuthGuard } from './guards/auth.guard';
import { RoleGuard } from './guards/role.guard';

export const routes: Routes = [
  { 
    path: 'login', 
    loadComponent: () => import('./components/login/login').then(m => m.LoginComponent)
  },
  { 
    path: 'dashboard', 
    loadComponent: () => import('./components/dashboard/dashboard').then(m => m.DashboardComponent),
    canActivate: [AuthGuard]
  },
  { 
    path: 'requisitions', 
    loadComponent: () => import('./components/requisitions/requisitions').then(m => m.RequisitionsComponent),
    canActivate: [AuthGuard, RoleGuard],
    data: { roles: ['admin', 'recruiter', 'hiring manager', 'read-only'] }
  },
  { 
    path: 'candidates', 
    loadComponent: () => import('./components/candidates/candidates').then(m => m.CandidatesComponent),
    canActivate: [AuthGuard, RoleGuard],
    data: { roles: ['admin', 'recruiter', 'hiring manager', 'interviewer', 'read-only'] }
  },
  { 
    path: 'applications', 
    loadComponent: () => import('./components/applications/applications').then(m => m.ApplicationsComponent),
    canActivate: [AuthGuard, RoleGuard],
    data: { roles: ['admin', 'recruiter', 'hiring manager', 'interviewer'] }
  },
  { 
    path: 'stage-history', 
    loadComponent: () => import('./components/stage-history/stage-history').then(m => m.StageHistoryComponent),
    canActivate: [AuthGuard, RoleGuard],
    data: { roles: ['admin', 'recruiter', 'hiring manager', 'interviewer'] }
  },
  { path: '', redirectTo: 'dashboard', pathMatch: 'full' }
];
