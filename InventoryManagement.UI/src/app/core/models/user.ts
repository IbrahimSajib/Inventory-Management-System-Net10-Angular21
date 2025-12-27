export interface User {
  id: number;
  userName: string;
  email: string;
  fullName?: string;
  phoneNumber?: string;
  isActive: boolean;
  createdAt?: Date;
  roles?: RoleInfo[];
}

export interface RoleInfo {
  id: number;
  roleName: string;
}

export interface CreateUserRequest {
  userName: string;
  email: string;
  fullName?: string;
  phoneNumber?: string;
  password: string;
  roleIds: number[];
}

export interface UpdateUserRequest {
  userName: string;
  email: string;
  fullName?: string;
  phoneNumber?: string;
  isActive: boolean;
  roleIds: number[];
}

export interface ChangePasswordRequest {
  userId: number;
  oldPassword: string;
  newPassword: string;
  confirmPassword: string;
}

export interface Role {
  id: number;
  roleName: string;
}

export interface PagedUserResult {
  items: User[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}