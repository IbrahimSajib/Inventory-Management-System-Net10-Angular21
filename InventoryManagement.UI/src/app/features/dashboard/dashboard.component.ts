import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../core/services/auth-service';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './dashboard.component.html'
})
export class DashboardComponent {
  private authService = inject(AuthService);
  currentUser = this.authService.currentUser;

  stats = signal([
    { label: 'Total Items', value: '1,234', icon: '📦', color: 'bg-blue-500' },
    { label: 'Total Customers', value: '567', icon: '👥', color: 'bg-green-500' },
    { label: 'Total Vendors', value: '89', icon: '🚚', color: 'bg-yellow-500' },
    { label: 'Pending Orders', value: '23', icon: '📋', color: 'bg-red-500' }
  ]);

  recentActivities = signal([
    { type: 'Order', description: 'New sales order created', timestamp: '2 hours ago' },
    { type: 'Item', description: 'Inventory updated for item #123', timestamp: '4 hours ago' },
    { type: 'Customer', description: 'New customer registered', timestamp: '1 day ago' }
  ]);
}