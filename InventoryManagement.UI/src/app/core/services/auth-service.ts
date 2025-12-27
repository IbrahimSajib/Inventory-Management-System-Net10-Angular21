import { Injectable, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../models/api-response';
import { LoginRequest, LoginResponse, RegisterRequest, UserInfo } from '../models/auth';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private http = inject(HttpClient);
  private router = inject(Router);

  private apiUrl = `${environment.apiUrl}/Auth`;

  // Signals for reactive state
  currentUser = signal<UserInfo | null>(null);
  isAuthenticated = signal<boolean>(false);

  constructor() {
    this.loadUserFromStorage();
  }

  login(request: LoginRequest): Observable<ApiResponse<LoginResponse>> {
    return this.http.post<ApiResponse<LoginResponse>>(`${this.apiUrl}/login`, request).pipe(
      tap(response => {
        if (response.success && response.data) {
          this.setSession(response.data);
        }
      })
    );
  }

  register(request: RegisterRequest): Observable<ApiResponse<LoginResponse>> {
    return this.http.post<ApiResponse<LoginResponse>>(`${this.apiUrl}/register`, request).pipe(
      tap(response => {
        if (response.success && response.data) {
          this.setSession(response.data);
        }
      })
    );
  }

  logout(): void {
    localStorage.removeItem('access_token');
    localStorage.removeItem('refresh_token');
    localStorage.removeItem('user_info');
    this.currentUser.set(null);
    this.isAuthenticated.set(false);
    this.router.navigate(['/auth/login']);
  }

  private setSession(authResult: LoginResponse): void {
    localStorage.setItem('access_token', authResult.accessToken);
    localStorage.setItem('refresh_token', authResult.refreshToken);
    localStorage.setItem('user_info', JSON.stringify(authResult.user));

    // Extract roles from token and update user info
    const rolesFromToken = this.extractRolesFromToken(authResult.accessToken);
    const userWithRoles: UserInfo = {
      ...authResult.user,
      roles: rolesFromToken.length > 0 ? rolesFromToken : authResult.user.roles || []
    };

    this.currentUser.set(userWithRoles);
    this.isAuthenticated.set(true);
  }

  private loadUserFromStorage(): void {
    const token = localStorage.getItem('access_token');
    const userInfo = localStorage.getItem('user_info');

    if (token && userInfo) {
      const user = JSON.parse(userInfo);
      // Extract roles from token
      const rolesFromToken = this.extractRolesFromToken(token);
      const userWithRoles: UserInfo = {
        ...user,
        roles: rolesFromToken.length > 0 ? rolesFromToken : user.roles || []
      };

      this.currentUser.set(userWithRoles);
      this.isAuthenticated.set(true);
    }
  }

  /**
   * Decode JWT token and extract roles from it
   * JWT tokens contain roles in the 'role' claim (can be array or string)
   */
  // Add console logging to extractRolesFromToken method
  private extractRolesFromToken(token: string): string[] {
    try {
      const payload = this.decodeToken(token);
      if (!payload) return [];

      // Log all claims in the token
      console.log('JWT Payload:', payload);

      // JWT roles from .NET are typically in 'role' claim (can be multiple claims with same key)
      // When parsed, they become an array
      const roleClaimKeys = [
        'role',  // Standard JWT role claim
        'roles',  // Alternative
        'http://schemas.microsoft.com/ws/2008/06/identity/claims/role'  // .NET format
      ];

      for (const key of roleClaimKeys) {
        if (payload[key]) {
          const roles = payload[key];
          console.log(`Found roles in claim '${key}':`, roles);

          if (Array.isArray(roles)) {
            return roles;
          } else if (typeof roles === 'string') {
            return [roles];
          }
        }
      }

      console.warn('No roles found in JWT token');
      return [];
    } catch (error) {
      console.error('Error extracting roles from token:', error);
      return [];
    }
  }

  /**
   * Decode JWT token without verification (client-side only)
   * For verification, you should validate on the server
   */
  private decodeToken(token: string): any {
    try {
      const parts = token.split('.');
      if (parts.length !== 3) {
        throw new Error('Invalid token format');
      }

      const decoded = JSON.parse(this.base64UrlDecode(parts[1]));
      return decoded;
    } catch (error) {
      console.error('Error parsing token:', error);
      return null;
    }
  }

  /**
   * Decode base64url string to regular string
   */
  private base64UrlDecode(str: string): string {
    let output = str.replace(/-/g, '+').replace(/_/g, '/');
    switch (output.length % 4) {
      case 0:
        break;
      case 2:
        output += '==';
        break;
      case 3:
        output += '=';
        break;
      default:
        throw new Error('Invalid base64url string');
    }
    return decodeURIComponent(
      atob(output)
        .split('')
        .map(c => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2))
        .join('')
    );
  }

  getToken(): string | null {
    return localStorage.getItem('access_token');
  }

  getRefreshToken(): string | null {
    return localStorage.getItem('refresh_token');
  }

  hasRole(role: string): boolean {
    const user = this.currentUser();
    return user?.roles?.includes(role) ?? false;
  }

  hasAnyRole(roles: string[]): boolean {
    const user = this.currentUser();
    return roles.some(role => user?.roles?.includes(role)) ?? false;
  }

  /**
   * Check if token is expired
   */
  isTokenExpired(): boolean {
    const token = this.getToken();
    if (!token) return true;

    try {
      const payload = this.decodeToken(token);
      if (!payload || !payload.exp) return true;

      const expirationDate = new Date(payload.exp * 1000);
      return expirationDate <= new Date();
    } catch {
      return true;
    }
  }
}