import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ItemService } from '../../core/services/item.service';
import { CategoryService } from '../../core/services/category.service';
import { UnitOfMeasureService } from '../../core/services/unit-of-measure.service';
import { Item, CreateItemRequest, UpdateItemRequest } from '../../core/models/item';
import { Category } from '../../core/models/category';
import { UnitOfMeasure } from '../../core/models/unit-of-measure';
import { AuthService } from '../../core/services/auth-service';

@Component({
  selector: 'app-items',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './items.component.html',
})
export class ItemsComponent implements OnInit {
  private itemService = inject(ItemService);
  private categoryService = inject(CategoryService);
  private unitService = inject(UnitOfMeasureService);
  private authService = inject(AuthService);
  private fb = inject(FormBuilder);

  items = signal<Item[]>([]);
  categories = signal<Category[]>([]);
  unitOfMeasures = signal<UnitOfMeasure[]>([]);
  showModal = signal(false);
  isEditMode = signal(false);
  isSaving = signal(false);
  searchTerm = signal('');
  pageNumber = signal(1);
  pageSize = signal(10);
  totalCount = signal(0);
  totalPages = signal(0);
  hasNextPage = signal(false);

  itemForm!: FormGroup;
  currentItemId: number | null = null;

  protected Math = Math;

  ngOnInit() {
    this.initializeForm();
    this.loadCategories();
    this.loadUnitOfMeasures();
    this.loadItems();
  }

  private initializeForm() {
    this.itemForm = this.fb.group({
      itemCode: ['', [Validators.required]],
      itemName: ['', [Validators.required]],
      description: [''],
      categoryId: ['', [Validators.required]],
      unitOfMeasureId: ['', [Validators.required]],
      unitPrice: ['', [Validators.required, Validators.min(0.01)]],
      reorderLevel: [''],
      imageUrl: ['']
    });
  }

  private loadItems() {
    this.isSaving.set(true);
    this.itemService
      .getAll(
        this.pageNumber(),
        this.pageSize(),
        this.searchTerm() || undefined
      )
      .subscribe({
        next: (result: any) => {
          this.items.set(result.items);
          this.totalCount.set(result.totalCount);
          this.totalPages.set(result.totalPages);
          this.hasNextPage.set(result.hasNextPage);
          this.isSaving.set(false);
        },
        error: () => {
          this.isSaving.set(false);
        }
      });
  }

  private loadCategories() {
    this.categoryService.getAllList().subscribe({
      next: (data) => this.categories.set(data),
      error: () => {}
    });
  }

  private loadUnitOfMeasures() {
    this.unitService.getAllList().subscribe({
      next: (data) => this.unitOfMeasures.set(data),
      error: () => {}
    });
  }

  openCreateModal() {
    this.isEditMode.set(false);
    this.currentItemId = null;
    this.itemForm.reset();
    this.showModal.set(true);
  }

  openEditModal(item: Item) {
    this.isEditMode.set(true);
    this.currentItemId = item.id;
    this.itemForm.patchValue(item);
    this.showModal.set(true);
  }

  closeModal() {
    this.showModal.set(false);
    this.itemForm.reset();
    this.currentItemId = null;
  }

  onSubmit() {
    if (!this.itemForm.valid) return;

    this.isSaving.set(true);
    const request = this.itemForm.value;

    if (this.isEditMode() && this.currentItemId) {
      this.itemService.update(this.currentItemId, request).subscribe({
        next: () => {
          this.closeModal();
          this.loadItems();
          this.isSaving.set(false);
        },
        error: () => {
          this.isSaving.set(false);
        }
      });
    } else {
      this.itemService.create(request).subscribe({
        next: () => {
          this.closeModal();
          this.loadItems();
          this.isSaving.set(false);
        },
        error: () => {
          this.isSaving.set(false);
        }
      });
    }
  }

  deleteItem(id: number) {
    if (!confirm('Are you sure you want to delete this item?')) return;

    this.itemService.delete(id).subscribe({
      next: () => {
        this.loadItems();
      },
      error: () => {}
    });
  }

  onSearch() {
    this.pageNumber.set(1);
    this.loadItems();
  }

  nextPage() {
    if (this.hasNextPage()) {
      this.pageNumber.update((p) => p + 1);
      this.loadItems();
    }
  }

  previousPage() {
    if (this.pageNumber() > 1) {
      this.pageNumber.update((p) => p - 1);
      this.loadItems();
    }
  }

  hasCreatePermission(): boolean {
    return this.authService.hasAnyRole(['Admin', 'Manager']);
  }

  hasEditPermission(): boolean {
    return this.authService.hasAnyRole(['Admin', 'Manager']);
  }

  hasDeletePermission(): boolean {
    return this.authService.hasAnyRole(['Admin']);
  }

  getControl(name: string) {
    return this.itemForm.get(name);
  }
}