import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpService } from './http.service';
import {
  Customer,
  CreateCustomerRequest,
  UpdateCustomerRequest
} from '../models/customer';

@Injectable({
  providedIn: 'root'
})
export class CustomerService {
  private http = inject(HttpService);
  private endpoint = '/Customer';

  getAll(): Observable<Customer[]> {
    return this.http.get<Customer[]>(this.endpoint);
  }

  getById(id: number): Observable<Customer> {
    return this.http.get<Customer>(`${this.endpoint}/${id}`);
  }

  create(request: CreateCustomerRequest): Observable<Customer> {
    return this.http.post<Customer>(this.endpoint, request);
  }

  update(request: UpdateCustomerRequest): Observable<Customer> {
    return this.http.put<Customer>(
      `${this.endpoint}/${request.id}`,
      request
    );
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.endpoint}/${id}`);
  }
}