import { Injectable } from '@angular/core';
import { CanActivate, Router, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { MatSnackBar } from '@angular/material/snack-bar';

@Injectable({
  providedIn: 'root'
})
export class RoleGuard implements CanActivate {
  constructor(
    private authService: AuthService,
    private router: Router,
    private snackBar: MatSnackBar
  ) {}

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): boolean {
    const requiredRoles = route.data['roles'] as string[];
    
    if (!requiredRoles || requiredRoles.length === 0) {
      return true; // No role requirement
    }

    if (!this.authService.isLoggedIn()) {
      this.router.navigate(['/login'], { 
        queryParams: { returnUrl: state.url } 
      });
      return false;
    }

    const userRole = this.authService.getCurrentUser()?.role;
    
    if (!userRole) {
      console.warn('User role not found, redirecting to dashboard');
      this.router.navigate(['/dashboard'], { 
        queryParams: { error: 'role_not_found' } 
      });
      return false;
    }

    const hasRequiredRole = requiredRoles.includes(userRole.toLowerCase());
    
    if (!hasRequiredRole) {
      const pageName = this.getPageName(state.url);
      const userRoleDisplay = this.getRoleDisplayName(userRole);
      const requiredRolesDisplay = requiredRoles.map(r => this.getRoleDisplayName(r)).join(', ');
      
      console.warn(`Access denied: User role '${userRoleDisplay}' cannot access ${pageName}. Required roles: ${requiredRolesDisplay}`);
      
      this.snackBar.open(
        `Access denied: You need ${requiredRolesDisplay} role to access ${pageName}`,
        'Close',
        {
          duration: 5000,
          panelClass: ['error-snackbar']
        }
      );
      
      this.router.navigate(['/dashboard'], { 
        queryParams: { 
          error: 'insufficient_permissions',
          requiredRoles: requiredRolesDisplay,
          userRole: userRoleDisplay,
          page: pageName
        } 
      });
      return false;
    }

    return true;
  }

  private getPageName(url: string): string {
    const pageMap: { [key: string]: string } = {
      '/requisitions': 'Requisitions',
      '/candidates': 'Candidates',
      '/applications': 'Applications',
      '/stage-history': 'Stage History',
      '/dashboard': 'Dashboard'
    };
    return pageMap[url] || url;
  }

  private getRoleDisplayName(role: string): string {
    const roleMap: { [key: string]: string } = {
      'admin': 'Administrator',
      'recruiter': 'Recruiter',
      'hiring manager': 'Hiring Manager',
      'interviewer': 'Interviewer',
      'read-only': 'Read-Only'
    };
    return roleMap[role.toLowerCase()] || role;
  }
}
