import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet } from '@angular/router';
import { AuthService } from '../../core/services/auth-service';

@Component({
  selector: 'app-layout',
  standalone: true,
  imports: [CommonModule, RouterOutlet],
  templateUrl: './layout.component.html'
})
export class LayoutComponent {
  private authService = inject(AuthService);

  sidenavOpen = signal(true);
  currentUser = this.authService.currentUser;

  menuItems = signal([
    { label: 'Dashboard', icon: '📊', route: '/dashboard' },
    { label: 'Users', icon: '👤', route: '/users' },
    { label: 'Categories', icon: '📁', route: '/categories' },
    { label: 'Items', icon: '📦', route: '/items' },
    { label: 'Unit of Measure', icon: '⚖️', route: '/unit-of-measure' },
    { label: 'Customers', icon: '👥', route: '/customers' },
    { label: 'Vendors', icon: '🚚', route: '/vendors' },
    { label: 'Purchase Orders', icon: '📋', route: '/purchase-orders' },
    { label: 'Sales Orders', icon: '💳', route: '/sales-orders' },
    { label: 'Quotations', icon: '💬', route: '/quotations' },
    // { label: 'Reports', icon: '📈', route: '/reports' }
  ]);

  toggleSidenav(): void {
    this.sidenavOpen.set(!this.sidenavOpen());
  }

  logout(): void {
    this.authService.logout();
  }

  isAdmin(): boolean {
    return this.authService.hasRole('Admin');
  }

  isManager(): boolean {
    return this.authService.hasRole('Manager');
  }
}