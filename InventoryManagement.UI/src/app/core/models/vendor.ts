export interface Vendor {
  id: number;
  name: string;
  email: string;
  phoneNumber?: string;
  address?: string;
  city?: string;
  state?: string;
  zipCode?: string;
  contactPerson?: string;
  createdAt?: Date;
  updatedAt?: Date;
}

export interface CreateVendorRequest {
  name: string;
  email: string;
  phoneNumber?: string;
  address?: string;
  city?: string;
  state?: string;
  zipCode?: string;
  contactPerson?: string;
}

export interface UpdateVendorRequest {
  id: number;
  name: string;
  email: string;
  phoneNumber?: string;
  address?: string;
  city?: string;
  state?: string;
  zipCode?: string;
  contactPerson?: string;
}