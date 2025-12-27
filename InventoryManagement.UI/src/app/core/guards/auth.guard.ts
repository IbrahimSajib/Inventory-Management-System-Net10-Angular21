import { inject } from '@angular/core';
import { Router, CanActivateFn } from '@angular/router';
import { AuthService } from '../services/auth-service';
import { NotificationService } from '../services/notification.service';

export const authGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  if (authService.isAuthenticated()) {
    return true;
  }

  router.navigate(['/auth/login'], { queryParams: { returnUrl: state.url } });
  return false;
};

export const roleGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);
  const notificationService = inject(NotificationService);

  // Check authentication first
  if (!authService.isAuthenticated()) {
    notificationService.showError('Please login to continue');
    router.navigate(['/auth/login']);
    return false;
  }

  const requiredRoles = route.data['roles'] as string[];
  const currentUser = authService.currentUser();
  const userRoles = currentUser?.roles || [];

  // Debug logging
  console.log('roleGuard check:', {
    path: state.url,
    requiredRoles,
    userRoles,
    hasPermission: authService.hasAnyRole(requiredRoles || [])
  });

  // If route requires specific roles, verify user has them
  if (requiredRoles && requiredRoles.length > 0) {
    const hasPermission = authService.hasAnyRole(requiredRoles);

    if (!hasPermission) {
      notificationService.showError(
        `Access denied. You need one of these roles: ${requiredRoles.join(', ')}`
      );
      router.navigate(['/dashboard']);
      return false;
    }
  }

  return true;
};