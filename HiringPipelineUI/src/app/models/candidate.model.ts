export interface Candidate {
  candidateId: number;
  firstName: string;
  lastName: string;
  email: string;
  phone?: string;
  resumeFileName?: string;
  resumeFilePath?: string;
  description?: string;
  skills?: string;
  status: string;
  createdAt: string;
  updatedAt: string;
}
