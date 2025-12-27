import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpClient, HttpParams } from '@angular/common/http';
import { HttpService } from './http.service';
import {
  Category,
  CreateCategoryRequest,
  UpdateCategoryRequest
} from '../models/category';
import { ApiResponse, PagedResult } from '../models/api-response';

interface CategoryPagedResult extends PagedResult<Category> {}

@Injectable({
  providedIn: 'root'
})
export class CategoryService {
  private httpService = inject(HttpService);
  private http = inject(HttpClient);
  private baseUrl = 'https://localhost:44307/api';  // Or use environment.apiUrl
  private endpoint = '/Category';

  // Paginated list
  getAll(
    pageNumber: number = 1,
    pageSize: number = 10,
    searchTerm?: string,
    sortBy?: string,
    sortDescending?: boolean
  ): Observable<CategoryPagedResult> {
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

    return this.httpService.get<CategoryPagedResult>(this.endpoint, { params });
  }

  // Simple list (no pagination)
  getAllList(): Observable<Category[]> {
    return this.httpService.get<Category[]>(`${this.endpoint}/list`);
  }

  getById(id: number): Observable<Category> {
    return this.httpService.get<Category>(`${this.endpoint}/${id}`);
  }

  create(request: CreateCategoryRequest): Observable<Category> {
    return this.httpService.post<Category>(this.endpoint, request);
  }

  update(id: number, request: UpdateCategoryRequest): Observable<Category> {
    return this.httpService.put<Category>(`${this.endpoint}/${id}`, request);
  }

  delete(id: number): Observable<void> {
    return this.httpService.delete<void>(`${this.endpoint}/${id}`);
  }
}