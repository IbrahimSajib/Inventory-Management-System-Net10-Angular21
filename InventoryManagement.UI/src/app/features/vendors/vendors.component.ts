import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { VendorService } from '../../core/services/vendor.service';
import { Vendor, CreateVendorRequest, UpdateVendorRequest } from '../../core/models/vendor';
import { AuthService } from '../../core/services/auth-service';

@Component({
  selector: 'app-vendors',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './vendors.component.html',
})
export class VendorsComponent implements OnInit {
  private vendorService = inject(VendorService);
  private authService = inject(AuthService);
  private fb = inject(FormBuilder);

  vendors = signal<Vendor[]>([]);
  showModal = signal(false);
  isEditMode = signal(false);
  isSaving = signal(false);
  searchTerm = signal('');
  pageNumber = signal(1);
  pageSize = signal(10);
  totalCount = signal(0);
  totalPages = signal(0);
  hasNextPage = signal(false);

  vendorForm!: FormGroup;
  editingId = signal<number | null>(null);

  protected Math = Math;

  ngOnInit() {
    this.initializeForm();
    this.loadVendors();
  }

  private initializeForm() {
    this.vendorForm = this.fb.group({
      vendorName: ['', [Validators.required]],
      email: ['', [Validators.email]],
      phoneNumber: [''],
      address: [''],
      city: [''],
      country: [''],
      postalCode: ['']
    });
  }

  private loadVendors() {
    this.isSaving.set(true);
    this.vendorService
      .getAll(
        this.pageNumber(),
        this.pageSize(),
        this.searchTerm() || undefined
      )
      .subscribe({
        next: (result: any) => {
          this.vendors.set(result.items);
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

  openCreateModal() {
    this.isEditMode.set(false);
    this.editingId.set(null);
    this.vendorForm.reset();
    this.showModal.set(true);
  }

  openEditModal(vendor: Vendor) {
    this.isEditMode.set(true);
    this.editingId.set(vendor.id);
    this.vendorForm.patchValue(vendor);
    this.showModal.set(true);
  }

  closeModal() {
    this.showModal.set(false);
    this.vendorForm.reset();
    this.editingId.set(null);
  }

  onSubmit() {
    if (this.vendorForm.invalid) return;

    this.isSaving.set(true);
    const formValue = this.vendorForm.value;

    if (this.isEditMode()) {
      const request: UpdateVendorRequest = formValue;
      this.vendorService.update(this.editingId()!, request).subscribe({
        next: () => {
          this.isSaving.set(false);
          this.closeModal();
          this.loadVendors();
        },
        error: () => {
          this.isSaving.set(false);
        }
      });
    } else {
      const request: CreateVendorRequest = formValue;
      this.vendorService.create(request).subscribe({
        next: () => {
          this.isSaving.set(false);
          this.closeModal();
          this.loadVendors();
        },
        error: () => {
          this.isSaving.set(false);
        }
      });
    }
  }

  deleteVendor(id: number) {
    if (confirm('Are you sure you want to delete this vendor?')) {
      this.vendorService.delete(id).subscribe({
        next: () => {
          this.loadVendors();
        }
      });
    }
  }

  onSearch() {
    this.pageNumber.set(1);
    this.loadVendors();
  }

  nextPage() {
    if (this.hasNextPage()) {
      this.pageNumber.update((p) => p + 1);
      this.loadVendors();
    }
  }

  previousPage() {
    if (this.pageNumber() > 1) {
      this.pageNumber.update((p) => p - 1);
      this.loadVendors();
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
    return this.vendorForm.get(name);
  }
}