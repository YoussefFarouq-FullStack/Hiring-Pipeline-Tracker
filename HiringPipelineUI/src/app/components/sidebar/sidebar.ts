import { Component, computed, signal, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';

interface NavigationItem {
  title: string;
  url: string;
  icon: string;
  description: string;
}

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [CommonModule, RouterModule],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <aside [class]="sidebarClasses()">
      <!-- Header -->
      <div class="p-4 border-b border-gray-200">
        <div [class]="collapsed() ? 'flex items-center justify-center' : 'flex items-center gap-3'">
          <div class="flex h-8 w-8 items-center justify-center rounded-lg bg-gradient-to-br from-blue-500 to-blue-600 text-white">
            <svg class="h-4 w-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z"></path>
            </svg>
          </div>
          <div *ngIf="!collapsed()">
                         <h1 class="font-semibold text-xs text-gray-900">Hiring Tracker</h1>
             <p class="text-xs text-gray-600">HR Dashboard</p>
          </div>
        </div>
      </div>

      <!-- Navigation -->
      <div class="px-3 py-4">
        <div class="text-xs font-medium text-gray-500 mb-2">
          <span *ngIf="!collapsed()">Navigation</span>
        </div>

        <nav class="space-y-1">
          <a
            *ngFor="let item of navigationItems"
            [routerLink]="item.url"
            routerLinkActive="bg-gradient-to-r from-blue-500 to-blue-600 text-white shadow-lg"
            [routerLinkActiveOptions]="{exact: true}"
            [class]="getNavClasses()"
          >
            <ng-container [ngSwitch]="item.icon">
              <!-- Dashboard Icon -->
              <svg *ngSwitchCase="'dashboard'" class="h-4 w-4 shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 7v10a2 2 0 002 2h14a2 2 0 002-2V9a2 2 0 00-2-2H5a2 2 0 00-2-2z"></path>
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 5a2 2 0 012-2h4a2 2 0 012 2v2H8V5z"></path>
              </svg>
              
              <!-- File Text Icon -->
              <svg *ngSwitchCase="'file-text'" class="h-4 w-4 shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z"></path>
              </svg>
              
              <!-- Users Icon -->
              <svg *ngSwitchCase="'users'" class="h-4 w-4 shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0zm6 3a2 2 0 11-4 0 2 2 0 014 0zM7 10a2 2 0 11-4 0 2 2 0 014 0z"></path>
              </svg>
              
              <!-- User Check Icon -->
              <svg *ngSwitchCase="'user-check'" class="h-4 w-4 shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4.354a4 4 0 110 5.292M15 21H3v-1a6 6 0 0112 0v1zm0 0h6v-1a6 6 0 00-9-5.197m13.5-9a2.5 2.5 0 11-5 0 2.5 2.5 0 015 0z"></path>
              </svg>
              
              <!-- Bar Chart Icon -->
              <svg *ngSwitchCase="'bar-chart'" class="h-4 w-4 shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z"></path>
              </svg>
              
              <!-- Default Icon -->
              <svg *ngSwitchDefault class="h-4 w-4 shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z"></path>
              </svg>
            </ng-container>
            
                         <div *ngIf="!collapsed()" class="flex flex-col items-start ml-3">
               <span class="text-xs font-medium">{{ item.title }}</span>
               <span class="text-xs opacity-75">{{ item.description }}</span>
             </div>
          </a>
        </nav>
      </div>

      <!-- Toggle Button -->
      <button
        (click)="toggleSidebar()"
        class="absolute top-4 -right-3 h-6 w-6 rounded-full bg-white border border-gray-200 flex items-center justify-center hover:bg-gray-50 shadow-md"
      >
        <svg class="h-3 w-3" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path *ngIf="collapsed()" stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7"></path>
          <path *ngIf="!collapsed()" stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 19l-7-7 7-7"></path>
        </svg>
      </button>
    </aside>
  `
})
export class AppSidebarComponent {
  collapsed = signal(false);

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
      title: 'Pipeline',
      url: '/stage-history',
      icon: 'bar-chart',
      description: 'Track hiring stages'
    }
  ];

  sidebarClasses = computed(() => 
    `relative bg-gradient-to-b from-white to-gray-50 border-r border-gray-200 transition-all duration-300 ${
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
}
