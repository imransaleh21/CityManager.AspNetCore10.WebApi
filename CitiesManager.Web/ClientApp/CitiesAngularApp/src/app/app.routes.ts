import { Routes } from '@angular/router';

export const routes: Routes = [
  {path: "", loadComponent: () => import('./home/home').then(m => m.HomeComponent)},
  {path: "cities", loadComponent: () => import('./cities/cities').then(m => m.CitiesComponent)},
  {path: "register", loadComponent: () => import('./register/register').then(m => m.RegisterComponent)},
  {path: "login", loadComponent: () => import('./login/login').then(m => m.LoginComponent)}
];
