import { Component, signal } from '@angular/core';
import { RouterOutlet, RouterLink, RouterLinkActive, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AccountService } from './services/account';

@Component({
  selector: 'app-root',
  imports: [CommonModule, RouterOutlet, RouterLink, RouterLinkActive],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  constructor(public accountService: AccountService, private router: Router) { }

  onLogOutClicked() {
    this.accountService.logout().subscribe({
      next: (response: string) => {
        this.accountService.currentUserName = null;
        this.router.navigate(['/login']);
      },
      error: (error) => { console.log(error); },
      complete: () => { },
    });
  }
}
