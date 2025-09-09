export interface Requisition {
  requisitionId: number;
  title: string;
  description?: string;
  department: string;
  location?: string;
  employmentType?: string;
  salary?: string;
  isDraft: boolean;
  priority: string;
  requiredSkills?: string;
  experienceLevel?: string;
  jobLevel?: string;
  status: string;
  createdAt: string;
  updatedAt: string;
}

export interface CreateRequisitionRequest {
  title: string;
  description?: string;
  department: string;
  location?: string;
  employmentType?: string;
  salary?: string;
  isDraft: boolean;
  priority: string;
  requiredSkills?: string;
  experienceLevel?: string;
  jobLevel?: string;
}

export interface UpdateRequisitionRequest {
  title?: string;
  description?: string;
  department?: string;
  location?: string;
  employmentType?: string;
  salary?: string;
  isDraft?: boolean;
  priority?: string;
  requiredSkills?: string;
  experienceLevel?: string;
  jobLevel?: string;
  status?: string;
}

// Constants for dropdown options
export const EMPLOYMENT_TYPES = [
  'Full-time',
  'Part-time', 
  'Contract',
  'Internship',
  'Temporary'
];

export const PRIORITIES = [
  'Low',
  'Medium',
  'High'
];

export const EXPERIENCE_LEVELS = [
  'Junior',
  'Mid',
  'Senior'
];

export const JOB_LEVELS = [
  'Entry',
  'Junior',
  'Mid',
  'Senior',
  'Lead',
  'Principal',
  'Director',
  'VP',
  'C-Level'
];

export const STATUSES = [
  'Open',
  'On Hold',
  'Closed',
  'Cancelled'
];
