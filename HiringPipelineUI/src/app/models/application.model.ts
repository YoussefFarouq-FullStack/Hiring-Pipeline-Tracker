export interface Application {
  applicationId: number;
  candidateId: number;
  requisitionId: number;
  currentStage: string;
  status?: string;
  createdAt: string;
  updatedAt: string;
}

export interface Candidate {
  candidateId: number;
  fullName: string;
}

export interface Requisition {
  requisitionId: number;
  title: string;
}
