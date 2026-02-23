import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormGroup, FormControl, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AccountService } from '../services/account';
import { LoginUser } from '../models/login-user';

@Component({
  selector: 'app-login',
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './login.html',
  styleUrl: './login.css',
})
export class LoginComponent {
  loginForm: FormGroup;
  isSubmitted: boolean = false;
  errorMessage: string = '';
  successMessage: string = '';

  constructor(
    private accountService: AccountService,
    private router: Router
  ) {
    this.loginForm = new FormGroup({
      email: new FormControl('', [Validators.required, Validators.email]),
      password: new FormControl('', [Validators.required])
    });
  }

  get email() {
    return this.loginForm.get('email');
  }

  get password() {
    return this.loginForm.get('password');
  }

  onSubmit(): void {
    this.isSubmitted = true;
    this.errorMessage = '';
    this.successMessage = '';

    if (this.loginForm.invalid) {
      return;
    }

    const loginUser: LoginUser = {
      email: this.loginForm.value.email,
      password: this.loginForm.value.password
    };

    this.accountService.login(loginUser).subscribe({
      next: (response: any) => {
        this.successMessage = 'Login successful! Redirecting...';
        this.accountService.currentUserName = response.email || null;

        // Store the token in localStorage
        localStorage.setItem('authToken', response.token);
        // Store refresh token in localStorage
        localStorage.setItem('refreshToken', response.refreshToken);
        // Redirect to cities page after .1 second
        setTimeout(() => {
          this.router.navigate(['/cities']);
        }, 100);
      },
      error: (error: any) => {
        console.error('❌ Login error:', error);
        
        if (error.error && typeof error.error === 'string') {
          this.errorMessage = error.error;
        } else if (error.error && error.error.title) {
          this.errorMessage = error.error.title;
        } else {
          this.errorMessage = 'Invalid email or password. Please try again.';
        }
      }
    });
  }

  onReset(): void {
    this.loginForm.reset();
    this.isSubmitted = false;
    this.errorMessage = '';
    this.successMessage = '';
  }
}
