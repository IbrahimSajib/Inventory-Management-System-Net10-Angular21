import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  FormBuilder,
  ReactiveFormsModule,
  FormGroup,
  Validators
} from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../core/services/auth-service';
import { RegisterRequest } from '../../core/models/auth';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './register.component.html'
})
export class RegisterComponent {
  private authService = inject(AuthService);
  private router = inject(Router);
  private fb = inject(FormBuilder);

  form: FormGroup;
  isLoading = signal(false);
  showPassword = signal(false);
  showConfirmPassword = signal(false);
  selectedRoles = signal<number[]>([]);

  // Mock roles
  availableRoles = [
    { id: 1, name: 'Admin' },
    { id: 2, name: 'Manager' },
    { id: 3, name: 'User' }
  ];

  constructor() {
    this.form = this.fb.group(
      {
        userName: ['', [Validators.required, Validators.minLength(3)]],
        email: ['', [Validators.required, Validators.email]],
        fullName: [''],
        phoneNumber: [''],
        password: ['', [Validators.required, Validators.minLength(6)]],
        confirmPassword: ['', Validators.required]
      },
      { validators: this.passwordMatchValidator }
    );
  }

  register(): void {
    if (this.form.invalid || this.selectedRoles().length === 0) {
      return;
    }

    this.isLoading.set(true);
    const request: RegisterRequest = {
      ...this.form.value,
      roleIds: this.selectedRoles()
    };

    this.authService.register(request).subscribe({
      next: () => {
        this.isLoading.set(false);
        this.router.navigate(['/dashboard']);
      },
      error: () => {
        this.isLoading.set(false);
      }
    });
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

  togglePasswordVisibility(field: 'password' | 'confirm'): void {
    if (field === 'password') {
      this.showPassword.set(!this.showPassword());
    } else {
      this.showConfirmPassword.set(!this.showConfirmPassword());
    }
  }

  private passwordMatchValidator(group: FormGroup): { [key: string]: boolean } | null {
    const password = group.get('password')?.value;
    const confirmPassword = group.get('confirmPassword')?.value;
    return password === confirmPassword ? null : { passwordMismatch: true };
  }

  getPasswordErrorMessage(): string {
    const passwordControl = this.form.get('password');
    if (passwordControl?.hasError('required')) {
      return 'Password is required';
    }
    if (passwordControl?.hasError('minlength')) {
      return 'Password must be at least 6 characters';
    }
    return '';
  }

  getPasswordMatchError(): string {
    if (this.form.hasError('passwordMismatch')) {
      return 'Passwords do not match';
    }
    return '';
  }
}