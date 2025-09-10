import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { User, CreateUserRequest, UpdateUserRequest } from '../models/user.model';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private apiUrl = '/api/users';

  constructor(private http: HttpClient) {}

  getAllUsers(): Observable<User[]> {
    return this.http.get<User[]>(this.apiUrl).pipe(
      map(users => this.sortUsersByRole(users))
    );
  }

  private sortUsersByRole(users: User[]): User[] {
    // Define role priority order
    const rolePriority: { [key: string]: number } = {
      'Admin': 1,
      'Recruiter': 2,
      'Hiring Manager': 3,
      'Interviewer': 4,
      'Read-only': 5,
      'Guest': 6
    };

    return users.sort((a, b) => {
      // Get the first role for each user (assuming users have one primary role)
      const roleA = a.roles && a.roles.length > 0 ? a.roles[0] : '';
      const roleB = b.roles && b.roles.length > 0 ? b.roles[0] : '';
      
      // Get priority values, default to 999 for unknown roles
      const priorityA = rolePriority[roleA] || 999;
      const priorityB = rolePriority[roleB] || 999;
      
      // Sort by priority, then by name if same priority
      if (priorityA !== priorityB) {
        return priorityA - priorityB;
      }
      
      // If same role priority, sort by last name, then first name
      const lastNameA = a.lastName || '';
      const lastNameB = b.lastName || '';
      if (lastNameA !== lastNameB) {
        return lastNameA.localeCompare(lastNameB);
      }
      
      return (a.firstName || '').localeCompare(b.firstName || '');
    });
  }

  getUserById(id: number): Observable<User> {
    return this.http.get<User>(`${this.apiUrl}/${id}`);
  }

  createUser(user: CreateUserRequest): Observable<User> {
    return this.http.post<User>(this.apiUrl, user);
  }

  updateUser(id: number, user: UpdateUserRequest): Observable<User> {
    return this.http.put<User>(`${this.apiUrl}/${id}`, user);
  }

  deleteUser(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  changePassword(id: number, currentPassword: string, newPassword: string): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/${id}/change-password`, {
      currentPassword,
      newPassword
    });
  }
}
