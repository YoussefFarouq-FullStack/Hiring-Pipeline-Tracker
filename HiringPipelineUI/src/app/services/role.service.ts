import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Role, Permission, CreateRoleRequest, UpdateRoleRequest } from '../models/user.model';

@Injectable({
  providedIn: 'root'
})
export class RoleService {
  private apiUrl = '/api/role';

  constructor(private http: HttpClient) {}

  getAllRoles(): Observable<Role[]> {
    return this.http.get<Role[]>(this.apiUrl);
  }

  getRoleById(id: number): Observable<Role> {
    return this.http.get<Role>(`${this.apiUrl}/${id}`);
  }

  createRole(role: CreateRoleRequest): Observable<Role> {
    return this.http.post<Role>(this.apiUrl, role);
  }

  updateRole(id: number, role: UpdateRoleRequest): Observable<Role> {
    return this.http.put<Role>(`${this.apiUrl}/${id}`, role);
  }

  deleteRole(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  getAllPermissions(): Observable<Permission[]> {
    return this.http.get<Permission[]>(`${this.apiUrl}/permissions`);
  }
}
