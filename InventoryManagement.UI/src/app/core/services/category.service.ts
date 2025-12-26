import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpService } from './http.service';
import { Category, CreateCategoryRequest, UpdateCategoryRequest } from '../models/category';

@Injectable({
  providedIn: 'root'
})
export class CategoryService {
  private http = inject(HttpService);
  private endpoint = '/Category';

  getAll(): Observable<Category[]> {
    return this.http.get<Category[]>(this.endpoint);
  }

  getById(id: number): Observable<Category> {
    return this.http.get<Category>(`${this.endpoint}/${id}`);
  }

  create(request: CreateCategoryRequest): Observable<Category> {
    return this.http.post<Category>(this.endpoint, request);
  }

  update(request: UpdateCategoryRequest): Observable<Category> {
    return this.http.put<Category>(`${this.endpoint}/${request.id}`, request);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.endpoint}/${id}`);
  }
}