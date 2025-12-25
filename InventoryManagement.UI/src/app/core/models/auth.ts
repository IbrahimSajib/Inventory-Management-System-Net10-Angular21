export interface LoginRequest {
  userName: string;
  password: string;
}

export interface RegisterRequest {
  userName: string;
  email: string;
  password: string;
  confirmPassword: string;
  fullName?: string;
  phoneNumber?: string;
  roleIds: number[];
}

export interface LoginResponse {
  accessToken: string;
  refreshToken: string;
  expiresAt: string;
  user: UserInfo;
}

export interface UserInfo {
  id: number;
  userName: string;
  email: string;
  fullName?: string;
  roles: string[];
}

export interface RefreshTokenRequest {
  refreshToken: string;
}