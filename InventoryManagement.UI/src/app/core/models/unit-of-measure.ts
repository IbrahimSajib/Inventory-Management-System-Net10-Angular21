export interface UnitOfMeasure {
  id: number;
  name: string;
  abbreviation: string;
  description?: string;
  createdAt?: Date;
  updatedAt?: Date;
}

export interface CreateUnitOfMeasureRequest {
  name: string;
  abbreviation: string;
  description?: string;
}

export interface UpdateUnitOfMeasureRequest {
  id: number;
  name: string;
  abbreviation: string;
  description?: string;
}