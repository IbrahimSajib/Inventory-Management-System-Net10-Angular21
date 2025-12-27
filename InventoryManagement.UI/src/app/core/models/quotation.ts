export enum QuotationStatus {
  Draft = 'Draft',
  Sent = 'Sent',
  Accepted = 'Accepted',
  Rejected = 'Rejected',
  Expired = 'Expired'
}

export interface Quotation {
  id: number;
  quotationNumber: string;
  quotationDate: Date;
  validUntil: Date;
  customerId: number;
  customerName: string;
  status: QuotationStatus;
  totalAmount: number;
  taxAmount?: number;
  discountAmount?: number;
  grandTotal: number;
  notes?: string;
  termsAndConditions?: string;
  createdAt: Date;
  items: QuotationItem[];
}

export interface QuotationItem {
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
  description?: string;
}

export interface CreateQuotationRequest {
  quotationDate: Date;
  validUntil: Date;
  customerId: number;
  notes?: string;
  termsAndConditions?: string;
  items: CreateQuotationItemRequest[];
}

export interface CreateQuotationItemRequest {
  itemId: number;
  quantity: number;
  unitPrice: number;
  discountPercent?: number;
  description?: string;
}

export interface UpdateQuotationRequest {
  quotationDate: Date;
  validUntil: Date;
  customerId: number;
  notes?: string;
  termsAndConditions?: string;
  items: CreateQuotationItemRequest[];
}

export interface PagedQuotationResult {
  items: Quotation[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}