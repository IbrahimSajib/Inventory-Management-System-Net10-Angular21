export interface UnitOfMeasure {
  id: number;
  unitName: string;
  shortName?: string;
  description?: string;
  createdAt: Date;
}

export interface CreateUnitOfMeasureRequest {
  unitName: string;
  shortName?: string;
  description?: string;
}

export interface UpdateUnitOfMeasureRequest {
  unitName: string;
  shortName?: string;
  description?: string;
}