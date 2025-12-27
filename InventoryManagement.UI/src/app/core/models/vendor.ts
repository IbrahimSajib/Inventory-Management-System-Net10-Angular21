export interface Vendor {
  id: number;
  vendorName: string;
  email?: string;
  phoneNumber?: string;
  address?: string;
  city?: string;
  country?: string;
  postalCode?: string;
  createdAt: Date;
}

export interface CreateVendorRequest {
  vendorName: string;
  email?: string;
  phoneNumber?: string;
  address?: string;
  city?: string;
  country?: string;
  postalCode?: string;
}

export interface UpdateVendorRequest {
  vendorName: string;
  email?: string;
  phoneNumber?: string;
  address?: string;
  city?: string;
  country?: string;
  postalCode?: string;
}