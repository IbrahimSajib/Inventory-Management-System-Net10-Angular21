import { Routes } from '@angular/router';
import { LoginComponent } from './features/auth/login.component';
import { RegisterComponent } from './features/auth/register.component';
import { authGuard, roleGuard } from './core/guards/auth.guard';
import { LayoutComponent } from './shared/components/layout.component';
import { DashboardComponent } from './features/dashboard/dashboard.component';
import { CategoriesComponent } from './features/categories/categories.component';
import { UsersComponent } from './features/users/users.component';

export const routes: Routes = [
  {
    path: 'auth',
    children: [
      {
        path: 'login',
        component: LoginComponent
      },
      {
        path: 'register',
        component: RegisterComponent
      },
      {
        path: '',
        redirectTo: 'login',
        pathMatch: 'full'
      }
    ]
  },
  {
    path: '',
    component: LayoutComponent,
    canActivate: [authGuard],
    children: [
      {
        path: 'dashboard',
        component: DashboardComponent
      },
      {
        path: 'categories',
        component: CategoriesComponent,
        canActivate: [roleGuard],
        data: { roles: ['Admin', 'Manager'] }
      },
      {
        path: 'users',
        component: UsersComponent,
        canActivate: [roleGuard],
        data: { roles: ['Admin'] }
      },
      {
        path: '',
        redirectTo: 'dashboard',
        pathMatch: 'full'
      }
    ]
  },
  {
    path: '**',
    redirectTo: 'dashboard'
  }
];