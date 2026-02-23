import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormGroup, FormControl, Validators, ReactiveFormsModule, AbstractControl, ValidationErrors } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AccountService } from '../services/account';
import { RegisterUser } from '../models/register-user';

@Component({
  selector: 'app-register',
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './register.html',
  styleUrl: './register.css'
})
export class RegisterComponent {
  registerForm: FormGroup;
  isSubmitted: boolean = false;
  errorMessage: string = '';
  successMessage: string = '';

  constructor(
    private accountService: AccountService,
    private router: Router
  ) {
    this.registerForm = new FormGroup({
      personName: new FormControl('', [Validators.required]),
      email: new FormControl('', [Validators.required, Validators.email]),
      password: new FormControl('', [
        Validators.required,
        Validators.minLength(3)
      ]),
      confirmPassword: new FormControl('', [Validators.required])
    }, { validators: this.passwordMatchValidator });
  }

  // Custom validator to check if password and confirmPassword match
  passwordMatchValidator(control: AbstractControl): ValidationErrors | null {
    const password = control.get('password');
    const confirmPassword = control.get('confirmPassword');

    if (!password || !confirmPassword) {
      return null;
    }

    return password.value === confirmPassword.value ? null : { passwordMismatch: true };
  }

  get personName() {
    return this.registerForm.get('personName');
  }

  get email() {
    return this.registerForm.get('email');
  }

  get password() {
    return this.registerForm.get('password');
  }

  get confirmPassword() {
    return this.registerForm.get('confirmPassword');
  }

  onSubmit(): void {
    this.isSubmitted = true;
    this.errorMessage = '';
    this.successMessage = '';

    if (this.registerForm.invalid) {
      return;
    }

    const registerUser: RegisterUser = {
      personName: this.registerForm.value.personName,
      email: this.registerForm.value.email,
      password: this.registerForm.value.password,
      confirmPassword: this.registerForm.value.confirmPassword
    };

    this.accountService.register(registerUser).subscribe({
      next: (response) => {
        this.successMessage = 'Registration successful! Redirecting to cities...';

        // Store the token in localStorage
        localStorage.setItem('authToken', response.Token);
        // Store refresh token in localStorage
        localStorage.setItem('refreshToken', response.RefreshToken);
        // Redirect to cities page after 2 seconds
        setTimeout(() => {
          this.router.navigate(['/cities']);
        }, 2000);
      },
      error: (error) => {
        console.error('❌ Registration error:', error);
        
        if (error.error && typeof error.error === 'string') {
          this.errorMessage = error.error;
        } else if (error.error && error.error.title) {
          this.errorMessage = error.error.title;
        } else {
          this.errorMessage = 'Registration failed. Please try again.';
        }
      }
    });
  }

  onReset(): void {
    this.registerForm.reset();
    this.isSubmitted = false;
    this.errorMessage = '';
    this.successMessage = '';
  }
}
