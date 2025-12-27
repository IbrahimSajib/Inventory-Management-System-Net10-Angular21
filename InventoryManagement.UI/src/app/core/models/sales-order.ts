export enum OrderStatus {
  Draft = 'Draft',
  Pending = 'Pending',
  Confirmed = 'Confirmed',
  Shipped = 'Shipped',
  Delivered = 'Delivered',
  Cancelled = 'Cancelled'
}

export interface SalesOrder {
  id: number;
  orderNumber: string;
  orderDate: Date;
  customerId: number;
  customerName: string;
  status: OrderStatus;
  totalAmount: number;
  taxAmount?: number;
  discountAmount?: number;
  grandTotal: number;
  notes?: string;
  expectedDeliveryDate?: Date;
  actualDeliveryDate?: Date;
  shippingAddress?: string;
  createdAt: Date;
  items: SalesOrderItem[];
}

export interface SalesOrderItem {
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

export interface CreateSalesOrderRequest {
  orderDate: Date;
  customerId: number;
  taxAmount?: number;
  discountAmount?: number;
  notes?: string;
  expectedDeliveryDate?: Date;
  shippingAddress?: string;
  items: CreateSalesOrderItemRequest[];
}

export interface CreateSalesOrderItemRequest {
  itemId: number;
  quantity: number;
  unitPrice: number;
  discountPercent?: number;
}

export interface UpdateSalesOrderRequest {
  orderDate: Date;
  customerId: number;
  taxAmount?: number;
  discountAmount?: number;
  notes?: string;
  expectedDeliveryDate?: Date;
  shippingAddress?: string;
  items: CreateSalesOrderItemRequest[];
}

export interface PagedSalesOrderResult {
  items: SalesOrder[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}