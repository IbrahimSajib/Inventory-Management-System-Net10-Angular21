import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, map, catchError, throwError } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../models/api-response';
import { NotificationService } from './notification.service';

@Injectable({
  providedIn: 'root'
})
export class HttpService {
  private http = inject(HttpClient);
  private notificationService = inject(NotificationService);
  private baseUrl = environment.apiUrl;

  get<T>(endpoint: string, options?: { params?: HttpParams }): Observable<T> {
    return this.http.get<ApiResponse<T>>(`${this.baseUrl}${endpoint}`, options)
      .pipe(
        map(response => {
          if (!response.success) {
            this.notificationService.showError(response.message || 'Failed to fetch data');
            throw new Error(response.message);
          }
          return response.data!;
        }),
        catchError(error => {
          this.notificationService.showError(error.message || 'An error occurred');
          return throwError(() => error);
        })
      );
  }

  post<T>(endpoint: string, body: any): Observable<T> {
    return this.http.post<ApiResponse<T>>(`${this.baseUrl}${endpoint}`, body)
      .pipe(
        map(response => {
          if (!response.success) {
            this.notificationService.showError(response.message || 'Operation failed');
            throw new Error(response.message);
          }
          this.notificationService.showSuccess('Created successfully');
          return response.data!;
        }),
        catchError(error => {
          this.notificationService.showError(error.message || 'An error occurred');
          return throwError(() => error);
        })
      );
  }

  put<T>(endpoint: string, body: any): Observable<T> {
    return this.http.put<ApiResponse<T>>(`${this.baseUrl}${endpoint}`, body)
      .pipe(
        map(response => {
          if (!response.success) {
            this.notificationService.showError(response.message || 'Update failed');
            throw new Error(response.message);
          }
          this.notificationService.showSuccess('Updated successfully');
          return response.data!;
        }),
        catchError(error => {
          this.notificationService.showError(error.message || 'An error occurred');
          return throwError(() => error);
        })
      );
  }

  delete<T>(endpoint: string): Observable<T> {
    return this.http.delete<ApiResponse<T>>(`${this.baseUrl}${endpoint}`)
      .pipe(
        map(response => {
          if (!response.success) {
            this.notificationService.showError(response.message || 'Delete failed');
            throw new Error(response.message);
          }
          this.notificationService.showSuccess('Deleted successfully');
          return response.data!;
        }),
        catchError(error => {
          this.notificationService.showError(error.message || 'An error occurred');
          return throwError(() => error);
        })
      );
  }
}