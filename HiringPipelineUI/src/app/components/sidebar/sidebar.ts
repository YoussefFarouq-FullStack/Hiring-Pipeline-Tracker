import { Component, computed, signal, ChangeDetectionStrategy, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { Subject, takeUntil } from 'rxjs';
import { ConfirmationDialogService } from '../shared/confirmation-dialog/confirmation-dialog.service';

interface NavigationItem {
  title: string;
  url: string;
  icon: string;
  description: string;
}

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [CommonModule, RouterModule, MatSnackBarModule],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './sidebar.html',
  styleUrls: ['./sidebar.scss']
})
export class AppSidebarComponent implements OnInit, OnDestroy {
  collapsed = signal(false);
  currentUser: any = null;
  private destroy$ = new Subject<void>();

  constructor(
    private authService: AuthService,
    private router: Router,
    private snackBar: MatSnackBar,
    private confirmationDialog: ConfirmationDialogService
  ) {}

  ngOnInit(): void {
    // Subscribe to current user changes
    this.authService.currentUser$.pipe(
      takeUntil(this.destroy$)
    ).subscribe(user => {
      this.currentUser = user;
    });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  navigationItems: NavigationItem[] = [
    {
      title: 'Dashboard',
      url: '/dashboard',
      icon: 'dashboard',
      description: 'Overview & Analytics'
    },
    {
      title: 'Requisitions',
      url: '/requisitions',
      icon: 'file-text',
      description: 'Manage job openings'
    },
    {
      title: 'Candidates',
      url: '/candidates',
      icon: 'users',
      description: 'Manage candidate profiles'
    },
    {
      title: 'Applications',
      url: '/applications',
      icon: 'user-check',
      description: 'View applications'
    },
    {
      title: 'Stage History',
      url: '/stage-history',
      icon: 'bar-chart',
      description: 'Track hiring stages'
    }
  ];

  getFilteredNavigationItems(): NavigationItem[] {
    if (!this.currentUser || !this.currentUser.role) {
      return this.navigationItems;
    }

    // Filter based on user role
    const userRole = this.currentUser.role.toLowerCase();
    
    if (userRole === 'admin') {
      return this.navigationItems; // Admin sees everything
    } else if (userRole === 'recruiter') {
      // Recruiters see most things except admin features
      return this.navigationItems;
    } else if (userRole === 'hiring manager') {
      // Hiring managers see candidates, applications, and stage history
      return this.navigationItems.filter(item => 
        ['/dashboard', '/candidates', '/applications', '/stage-history'].includes(item.url)
      );
    } else if (userRole === 'interviewer') {
      // Interviewers see only applications and stage history
      return this.navigationItems.filter(item => 
        ['/dashboard', '/applications', '/stage-history'].includes(item.url)
      );
    } else if (userRole === 'read-only') {
      // Read-only users see only dashboard and view-only pages
      return this.navigationItems.filter(item => 
        ['/dashboard', '/requisitions', '/candidates'].includes(item.url)
      );
    }

    // Default: show only dashboard
    return this.navigationItems.filter(item => item.url === '/dashboard');
  }

  sidebarClasses = computed(() => 
    `relative bg-gradient-to-b from-white to-gray-50 border-r border-gray-200 transition-all duration-300 flex flex-col h-screen ${
      this.collapsed() ? 'w-16' : 'w-64'
    }`
  );

     getNavClasses(): string {
     const baseClasses = 'flex items-center px-3 py-2 rounded-lg text-xs transition-all duration-200 hover:bg-gradient-to-r hover:from-gray-100 hover:to-gray-200 hover:text-gray-900';
     return this.collapsed() 
       ? `${baseClasses} justify-center w-full` 
       : `${baseClasses} gap-3 hover:transform hover:translate-x-1`;
   }



  toggleSidebar(): void {
    this.collapsed.update(value => !value);
  }

  signOut(): void {
    this.confirmationDialog.confirmWarning(
      'Sign Out',
      'Are you sure you want to sign out? You will need to log in again to access the system.',
      'Sign Out',
      'Stay Logged In'
    ).subscribe(confirmed => {
      if (confirmed) {
        this.authService.logout().subscribe({
          next: () => {
            this.snackBar.open('You have been signed out successfully', 'Close', {
              duration: 3000,
              panelClass: ['success-snackbar']
            });
          },
          error: (error) => {
            console.error('Logout error:', error);
            // Even if logout fails, show success message since local auth is cleared
            this.snackBar.open('You have been signed out successfully', 'Close', {
              duration: 3000,
              panelClass: ['success-snackbar']
            });
          }
        });
      }
    });
  }
}
