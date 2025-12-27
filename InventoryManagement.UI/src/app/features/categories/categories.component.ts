import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, FormGroup, Validators } from '@angular/forms';
import { CategoryService } from '../../core/services/category.service';
import { Category, CreateCategoryRequest, UpdateCategoryRequest } from '../../core/models/category';

@Component({
  selector: 'app-categories',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './categories.component.html'
})
export class CategoriesComponent implements OnInit {
  private categoryService = inject(CategoryService);
  private fb = inject(FormBuilder);

  categories = signal<Category[]>([]);
  isLoading = signal(false);
  isSaving = signal(false);
  showModal = signal(false);
  editingId = signal<number | null>(null);
  searchQuery = signal('');
  currentPage = signal(1);
  pageSize = signal(10);

  form: FormGroup;

  constructor() {
    this.form = this.fb.group({
      categoryName: ['', Validators.required],
      description: ['']
    });
  }

  ngOnInit(): void {
    this.loadCategories();
  }

  loadCategories(): void {
    this.isLoading.set(true);
    this.categoryService
      .getAll(this.currentPage(), this.pageSize(), this.searchQuery())
      .subscribe({
        next: (data) => {
          this.categories.set(data.items);
          this.isLoading.set(false);
        },
        error: () => {
          this.isLoading.set(false);
        }
      });
  }

  openModal(): void {
    this.editingId.set(null);
    this.form.reset();
    this.showModal.set(true);
  }

  closeModal(): void {
    this.showModal.set(false);
    this.editingId.set(null);
    this.form.reset();
  }

  editCategory(category: Category): void {
    this.editingId.set(category.id);
    this.form.patchValue({
      categoryName: category.categoryName,
      description: category.description
    });
    this.showModal.set(true);
  }

  save(): void {
    if (this.form.invalid) return;

    this.isSaving.set(true);
    const formValue = this.form.value;

    if (this.editingId()) {
      // Update
      const request: UpdateCategoryRequest = {
        categoryName: formValue.categoryName,
        description: formValue.description
      };

      this.categoryService.update(this.editingId()!, request).subscribe({
        next: () => {
          this.isSaving.set(false);
          this.closeModal();
          this.loadCategories();
        },
        error: () => {
          this.isSaving.set(false);
        }
      });
    } else {
      // Create
      const request: CreateCategoryRequest = {
        categoryName: formValue.categoryName,
        description: formValue.description
      };

      this.categoryService.create(request).subscribe({
        next: () => {
          this.isSaving.set(false);
          this.closeModal();
          this.loadCategories();
        },
        error: () => {
          this.isSaving.set(false);
        }
      });
    }
  }

  deleteCategory(id: number): void {
    if (confirm('Are you sure you want to delete this category?')) {
      this.categoryService.delete(id).subscribe({
        next: () => {
          this.loadCategories();
        }
      });
    }
  }

  onSearch(): void {
    this.currentPage.set(1);
    this.loadCategories();
  }
}