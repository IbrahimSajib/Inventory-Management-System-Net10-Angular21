import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { CustomerService } from '../../core/services/customer.service';
import { Customer, CreateCustomerRequest, UpdateCustomerRequest } from '../../core/models/customer';
import { AuthService } from '../../core/services/auth-service';

@Component({
  selector: 'app-customers',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './customers.component.html',
})
export class CustomersComponent implements OnInit {
  private customerService = inject(CustomerService);
  private authService = inject(AuthService);
  private fb = inject(FormBuilder);

  customers = signal<Customer[]>([]);
  showModal = signal(false);
  isEditMode = signal(false);
  isSaving = signal(false);
  searchTerm = signal('');
  pageNumber = signal(1);
  pageSize = signal(10);
  totalCount = signal(0);
  totalPages = signal(0);
  hasNextPage = signal(false);

  customerForm!: FormGroup;
  editingId = signal<number | null>(null);

  protected Math = Math;

  ngOnInit() {
    this.initializeForm();
    this.loadCustomers();
  }

  private initializeForm() {
    this.customerForm = this.fb.group({
      customerName: ['', [Validators.required]],
      email: ['', [Validators.email]],
      phoneNumber: [''],
      address: [''],
      city: [''],
      country: [''],
      postalCode: ['']
    });
  }

  private loadCustomers() {
    this.isSaving.set(true);
    this.customerService
      .getAll(
        this.pageNumber(),
        this.pageSize(),
        this.searchTerm() || undefined
      )
      .subscribe({
        next: (result: any) => {
          this.customers.set(result.items);
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
    this.customerForm.reset();
    this.showModal.set(true);
  }

  openEditModal(customer: Customer) {
    this.isEditMode.set(true);
    this.editingId.set(customer.id);
    this.customerForm.patchValue(customer);
    this.showModal.set(true);
  }

  closeModal() {
    this.showModal.set(false);
    this.customerForm.reset();
    this.editingId.set(null);
  }

  onSubmit() {
    if (this.customerForm.invalid) return;

    this.isSaving.set(true);
    const formValue = this.customerForm.value;

    if (this.isEditMode()) {
      const request: UpdateCustomerRequest = formValue;
      this.customerService.update(this.editingId()!, request).subscribe({
        next: () => {
          this.isSaving.set(false);
          this.closeModal();
          this.loadCustomers();
        },
        error: () => {
          this.isSaving.set(false);
        }
      });
    } else {
      const request: CreateCustomerRequest = formValue;
      this.customerService.create(request).subscribe({
        next: () => {
          this.isSaving.set(false);
          this.closeModal();
          this.loadCustomers();
        },
        error: () => {
          this.isSaving.set(false);
        }
      });
    }
  }

  deleteCustomer(id: number) {
    if (confirm('Are you sure you want to delete this customer?')) {
      this.customerService.delete(id).subscribe({
        next: () => {
          this.loadCustomers();
        }
      });
    }
  }

  onSearch() {
    this.pageNumber.set(1);
    this.loadCustomers();
  }

  nextPage() {
    if (this.hasNextPage()) {
      this.pageNumber.update((p) => p + 1);
      this.loadCustomers();
    }
  }

  previousPage() {
    if (this.pageNumber() > 1) {
      this.pageNumber.update((p) => p - 1);
      this.loadCustomers();
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
    return this.customerForm.get(name);
  }
}