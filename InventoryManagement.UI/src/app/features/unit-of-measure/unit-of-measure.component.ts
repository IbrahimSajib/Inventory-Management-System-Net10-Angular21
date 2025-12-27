import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { UnitOfMeasureService } from '../../core/services/unit-of-measure.service';
import { UnitOfMeasure, CreateUnitOfMeasureRequest, UpdateUnitOfMeasureRequest } from '../../core/models/unit-of-measure';
import { AuthService } from '../../core/services/auth-service';

@Component({
  selector: 'app-unit-of-measure',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './unit-of-measure.component.html'
})
export class UnitOfMeasureComponent implements OnInit {
  private unitService = inject(UnitOfMeasureService);
  private authService = inject(AuthService);
  private fb = inject(FormBuilder);

  units = signal<UnitOfMeasure[]>([]);
  showModal = signal(false);
  isEditMode = signal(false);
  isSaving = signal(false);
  searchTerm = signal('');
  pageNumber = signal(1);
  pageSize = signal(10);
  totalCount = signal(0);
  totalPages = signal(0);
  hasNextPage = signal(false);

  unitForm!: FormGroup;
  editingId = signal<number | null>(null);

  protected Math = Math;

  ngOnInit() {
    this.initializeForm();
    this.loadUnits();
  }

  private initializeForm() {
    this.unitForm = this.fb.group({
      unitName: ['', [Validators.required]],
      shortName: [''],
      description: ['']
    });
  }

  private loadUnits() {
    this.isSaving.set(true);
    this.unitService
      .getAll(
        this.pageNumber(),
        this.pageSize(),
        this.searchTerm() || undefined
      )
      .subscribe({
        next: (result: any) => {
          this.units.set(result.items);
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
    this.unitForm.reset();
    this.showModal.set(true);
  }

  openEditModal(unit: UnitOfMeasure) {
    this.isEditMode.set(true);
    this.editingId.set(unit.id);
    this.unitForm.patchValue(unit);
    this.showModal.set(true);
  }

  closeModal() {
    this.showModal.set(false);
    this.unitForm.reset();
    this.editingId.set(null);
  }

  onSubmit() {
    if (this.unitForm.invalid) return;

    this.isSaving.set(true);
    const formValue = this.unitForm.value;

    if (this.isEditMode()) {
      const request: UpdateUnitOfMeasureRequest = formValue;
      this.unitService.update(this.editingId()!, request).subscribe({
        next: () => {
          this.isSaving.set(false);
          this.closeModal();
          this.loadUnits();
        },
        error: () => {
          this.isSaving.set(false);
        }
      });
    } else {
      const request: CreateUnitOfMeasureRequest = formValue;
      this.unitService.create(request).subscribe({
        next: () => {
          this.isSaving.set(false);
          this.closeModal();
          this.loadUnits();
        },
        error: () => {
          this.isSaving.set(false);
        }
      });
    }
  }

  deleteUnit(id: number) {
    if (confirm('Are you sure you want to delete this unit?')) {
      this.unitService.delete(id).subscribe({
        next: () => {
          this.loadUnits();
        }
      });
    }
  }

  onSearch() {
    this.pageNumber.set(1);
    this.loadUnits();
  }

  nextPage() {
    if (this.hasNextPage()) {
      this.pageNumber.update((p) => p + 1);
      this.loadUnits();
    }
  }

  previousPage() {
    if (this.pageNumber() > 1) {
      this.pageNumber.update((p) => p - 1);
      this.loadUnits();
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
    return this.unitForm.get(name);
  }
}