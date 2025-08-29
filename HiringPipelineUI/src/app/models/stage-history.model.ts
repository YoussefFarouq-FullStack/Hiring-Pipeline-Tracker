export interface StageHistory {
  stageHistoryId: number;
  applicationId: number;
  fromStage?: string;
  toStage: string;
  movedBy: string;
  movedAt: string;
  notes?: string;
  reason?: string;
}

export interface CreateStageHistoryDto {
  applicationId: number;
  fromStage?: string;
  toStage: string;
  movedBy: string;
  notes?: string;
  reason?: string;
}

// Define the hiring pipeline stages in order
export const HIRING_STAGES = [
  'Applied',
  'Phone Screen',
  'Technical Interview',
  'Onsite Interview',
  'Reference Check',
  'Offer',
  'Hired',
  'Rejected',
  'Withdrawn'
] as const;

export type HiringStage = typeof HIRING_STAGES[number];

// Stage progression rules
export const STAGE_PROGRESSION_RULES = {
  'Applied': ['Phone Screen', 'Rejected', 'Withdrawn'],
  'Phone Screen': ['Technical Interview', 'Rejected', 'Withdrawn'],
  'Technical Interview': ['Onsite Interview', 'Rejected', 'Withdrawn'],
  'Onsite Interview': ['Reference Check', 'Rejected', 'Withdrawn'],
  'Reference Check': ['Offer', 'Rejected', 'Withdrawn'],
  'Offer': ['Hired', 'Rejected', 'Withdrawn'],
  'Hired': [], // Terminal stage
  'Rejected': [], // Terminal stage
  'Withdrawn': [] // Terminal stage
} as const;



