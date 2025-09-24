import { Component, OnInit, ChangeDetectionStrategy, ChangeDetectorRef, AfterViewInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, ActivatedRoute, Router } from '@angular/router';
import { CandidateService } from '../../services/candidate.service';
import { RequisitionService } from '../../services/requisition.service';
import { ApplicationService } from '../../services/application.service';
import { StageHistoryService } from '../../services/stage-history.service';
import { AuditLogService } from '../../services/audit-log.service';
import { Candidate } from '../../models/candidate.model';
import { Requisition } from '../../models/requisition.model';
import { Application } from '../../models/application.model';
import { StageHistory } from '../../models/stage-history.model';
import { LoadingSpinnerComponent } from '../shared/loading-spinner/loading-spinner.component';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterModule, LoadingSpinnerComponent],
  templateUrl: './dashboard.html',
  styleUrls: ['./dashboard.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class DashboardComponent implements OnInit, AfterViewInit {
  // Dashboard metrics
  totalCandidates = 0;
  activeRequisitions = 0;
  totalHired = 0;
  totalApplications = 0;
  
  // Stage distribution data
  stageDistribution: { stage: string; count: number; percentage: number; color: string }[] = [];
  
  // Hiring velocity data (last 6 months)
  hiringVelocity: { month: string; hired: number; applied: number }[] = [];
  
  // Recent stage history data
  recentHistory: StageHistory[] = [];
  
  // Related data for candidate name lookup
  applications: Application[] = [];
  candidates: Candidate[] = [];
  requisitions: Requisition[] = [];
  
  // Recent activity data
  recentApplications: Application[] = [];
  recentRequisitions: Requisition[] = [];
  
  // Loading states
  isLoading = false;
  hasError = false;
  errorMessage = '';
  isLoadingHistory = false;
  hasHistoryError = false;

  constructor(
    private candidateService: CandidateService,
    private requisitionService: RequisitionService,
    private applicationService: ApplicationService,
    private stageHistoryService: StageHistoryService,
    private auditLogService: AuditLogService,
    private cdr: ChangeDetectorRef,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit(): void {
    // Note: Audit logging is now handled by the middleware automatically
    // The middleware will log "View Dashboard" when the dashboard page is accessed
    // and log background data fetches as "BackgroundFetch" type
    
    this.loadDashboardData();
    this.checkForPermissionErrors();
  }

  private checkForPermissionErrors(): void {
    this.route.queryParams.subscribe(params => {
      if (params['error'] === 'insufficient_permissions') {
        const requiredRoles = params['requiredRoles'];
        const userRole = params['userRole'];
        const page = params['page'];
        
        if (requiredRoles && userRole && page) {
          this.errorMessage = `Access denied: You need ${requiredRoles} role to access ${page}. Your current role: ${userRole}`;
          this.hasError = true;
          this.cdr.markForCheck();
        }
      } else if (params['error'] === 'role_not_found') {
        this.errorMessage = 'Access denied: Your user role could not be determined. Please contact your administrator.';
        this.hasError = true;
        this.cdr.markForCheck();
      }
    });
  }

  ngAfterViewInit(): void {
    // Trigger change detection after view is initialized
    this.cdr.detectChanges();
  }

  private loadDashboardData(): void {
    this.isLoading = true;
    this.hasError = false;

    // Load all data in parallel
    Promise.all([
      this.loadCandidates(),
      this.loadRequisitions(),
      this.loadApplications(),
      this.loadStageHistory(),
      this.loadRecentHistory()
    ]).then(() => {
      this.calculateMetrics();
      this.calculateStageDistribution();
      this.calculateHiringVelocity();
      this.isLoading = false;
      // Trigger change detection after data is loaded
      this.cdr.detectChanges();
    }).catch((error) => {
      this.handleError(error, 'Failed to load dashboard data');
      // Trigger change detection after error
      this.cdr.detectChanges();
    });
  }

  private async loadCandidates(): Promise<void> {
    return new Promise((resolve, reject) => {
      this.candidateService.getCandidatesForDashboard().subscribe({
        next: (candidates: Candidate[]) => {
          this.totalCandidates = candidates.length;
          this.candidates = candidates;
          
          // If no candidates, add sample data
          if (this.candidates.length === 0) {
            this.candidates = [
              {
                candidateId: 1,
                firstName: 'Sarah',
                lastName: 'Johnson',
                email: 'sarah.johnson@email.com',
                phone: '+1-555-0123',
                status: 'Active',
                createdAt: new Date().toISOString(),
                updatedAt: new Date().toISOString()
              },
              {
                candidateId: 2,
                firstName: 'Michael',
                lastName: 'Chen',
                email: 'michael.chen@email.com',
                phone: '+1-555-0124',
                status: 'Active',
                createdAt: new Date().toISOString(),
                updatedAt: new Date().toISOString()
              }
            ];
          }
          resolve();
        },
        error: reject
      });
    });
  }

  private async loadRequisitions(): Promise<void> {
    console.log('🔍 Starting loadRequisitions...');
    return new Promise((resolve, reject) => {
      this.requisitionService.getRequisitionsForDashboard().subscribe({
        next: (requisitions: Requisition[]) => {
          console.log('📊 Requisitions received:', requisitions.length);
          console.log('📊 Requisition statuses:', requisitions.map(r => ({ id: r.requisitionId, status: r.status })));
          
          // Count active requisitions - check for various active statuses
          this.activeRequisitions = requisitions.filter(r => 
            r.status === 'Active' || 
            r.status === 'Open' || 
            r.status === 'Published' ||
            r.status === 'active' ||
            r.status === 'open' ||
            r.status === 'published'
          ).length;
          
          console.log('📈 Active requisitions count:', this.activeRequisitions);
          
          this.requisitions = requisitions;
          // Get recent requisitions (last 5 created)
          this.recentRequisitions = requisitions
            .sort((a, b) => new Date(b.createdAt || new Date()).getTime() - new Date(a.createdAt || new Date()).getTime())
            .slice(0, 5);
          
          console.log('📈 Recent requisitions:', this.recentRequisitions.length);
          
          // If no recent requisitions, add sample data
          if (this.recentRequisitions.length === 0) {
            console.log('➕ Adding sample requisition data...');
            this.recentRequisitions = [
              {
                requisitionId: 1,
                title: 'Senior React Developer',
                description: 'Lead development of React applications with modern technologies',
                department: 'Engineering',
                location: 'San Francisco, CA',
                employmentType: 'Full-time',
                salary: '$120,000 - $160,000',
                isDraft: false,
                priority: 'High',
                requiredSkills: 'React, TypeScript, Node.js, GraphQL',
                experienceLevel: 'Senior',
                jobLevel: 'Senior',
                status: 'Open',
                createdAt: new Date(Date.now() - 24 * 60 * 60 * 1000).toISOString(),
                updatedAt: new Date().toISOString()
              }
            ];
          }
          console.log('✅ Final recentRequisitions length:', this.recentRequisitions.length);
          resolve();
        },
        error: reject
      });
    });
  }

  private async loadApplications(): Promise<void> {
    console.log('🔍 Starting loadApplications...');
    return new Promise((resolve, reject) => {
      this.applicationService.getApplicationsForDashboard().subscribe({
        next: (applications: Application[]) => {
          console.log('📊 Applications received:', applications.length);
          this.totalApplications = applications.length;
          this.applications = applications;
          // Get recent applications (last 5 applied)
          this.recentApplications = applications
            .sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime())
            .slice(0, 5);
          
          console.log('📈 Recent applications:', this.recentApplications.length);
          
          // If no recent applications, add sample data
          if (this.recentApplications.length === 0) {
            console.log('➕ Adding sample application data...');
            this.recentApplications = [
              {
                applicationId: 1,
                candidateId: 1,
                requisitionId: 1,
                currentStage: 'Applied',
                status: 'Active',
                createdAt: new Date(Date.now() - 2 * 60 * 60 * 1000).toISOString(), // 2 hours ago
                updatedAt: new Date().toISOString()
              },
              {
                applicationId: 2,
                candidateId: 2,
                requisitionId: 1,
                currentStage: 'Screening',
                status: 'Active',
                createdAt: new Date(Date.now() - 5 * 60 * 60 * 1000).toISOString(), // 5 hours ago
                updatedAt: new Date().toISOString()
              }
            ];
          }
          console.log('✅ Final recentApplications length:', this.recentApplications.length);
          resolve();
        },
        error: reject
      });
    });
  }

  private async loadStageHistory(): Promise<void> {
    return new Promise((resolve, reject) => {
      this.stageHistoryService.getStageHistoryForDashboard().subscribe({
        next: (histories: StageHistory[]) => {
          // Count hired candidates
          const hiredHistories = histories.filter(h => 
            h.toStage.toLowerCase().includes('hired')
          );
          this.totalHired = new Set(hiredHistories.map(h => h.applicationId)).size;
          resolve();
        },
        error: reject
      });
    });
  }

  private async loadRecentHistory(): Promise<void> {
    console.log('🔍 Starting loadRecentHistory...');
    this.isLoadingHistory = true;
    this.hasHistoryError = false;
    this.cdr.detectChanges(); // Force update to show loading state
    
    return new Promise((resolve, reject) => {
      this.stageHistoryService.getStageHistoryForDashboard().subscribe({
        next: (histories: StageHistory[]) => {
          console.log('📊 Stage histories received:', histories.length);
          
          // Get the 5 most recent stage movements
          this.recentHistory = histories
            .sort((a, b) => new Date(b.movedAt).getTime() - new Date(a.movedAt).getTime())
            .slice(0, 5);
          
          console.log('📈 Recent history after sorting:', this.recentHistory.length);
          
          // If no recent history, add sample data
          if (this.recentHistory.length === 0) {
            console.log('➕ Adding sample history data...');
            this.recentHistory = [
              {
                stageHistoryId: 1,
                applicationId: 1,
                fromStage: 'Applied',
                toStage: 'Screening',
                movedBy: 'Recruiter',
                movedAt: new Date(Date.now() - 3 * 60 * 60 * 1000).toISOString(), // 3 hours ago
                reason: 'Initial screening passed',
                notes: 'Candidate shows strong technical background'
              },
              {
                stageHistoryId: 2,
                applicationId: 2,
                fromStage: 'Screening',
                toStage: 'Interview',
                movedBy: 'Hiring Manager',
                movedAt: new Date(Date.now() - 6 * 60 * 60 * 1000).toISOString(), // 6 hours ago
                reason: 'Technical assessment completed',
                notes: 'Ready for technical interview'
              }
            ];
          }
          
          console.log('✅ Final recentHistory length:', this.recentHistory.length);
          this.isLoadingHistory = false;
          this.cdr.detectChanges(); // Force update to show data
          resolve();
        },
        error: (error) => {
          console.error('❌ Error loading recent history:', error);
          this.hasHistoryError = true;
          this.isLoadingHistory = false;
          this.cdr.detectChanges(); // Force update to show error
          reject(error);
        }
      });
    });
  }

  private calculateMetrics(): void {
    // Metrics are calculated in the load methods
  }

  private calculateStageDistribution(): void {
    // This would typically come from your backend
    // For now, we'll create sample data
    this.stageDistribution = [
      { stage: 'Applied', count: 45, percentage: 30, color: '#3B82F6' },
      { stage: 'Screening', count: 25, percentage: 17, color: '#8B5CF6' },
      { stage: 'Interview', count: 35, percentage: 23, color: '#F59E0B' },
      { stage: 'Offer', count: 15, percentage: 10, color: '#EF4444' },
      { stage: 'Hired', count: 20, percentage: 13, color: '#10B981' },
      { stage: 'Rejected', count: 10, percentage: 7, color: '#6B7280' }
    ];
  }

  private calculateHiringVelocity(): void {
    // This would typically come from your backend
    // For now, we'll create sample data for the last 6 months
    const months = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun'];
    this.hiringVelocity = months.map((month, index) => ({
      month,
      hired: [8, 12, 15, 10, 18, 14][index], // Static sample data
      applied: [25, 35, 42, 30, 48, 38][index] // Static sample data
    }));
  }

  private handleError(error: any, fallbackMessage: string): void {
    console.error(fallbackMessage, error);
    this.hasError = true;
    this.errorMessage = error.message || fallbackMessage;
    this.isLoading = false;
    // Trigger change detection after error
    this.cdr.detectChanges();
  }

  // Helper methods for template
  getStageColor(stage: string): string {
    const stageLower = stage.toLowerCase();
    if (stageLower.includes('applied')) return '#3B82F6';
    if (stageLower.includes('screening')) return '#8B5CF6';
    if (stageLower.includes('interview')) return '#F59E0B';
    if (stageLower.includes('offer')) return '#EF4444';
    if (stageLower.includes('hired')) return '#10B981';
    if (stageLower.includes('rejected')) return '#6B7280';
    return '#3B82F6';
  }

  getStageIcon(stage: string): string {
    const stageLower = stage.toLowerCase();
    if (stageLower.includes('applied')) return 'person_add';
    if (stageLower.includes('screening')) return 'filter_list';
    if (stageLower.includes('interview')) return 'people';
    if (stageLower.includes('offer')) return 'card_giftcard';
    if (stageLower.includes('hired')) return 'check_circle';
    if (stageLower.includes('rejected')) return 'cancel';
    return 'help';
  }

  getStageDotClass(stage: string): string {
    const stageLower = stage.toLowerCase();
    if (stageLower.includes('applied')) return 'bg-blue-500';
    if (stageLower.includes('screening')) return 'bg-purple-500';
    if (stageLower.includes('interview')) return 'bg-yellow-500';
    if (stageLower.includes('offer')) return 'bg-red-500';
    if (stageLower.includes('hired')) return 'bg-green-500';
    if (stageLower.includes('rejected')) return 'bg-gray-500';
    return 'bg-blue-500';
  }

  getStageBadgeClass(stage: string): string {
    const stageLower = stage.toLowerCase();
    if (stageLower.includes('applied')) return 'bg-blue-100 text-blue-800';
    if (stageLower.includes('screening')) return 'bg-purple-100 text-purple-800';
    if (stageLower.includes('interview')) return 'bg-yellow-100 text-yellow-800';
    if (stageLower.includes('offer')) return 'bg-red-100 text-red-800';
    if (stageLower.includes('hired')) return 'bg-green-100 text-green-800';
    if (stageLower.includes('rejected')) return 'bg-gray-100 text-gray-800';
    return 'bg-blue-100 text-blue-800';
  }

  getCandidateName(applicationId: number): string {
    try {
      // Find the application for this stage history entry
      const application = this.applications.find(app => app.applicationId === applicationId);
      if (!application) {
        console.log(`⚠️ Application not found for ID: ${applicationId}`);
        return `Application #${applicationId}`;
      }

      // Find the candidate for this application
      const candidate = this.candidates.find(c => c.candidateId === application.candidateId);
      if (!candidate) {
        console.log(`⚠️ Candidate not found for ID: ${application.candidateId}`);
        return `Candidate #${application.candidateId}`;
      }

      return `${candidate.firstName} ${candidate.lastName}`;
    } catch (error) {
      console.error('❌ Error in getCandidateName:', error);
      return `Candidate #${applicationId}`;
    }
  }

  getTimeAgo(dateString: string): string {
    try {
      if (!dateString) return 'Unknown time';
      const date = new Date(dateString);
      if (isNaN(date.getTime())) return 'Invalid date';
      
      const now = new Date();
      const diffInSeconds = Math.floor((now.getTime() - date.getTime()) / 1000);
      
      if (diffInSeconds < 60) return 'Just now';
      if (diffInSeconds < 3600) return `${Math.floor(diffInSeconds / 60)}m ago`;
      if (diffInSeconds < 86400) return `${Math.floor(diffInSeconds / 3600)}h ago`;
      if (diffInSeconds < 2592000) return `${Math.floor(diffInSeconds / 86400)}d ago`;
      if (diffInSeconds < 31536000) return `${Math.floor(diffInSeconds / 2592000)}mo ago`;
      return `${Math.floor(diffInSeconds / 31536000)}y ago`;
    } catch (error) {
      console.error('❌ Error in getTimeAgo:', error);
      return 'Unknown time';
    }
  }

  getRequisitionTitle(requisitionId: number): string {
    try {
      const requisition = this.requisitions.find(r => r.requisitionId === requisitionId);
      if (!requisition) {
        console.log(`⚠️ Requisition not found for ID: ${requisitionId}`);
        return `Requisition #${requisitionId}`;
      }
      return requisition.title || `Requisition #${requisitionId}`;
    } catch (error) {
      console.error('❌ Error in getRequisitionTitle:', error);
      return `Requisition #${requisitionId}`;
    }
  }

  retryLoad(): void {
    this.loadDashboardData();
  }

  // Computed properties for template
  get totalHiredCount(): number {
    return this.hiringVelocity.reduce((sum, item) => sum + item.hired, 0);
  }

  get totalAppliedCount(): number {
    return this.hiringVelocity.reduce((sum, item) => sum + item.applied, 0);
  }

  // TrackBy functions for better performance
  trackByStage(index: number, item: { stage: string; count: number; percentage: number; color: string }): string {
    return item.stage;
  }

  trackByMonth(index: number, item: { month: string; hired: number; applied: number }): string {
    return item.month;
  }

  trackByHistory(index: number, item: StageHistory): number {
    return item.stageHistoryId;
  }

  trackByApplication(index: number, item: Application): number {
    return item.applicationId;
  }

  trackByRequisition(index: number, item: Requisition): number {
    return item.requisitionId;
  }

  // Navigation methods for metric cards
  navigateToCandidates(): void {
    this.router.navigate(['/candidates']);
  }

  navigateToRequisitions(): void {
    this.router.navigate(['/requisitions']);
  }

  navigateToApplications(): void {
    this.router.navigate(['/applications']);
  }

  navigateToStageHistory(): void {
    this.router.navigate(['/stage-history']);
  }
}
