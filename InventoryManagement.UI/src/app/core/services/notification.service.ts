import { Injectable, signal } from '@angular/core';

export interface Notification {
  id: string;
  message: string;
  type: 'success' | 'error' | 'info' | 'warning';
  duration?: number;
}

@Injectable({
  providedIn: 'root'
})
export class NotificationService {
  notifications = signal<Notification[]>([]);

  showSuccess(message: string, duration: number = 3000): void {
    this.addNotification(message, 'success', duration);
  }

  showError(message: string, duration: number = 5000): void {
    this.addNotification(message, 'error', duration);
  }

  showInfo(message: string, duration: number = 3000): void {
    this.addNotification(message, 'info', duration);
  }

  showWarning(message: string, duration: number = 4000): void {
    this.addNotification(message, 'warning', duration);
  }

  private addNotification(message: string, type: 'success' | 'error' | 'info' | 'warning', duration: number): void {
    const id = Date.now().toString();
    const notification: Notification = { id, message, type, duration };
    
    const currentNotifications = this.notifications();
    this.notifications.set([...currentNotifications, notification]);

    if (duration > 0) {
      setTimeout(() => {
        this.removeNotification(id);
      }, duration);
    }
  }

  removeNotification(id: string): void {
    this.notifications.set(this.notifications().filter(n => n.id !== id));
  }

  clearAll(): void {
    this.notifications.set([]);
  }
}