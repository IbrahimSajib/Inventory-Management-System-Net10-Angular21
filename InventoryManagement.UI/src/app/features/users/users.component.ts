// src/app/features/users/users.component.ts
import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  FormBuilder,
  ReactiveFormsModule,
  FormGroup,
  Validators,
  FormsModule
} from '@angular/forms';
import { UserService } from '../../core/services/user.service';
import { User, CreateUserRequest, UpdateUserRequest, Role } from '../../core/models/user';

@Component({
  selector: 'app-users',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule],
  templateUrl: './users.component.html'
})
export class UsersComponent implements OnInit {
  private userService = inject(UserService);
  private fb = inject(FormBuilder);

  users = signal<User[]>([]);
  isLoading = signal(false);
  isSaving = signal(false);
  showModal = signal(false);
  editingId = signal<number | null>(null);
  searchQuery = signal('');
  selectedRoles = signal<number[]>([]);
  currentPage = signal(1);
  pageSize = signal(10);

  // Mock roles - replace with API call if needed
  availableRoles = signal<Role[]>([
    { id: 1, roleName: 'Admin' },
    { id: 2, roleName: 'Manager' },
    { id: 3, roleName: 'User' }
  ]);

  form: FormGroup;

  constructor() {
    this.form = this.fb.group({
      userName: ['', [Validators.required, Validators.minLength(3)]],
      email: ['', [Validators.required, Validators.email]],
      fullName: [''],
      phoneNumber: [''],
      password: ['', []],
      isActive: [true]
    });
  }

  ngOnInit(): void {
    this.loadUsers();
  }

  loadUsers(): void {
    this.isLoading.set(true);
    this.userService
      .getAll(this.currentPage(), this.pageSize(), this.searchQuery())
      .subscribe({
        next: (data) => {
          this.users.set(data.items);
          this.isLoading.set(false);
        },
        error: () => {
          this.isLoading.set(false);
        }
      });
  }

  get filteredUsers(): User[] {
    return this.users();
  }

  openModal(): void {
    this.editingId.set(null);
    this.selectedRoles.set([]);
    this.form.reset({ isActive: true });
    this.form.get('password')?.setValidators([Validators.required, Validators.minLength(6)]);
    this.form.get('password')?.updateValueAndValidity();
    this.showModal.set(true);
  }

  closeModal(): void {
    this.showModal.set(false);
    this.editingId.set(null);
    this.selectedRoles.set([]);
    this.form.reset();
  }

  editUser(user: User): void {
    this.editingId.set(user.id);
    this.form.patchValue({
      userName: user.userName,
      email: user.email,
      fullName: user.fullName,
      phoneNumber: user.phoneNumber,
      isActive: user.isActive
    });

    // Map user roles to role IDs - roles come as RoleInfo with id
    const roleIds = user.roles?.map(role => role.id) || [];
    this.selectedRoles.set(roleIds);

    // Disable password validation when editing
    this.form.get('password')?.setValidators([]);
    this.form.get('password')?.updateValueAndValidity();
    this.showModal.set(true);
  }

  toggleRole(roleId: number): void {
    const roles = this.selectedRoles();
    const index = roles.indexOf(roleId);
    if (index > -1) {
      this.selectedRoles.set([...roles.slice(0, index), ...roles.slice(index + 1)]);
    } else {
      this.selectedRoles.set([...roles, roleId]);
    }
  }

  isRoleSelected(roleId: number): boolean {
    return this.selectedRoles().includes(roleId);
  }

  save(): void {
    if (this.form.invalid) return;

    if (this.selectedRoles().length === 0) {
      alert('Please select at least one role');
      return;
    }

    this.isSaving.set(true);
    const formValue = this.form.value;

    if (this.editingId()) {
      // Update user with roles
      const request: UpdateUserRequest = {
        userName: formValue.userName,
        email: formValue.email,
        fullName: formValue.fullName,
        phoneNumber: formValue.phoneNumber,
        isActive: formValue.isActive,
        roleIds: this.selectedRoles()
      };

      this.userService.update(this.editingId()!, request).subscribe({
        next: () => {
          this.isSaving.set(false);
          this.closeModal();
          this.loadUsers();
        },
        error: () => {
          this.isSaving.set(false);
        }
      });
    } else {
      // Create user with roles
      const request: CreateUserRequest = {
        userName: formValue.userName,
        email: formValue.email,
        fullName: formValue.fullName,
        phoneNumber: formValue.phoneNumber,
        password: formValue.password,
        roleIds: this.selectedRoles()
      };

      this.userService.create(request).subscribe({
        next: () => {
          this.isSaving.set(false);
          this.closeModal();
          this.loadUsers();
        },
        error: () => {
          this.isSaving.set(false);
        }
      });
    }
  }

  deleteUser(id: number): void {
    if (confirm('Are you sure you want to delete this user?')) {
      this.userService.delete(id).subscribe({
        next: () => {
          this.loadUsers();
        }
      });
    }
  }

  onSearch(): void {
    this.currentPage.set(1);
    this.loadUsers();
  }
}