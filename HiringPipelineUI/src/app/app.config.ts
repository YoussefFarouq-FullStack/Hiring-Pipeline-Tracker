import { ApplicationConfig, provideBrowserGlobalErrorListeners, provideZoneChangeDetection, ErrorHandler } from '@angular/core';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { provideRouter } from '@angular/router';
import { provideAnimations } from '@angular/platform-browser/animations';

import { routes } from './app.routes';
import { AuthInterceptor } from './interceptors/auth.interceptor';
import { GlobalErrorHandler } from './error-handler';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideHttpClient(withInterceptors([AuthInterceptor])),
    provideAnimations(),
    { provide: ErrorHandler, useClass: GlobalErrorHandler }
  ]
};
