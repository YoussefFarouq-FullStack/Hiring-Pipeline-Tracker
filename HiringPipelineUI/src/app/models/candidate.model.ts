export interface Candidate {
  candidateId: number;
  firstName: string;
  lastName: string;
  email: string;
  phone?: string;
  linkedInUrl?: string;
  source?: string;
  status: string;
  createdAt: string;
  updatedAt: string;
}
