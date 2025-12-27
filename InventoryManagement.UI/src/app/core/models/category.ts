export interface Category {
  id: number;
  categoryName: string;
  description?: string;
  createdAt: Date;
}

export interface CreateCategoryRequest {
  categoryName: string;
  description?: string;
}

export interface UpdateCategoryRequest {
  categoryName: string;
  description?: string;
}