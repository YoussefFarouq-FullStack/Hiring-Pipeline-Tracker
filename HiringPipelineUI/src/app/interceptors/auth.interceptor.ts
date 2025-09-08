import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth.service';

export const AuthInterceptor: HttpInterceptorFn = (req, next) => {
  // Get the auth service using inject
  const authService = inject(AuthService);
  
  // Get the token from the auth service
  const token = authService.getToken();
  
  // If we have a token and this is not the login request, add it to the headers
  if (token && !req.url.includes('/api/auth/login')) {
    const authReq = req.clone({
      headers: req.headers.set('Authorization', `Bearer ${token}`)
    });
    return next(authReq);
  }
  
  // Otherwise, pass the request through unchanged
  return next(req);
};
