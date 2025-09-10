export interface User {
  id: number;
  username: string;
  email: string;
  firstName: string;
  lastName: string;
  roles: string[]; // Changed from Role[] to string[] to match backend DTO
  permissions: string[];
  isActive: boolean;
  lastLoginAt?: string;
  createdAt?: string;
  updatedAt?: string;
}

export interface Role {
  id: number;
  name: string;
  description: string;
  permissions: Permission[];
  createdAt?: string;
  updatedAt?: string;
}

export interface Permission {
  id: number;
  name: string;
  resource: string;
  action: string;
  description: string;
  createdAt?: string;
}

export interface CreateUserRequest {
  username: string;
  email: string;
  firstName: string;
  lastName: string;
  password: string;
  roleIds: number[]; // Changed from roleId to roleIds to match backend DTO
  isActive: boolean;
}

export interface UpdateUserRequest {
  username?: string;
  email?: string;
  firstName?: string;
  lastName?: string;
  password?: string;
  roleIds?: number[]; // Changed from roleId to roleIds to match backend DTO
  isActive?: boolean;
}

export interface CreateRoleRequest {
  name: string;
  description: string;
  permissionIds: number[];
}

export interface UpdateRoleRequest {
  name?: string;
  description?: string;
  permissionIds?: number[];
}
