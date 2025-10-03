export interface Product {
  id: number;
  code: string;
  name: string;
  imagePath?: string;
  price: number;
  minimumQuantity: number;
  discountRate: number;
  categoryId: number;
  categoryName: string;
}

export interface ProductCreate {
  code: string;
  name: string;
  price: number;
  minimumQuantity: number;
  discountRate: number;
  categoryId: number;
  imageFile?: File;
}

export interface ProductUpdate {
  name: string;
  price: number;
  minimumQuantity: number;
  discountRate: number;
  categoryId: number;
  imageFile?: File | null;
}

export interface PagedResult<T> {
  items: T[]; // Changed from 'data' to 'items'
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
}