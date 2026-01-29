import { Routes } from '@angular/router';

export const routes: Routes = [
  {path: "", redirectTo: "cities", pathMatch: "full"},
  {path: "cities", loadComponent: () => import('./cities/cities').then(m => m.CitiesComponent)}
];
