export enum OrderStatus {
  Draft = 'Draft',
  Pending = 'Pending',
  Confirmed = 'Confirmed',
  Received = 'Received',
  Cancelled = 'Cancelled'
}

export interface PurchaseOrder {
  id: number;
  orderNumber: string;
  orderDate: Date;
  vendorId: number;
  vendorName: string;
  status: OrderStatus;
  totalAmount: number;
  taxAmount?: number;
  discountAmount?: number;
  grandTotal: number;
  notes?: string;
  expectedDeliveryDate?: Date;
  actualDeliveryDate?: Date;
  createdAt: Date;
  items: PurchaseOrderItem[];
}

export interface PurchaseOrderItem {
  id: number;
  itemId: number;
  itemName: string;
  itemCode: string;
  quantity: number;
  unitPrice: number;
  totalPrice: number;
  discountPercent?: number;
  discountAmount?: number;
  netAmount: number;
}

export interface CreatePurchaseOrderRequest {
  orderDate: Date;
  vendorId: number;
  taxAmount?: number;
  discountAmount?: number;
  notes?: string;
  expectedDeliveryDate?: Date;
  items: CreatePurchaseOrderItemRequest[];
}

export interface CreatePurchaseOrderItemRequest {
  itemId: number;
  quantity: number;
  unitPrice: number;
  discountPercent?: number;
}

export interface UpdatePurchaseOrderRequest {
  orderDate: Date;
  vendorId: number;
  taxAmount?: number;
  discountAmount?: number;
  notes?: string;
  expectedDeliveryDate?: Date;
  items: CreatePurchaseOrderItemRequest[];
}

export interface PagedPurchaseOrderResult {
  items: PurchaseOrder[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}