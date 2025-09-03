import { Candidate } from './candidate.model';
import { Requisition } from './requisition.model';

export interface Application {
  applicationId: number;
  candidateId: number;
  requisitionId: number;
  currentStage: string;
  status: string;
  createdAt: string;
  updatedAt: string;
  // Add candidate and requisition details for display
  candidate?: Candidate;
  requisition?: Requisition;
}
