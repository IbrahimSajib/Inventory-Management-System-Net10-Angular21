export interface Item {
  id: number;
  name: string;
  description?: string;
  sku: string;
  categoryId: number;
  unitOfMeasureId: number;
  unitPrice: number;
  reorderLevel: number;
  createdAt?: Date;
  updatedAt?: Date;
  category?: any;
  unitOfMeasure?: any;
}

export interface CreateItemRequest {
  name: string;
  description?: string;
  sku: string;
  categoryId: number;
  unitOfMeasureId: number;
  unitPrice: number;
  reorderLevel: number;
}

export interface UpdateItemRequest {
  id: number;
  name: string;
  description?: string;
  sku: string;
  categoryId: number;
  unitOfMeasureId: number;
  unitPrice: number;
  reorderLevel: number;
}