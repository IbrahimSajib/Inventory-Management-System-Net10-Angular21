import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpClient, HttpParams } from '@angular/common/http';
import { HttpService } from './http.service';
import {
  Vendor,
  CreateVendorRequest,
  UpdateVendorRequest
} from '../models/vendor';
import { PagedResult } from '../models/api-response';

interface VendorPagedResult extends PagedResult<Vendor> {}

@Injectable({
  providedIn: 'root'
})
export class VendorService {
  private http = inject(HttpService);
  private httpClient = inject(HttpClient);
  private endpoint = '/Vendor';

  // Paginated list
  getAll(
    pageNumber: number = 1,
    pageSize: number = 10,
    searchTerm?: string,
    sortBy?: string,
    sortDescending?: boolean
  ): Observable<VendorPagedResult> {
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

    return this.http.get<VendorPagedResult>(this.endpoint, { params });
  }

  // Simple list (no pagination)
  getAllList(): Observable<Vendor[]> {
    return this.http.get<Vendor[]>(`${this.endpoint}/list`);
  }

  getById(id: number): Observable<Vendor> {
    return this.http.get<Vendor>(`${this.endpoint}/${id}`);
  }

  create(request: CreateVendorRequest): Observable<Vendor> {
    return this.http.post<Vendor>(this.endpoint, request);
  }

  update(id: number, request: UpdateVendorRequest): Observable<Vendor> {
    return this.http.put<Vendor>(`${this.endpoint}/${id}`, request);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.endpoint}/${id}`);
  }
}