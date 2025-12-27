export interface Customer {
  id: number;
  customerName: string;
  email?: string;
  phoneNumber?: string;
  address?: string;
  city?: string;
  country?: string;
  postalCode?: string;
  createdAt: Date;
}

export interface CreateCustomerRequest {
  customerName: string;
  email?: string;
  phoneNumber?: string;
  address?: string;
  city?: string;
  country?: string;
  postalCode?: string;
}

export interface UpdateCustomerRequest {
  customerName: string;
  email?: string;
  phoneNumber?: string;
  address?: string;
  city?: string;
  country?: string;
  postalCode?: string;
}