import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpService } from './http.service';
import {
  Vendor,
  CreateVendorRequest,
  UpdateVendorRequest
} from '../models/vendor';

@Injectable({
  providedIn: 'root'
})
export class VendorService {
  private http = inject(HttpService);
  private endpoint = '/Vendor';

  getAll(): Observable<Vendor[]> {
    return this.http.get<Vendor[]>(this.endpoint);
  }

  getById(id: number): Observable<Vendor> {
    return this.http.get<Vendor>(`${this.endpoint}/${id}`);
  }

  create(request: CreateVendorRequest): Observable<Vendor> {
    return this.http.post<Vendor>(this.endpoint, request);
  }

  update(request: UpdateVendorRequest): Observable<Vendor> {
    return this.http.put<Vendor>(
      `${this.endpoint}/${request.id}`,
      request
    );
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.endpoint}/${id}`);
  }
}