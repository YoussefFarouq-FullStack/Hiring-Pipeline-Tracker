export interface Application {
  applicationId: number;
  candidateId: number;
  requisitionId: number;
  currentStage: string;
  status?: string;
  createdAt: string;
  updatedAt: string;
  // Add candidate and requisition details for display
  candidate?: Candidate;
  requisition?: Requisition;
}

export interface Candidate {
  candidateId: number;
  firstName: string;
  lastName: string;
  email: string;
  phone: string;
  status: string;
}

export interface Requisition {
  requisitionId: number;
  title: string;
  department: string;
  status: string;
}
