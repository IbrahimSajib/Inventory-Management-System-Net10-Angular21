import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NotificationService } from '../../core/services/notification.service';

@Component({
  selector: 'app-toast',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="fixed top-4 right-4 z-50 space-y-2 max-w-md">
      @for (notification of notifications(); track notification.id) {
        <div 
          [ngClass]="getToastClasses(notification.type)"
          class="px-4 py-3 rounded-lg shadow-lg flex items-center justify-between animate-in fade-in slide-in-from-top-2 duration-300"
        >
          <div class="flex items-center gap-2">
            <span [ngClass]="getIconClass(notification.type)">
              @switch (notification.type) {
                @case ('success') { ✓ }
                @case ('error') { ✕ }
                @case ('warning') { ⚠ }
                @case ('info') { ℹ }
              }
            </span>
            <span>{{ notification.message }}</span>
          </div>
          <button 
            (click)="close(notification.id)"
            class="ml-4 text-lg font-bold opacity-70 hover:opacity-100"
          >
            ×
          </button>
        </div>
      }
    </div>
  `,
  styles: [`
    :host {
      pointer-events: none;
    }
    div > div {
      pointer-events: auto;
    }
  `]
})
export class ToastComponent {
  private notificationService = inject(NotificationService);
  protected notifications = this.notificationService.notifications;

  getToastClasses(type: string): string {
    const baseClasses = 'text-white font-medium';
    switch (type) {
      case 'success':
        return `${baseClasses} bg-green-500`;
      case 'error':
        return `${baseClasses} bg-red-500`;
      case 'warning':
        return `${baseClasses} bg-yellow-500`;
      case 'info':
        return `${baseClasses} bg-blue-500`;
      default:
        return `${baseClasses} bg-gray-500`;
    }
  }

  getIconClass(type: string): string {
    return `text-lg font-bold`;
  }

  close(id: string): void {
    this.notificationService.removeNotification(id);
  }
}