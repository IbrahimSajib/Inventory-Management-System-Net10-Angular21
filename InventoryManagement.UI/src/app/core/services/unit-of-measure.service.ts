import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpClient, HttpParams } from '@angular/common/http';
import { HttpService } from './http.service';
import {
  UnitOfMeasure,
  CreateUnitOfMeasureRequest,
  UpdateUnitOfMeasureRequest
} from '../models/unit-of-measure';
import { PagedResult } from '../models/api-response';

interface UnitOfMeasurePagedResult extends PagedResult<UnitOfMeasure> {}

@Injectable({
  providedIn: 'root'
})
export class UnitOfMeasureService {
  private http = inject(HttpService);
  private httpClient = inject(HttpClient);
  private endpoint = '/UnitOfMeasure';

  // Paginated list
  getAll(
    pageNumber: number = 1,
    pageSize: number = 10,
    searchTerm?: string,
    sortBy?: string,
    sortDescending?: boolean
  ): Observable<UnitOfMeasurePagedResult> {
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

    return this.http.get<UnitOfMeasurePagedResult>(this.endpoint, { params });
  }

  // Simple list (no pagination)
  getAllList(): Observable<UnitOfMeasure[]> {
    return this.http.get<UnitOfMeasure[]>(`${this.endpoint}/list`);
  }

  getById(id: number): Observable<UnitOfMeasure> {
    return this.http.get<UnitOfMeasure>(`${this.endpoint}/${id}`);
  }

  create(request: CreateUnitOfMeasureRequest): Observable<UnitOfMeasure> {
    return this.http.post<UnitOfMeasure>(this.endpoint, request);
  }

  update(id: number, request: UpdateUnitOfMeasureRequest): Observable<UnitOfMeasure> {
    return this.http.put<UnitOfMeasure>(`${this.endpoint}/${id}`, request);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.endpoint}/${id}`);
  }
}