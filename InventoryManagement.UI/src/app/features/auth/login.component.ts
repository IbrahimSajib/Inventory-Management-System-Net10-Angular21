import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../core/services/auth-service';
import { LoginRequest } from '../../core/models/auth';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './login.component.html'
})
export class LoginComponent {
  private authService = inject(AuthService);
  private router = inject(Router);

  userName = '';
  password = '';
  isLoading = signal(false);
  showPassword = signal(false);

  login(): void {
    if (!this.userName || !this.password) {
      return;
    }

    this.isLoading.set(true);
    const request: LoginRequest = {
      userName: this.userName,
      password: this.password
    };

    this.authService.login(request).subscribe({
      next: () => {
        this.isLoading.set(false);
        this.router.navigate(['/dashboard']);
      },
      error: () => {
        this.isLoading.set(false);
      }
    });
  }

  togglePasswordVisibility(): void {
    this.showPassword.set(!this.showPassword());
  }

  isFormValid(): boolean {
    return this.userName.trim().length > 0 && this.password.length > 0;
  }
}