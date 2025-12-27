import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpClient, HttpParams } from '@angular/common/http';
import { HttpService } from './http.service';
import {
  PurchaseOrder,
  CreatePurchaseOrderRequest,
  UpdatePurchaseOrderRequest,
  PagedPurchaseOrderResult
} from '../models/purchase-order';

@Injectable({
  providedIn: 'root'
})
export class PurchaseOrderService {
  private http = inject(HttpService);
  private httpClient = inject(HttpClient);
  private endpoint = '/PurchaseOrder';

  // Paginated list
  getAll(
    pageNumber: number = 1,
    pageSize: number = 10,
    searchTerm?: string,
    sortBy?: string,
    sortDescending?: boolean
  ): Observable<PagedPurchaseOrderResult> {
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

    return this.http.get<PagedPurchaseOrderResult>(this.endpoint, { params });
  }

  getById(id: number): Observable<PurchaseOrder> {
    return this.http.get<PurchaseOrder>(`${this.endpoint}/${id}`);
  }

  create(request: CreatePurchaseOrderRequest): Observable<PurchaseOrder> {
    return this.http.post<PurchaseOrder>(this.endpoint, request);
  }

  update(id: number, request: UpdatePurchaseOrderRequest): Observable<PurchaseOrder> {
    return this.http.put<PurchaseOrder>(`${this.endpoint}/${id}`, request);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.endpoint}/${id}`);
  }
}