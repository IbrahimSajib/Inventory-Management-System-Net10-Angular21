import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpService } from './http.service';
import {
  Item,
  CreateItemRequest,
  UpdateItemRequest
} from '../models/item';

@Injectable({
  providedIn: 'root'
})
export class ItemService {
  private http = inject(HttpService);
  private endpoint = '/Item';

  getAll(): Observable<Item[]> {
    return this.http.get<Item[]>(this.endpoint);
  }

  getById(id: number): Observable<Item> {
    return this.http.get<Item>(`${this.endpoint}/${id}`);
  }

  create(request: CreateItemRequest): Observable<Item> {
    return this.http.post<Item>(this.endpoint, request);
  }

  update(request: UpdateItemRequest): Observable<Item> {
    return this.http.put<Item>(`${this.endpoint}/${request.id}`, request);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.endpoint}/${id}`);
  }
}