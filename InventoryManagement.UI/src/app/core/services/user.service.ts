import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpClient, HttpParams } from '@angular/common/http';
import { HttpService } from './http.service';
import {
  User,
  CreateUserRequest,
  UpdateUserRequest,
  ChangePasswordRequest,
  PagedUserResult
} from '../models/user';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private httpService = inject(HttpService);

  getAll(
    pageNumber: number = 1,
    pageSize: number = 10,
    searchTerm?: string,
    sortBy?: string,
    sortDescending?: boolean
  ): Observable<PagedUserResult> {
    let params = new HttpParams()
      .set('pageNumber', pageNumber)
      .set('pageSize', pageSize);

    if (searchTerm) {
      params = params.set('searchTerm', searchTerm);
    }
    if (sortBy) {
      params = params.set('sortBy', sortBy);
    }
    if (sortDescending !== undefined) {
      params = params.set('sortDescending', sortDescending);
    }

    return this.httpService.get<PagedUserResult>('/User', { params });
  }

  getById(id: number): Observable<User> {
    return this.httpService.get<User>(`/User/${id}`);
  }

  create(request: CreateUserRequest): Observable<User> {
    return this.httpService.post<User>('/User', request);
  }

  update(id: number, request: UpdateUserRequest): Observable<User> {
    return this.httpService.put<User>(`/User/${id}`, request);
  }

  delete(id: number): Observable<void> {
    return this.httpService.delete<void>(`/User/${id}`);
  }

  changePassword(request: ChangePasswordRequest): Observable<void> {
    return this.httpService.post<void>('/User/change-password', request);
  }
}