import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpClient, HttpParams } from '@angular/common/http';
import { HttpService } from './http.service';
import {
  SalesOrder,
  CreateSalesOrderRequest,
  UpdateSalesOrderRequest,
  PagedSalesOrderResult
} from '../models/sales-order';

@Injectable({
  providedIn: 'root'
})
export class SalesOrderService {
  private http = inject(HttpService);
  private httpClient = inject(HttpClient);
  private endpoint = '/SalesOrder';

  // Paginated list
  getAll(
    pageNumber: number = 1,
    pageSize: number = 10,
    searchTerm?: string,
    sortBy?: string,
    sortDescending?: boolean
  ): Observable<PagedSalesOrderResult> {
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

    return this.http.get<PagedSalesOrderResult>(this.endpoint, { params });
  }

  getById(id: number): Observable<SalesOrder> {
    return this.http.get<SalesOrder>(`${this.endpoint}/${id}`);
  }

  create(request: CreateSalesOrderRequest): Observable<SalesOrder> {
    return this.http.post<SalesOrder>(this.endpoint, request);
  }

  update(id: number, request: UpdateSalesOrderRequest): Observable<SalesOrder> {
    return this.http.put<SalesOrder>(`${this.endpoint}/${id}`, request);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.endpoint}/${id}`);
  }
}