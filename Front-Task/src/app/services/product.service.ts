import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { Product, ProductCreate, ProductUpdate, PagedResult } from '../models/product.model';

@Injectable({
  providedIn: 'root'
})
export class ProductService {
  private apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  getAll(pageNumber: number = 1, pageSize: number = 10): Observable<PagedResult<Product>> {
    const params = new HttpParams()
      .set('pageNumber', pageNumber.toString())
      .set('pageSize', pageSize.toString());
    
    return this.http.get<PagedResult<Product>>(`${this.apiUrl}/product`, { params });
  }

  getById(id: number): Observable<Product> {
    return this.http.get<Product>(`${this.apiUrl}/product/${id}`);
  }

  getByCode(code: string): Observable<Product> {
    return this.http.get<Product>(`${this.apiUrl}/product/by-code/${code}`);
  }

  getByCategory(categoryId: number): Observable<Product[]> {
    return this.http.get<Product[]>(`${this.apiUrl}/product/by-category/${categoryId}`);
  }

  create(product: ProductCreate): Observable<Product> {
    const formData = new FormData();
    formData.append('code', product.code);
    formData.append('name', product.name);
    formData.append('price', product.price.toString());
    formData.append('minimumQuantity', product.minimumQuantity.toString());
    formData.append('discountRate', product.discountRate.toString());
    formData.append('categoryId', product.categoryId.toString());
    
    if (product.imageFile) {
      formData.append('imageFile', product.imageFile);
    } else {
      formData.append('imageFile', new Blob(), ''); // Append an empty Blob if no image is selected
    }

    return this.http.post<Product>(`${this.apiUrl}/product`, formData);
  }

  update(id: number, product: ProductUpdate): Observable<any> {
    const formData = new FormData();
    formData.append('name', product.name);
    formData.append('price', product.price.toString());
    formData.append('minimumQuantity', product.minimumQuantity.toString());
    formData.append('discountRate', product.discountRate.toString());
    formData.append('categoryId', product.categoryId.toString());
    
    if (product.imageFile) {
      formData.append('imageFile', product.imageFile);
    } else {
      formData.append('imageFile', new Blob(), ''); // Append an empty Blob if no image is selected
    }

    return this.http.put(`${this.apiUrl}/product/${id}`, formData);
  }

  delete(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/product/${id}`);
  }
}
