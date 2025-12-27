import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpClient, HttpParams } from '@angular/common/http';
import { HttpService } from './http.service';
import {
  Quotation,
  CreateQuotationRequest,
  UpdateQuotationRequest,
  PagedQuotationResult
} from '../models/quotation';

@Injectable({
  providedIn: 'root'
})
export class QuotationService {
  private http = inject(HttpService);
  private httpClient = inject(HttpClient);
  private endpoint = '/Quotation';

  // Paginated list
  getAll(
    pageNumber: number = 1,
    pageSize: number = 10,
    searchTerm?: string,
    sortBy?: string,
    sortDescending?: boolean
  ): Observable<PagedQuotationResult> {
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

    return this.http.get<PagedQuotationResult>(this.endpoint, { params });
  }

  getById(id: number): Observable<Quotation> {
    return this.http.get<Quotation>(`${this.endpoint}/${id}`);
  }

  create(request: CreateQuotationRequest): Observable<Quotation> {
    return this.http.post<Quotation>(this.endpoint, request);
  }

  update(id: number, request: UpdateQuotationRequest): Observable<Quotation> {
    return this.http.put<Quotation>(`${this.endpoint}/${id}`, request);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.endpoint}/${id}`);
  }
}