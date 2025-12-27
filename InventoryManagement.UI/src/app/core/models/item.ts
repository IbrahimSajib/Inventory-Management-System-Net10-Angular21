export interface Item {
  id: number;
  itemCode: string;
  itemName: string;
  categoryId: number;
  categoryName: string;
  unitOfMeasureId: number;
  unitOfMeasureName: string;
  unitPrice: number;
  reorderLevel?: number;
  imageUrl?: string;
  quantityOnHand: number;
  quantityAvailable: number;
  createdAt: Date;
}

export interface CreateItemRequest {
  itemCode: string;
  itemName: string;
  description?: string;
  categoryId: number;
  unitOfMeasureId: number;
  unitPrice: number;
  reorderLevel?: number;
  imageUrl?: string;
}

export interface UpdateItemRequest {
  itemCode: string;
  itemName: string;
  description?: string;
  categoryId: number;
  unitOfMeasureId: number;
  unitPrice: number;
  reorderLevel?: number;
  imageUrl?: string;
}