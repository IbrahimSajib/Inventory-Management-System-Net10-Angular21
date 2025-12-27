import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpClient, HttpParams } from '@angular/common/http';
import { HttpService } from './http.service';
import {
  Customer,
  CreateCustomerRequest,
  UpdateCustomerRequest
} from '../models/customer';
import { PagedResult } from '../models/api-response';

interface CustomerPagedResult extends PagedResult<Customer> {}

@Injectable({
  providedIn: 'root'
})
export class CustomerService {
  private http = inject(HttpService);
  private httpClient = inject(HttpClient);
  private endpoint = '/Customer';

  // Paginated list
  getAll(
    pageNumber: number = 1,
    pageSize: number = 10,
    searchTerm?: string,
    sortBy?: string,
    sortDescending?: boolean
  ): Observable<CustomerPagedResult> {
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

    return this.http.get<CustomerPagedResult>(this.endpoint, { params });
  }

  // Simple list (no pagination)
  getAllList(): Observable<Customer[]> {
    return this.http.get<Customer[]>(`${this.endpoint}/list`);
  }

  getById(id: number): Observable<Customer> {
    return this.http.get<Customer>(`${this.endpoint}/${id}`);
  }

  create(request: CreateCustomerRequest): Observable<Customer> {
    return this.http.post<Customer>(this.endpoint, request);
  }

  update(id: number, request: UpdateCustomerRequest): Observable<Customer> {
    return this.http.put<Customer>(`${this.endpoint}/${id}`, request);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.endpoint}/${id}`);
  }
}