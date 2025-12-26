import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpService } from './http.service';
import {
  UnitOfMeasure,
  CreateUnitOfMeasureRequest,
  UpdateUnitOfMeasureRequest
} from '../models/unit-of-measure';

@Injectable({
  providedIn: 'root'
})
export class UnitOfMeasureService {
  private http = inject(HttpService);
  private endpoint = '/UnitOfMeasure';

  getAll(): Observable<UnitOfMeasure[]> {
    return this.http.get<UnitOfMeasure[]>(this.endpoint);
  }

  getById(id: number): Observable<UnitOfMeasure> {
    return this.http.get<UnitOfMeasure>(`${this.endpoint}/${id}`);
  }

  create(request: CreateUnitOfMeasureRequest): Observable<UnitOfMeasure> {
    return this.http.post<UnitOfMeasure>(this.endpoint, request);
  }

  update(request: UpdateUnitOfMeasureRequest): Observable<UnitOfMeasure> {
    return this.http.put<UnitOfMeasure>(
      `${this.endpoint}/${request.id}`,
      request
    );
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.endpoint}/${id}`);
  }
}