export interface Customer {
  id: number;
  name: string;
  email: string;
  phoneNumber?: string;
  address?: string;
  city?: string;
  state?: string;
  zipCode?: string;
  createdAt?: Date;
  updatedAt?: Date;
}

export interface CreateCustomerRequest {
  name: string;
  email: string;
  phoneNumber?: string;
  address?: string;
  city?: string;
  state?: string;
  zipCode?: string;
}

export interface UpdateCustomerRequest {
  id: number;
  name: string;
  email: string;
  phoneNumber?: string;
  address?: string;
  city?: string;
  state?: string;
  zipCode?: string;
}