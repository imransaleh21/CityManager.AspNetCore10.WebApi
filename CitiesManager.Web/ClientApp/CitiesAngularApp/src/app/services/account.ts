import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { RegisterUser } from '../models/register-user';

const API_BASE_URL = 'https://localhost:1112/api/v1/account/';

@Injectable({
  providedIn: 'root',
})
export class AccountService {
  constructor(private httpClient: HttpClient) { }

  register(registerUser: RegisterUser): Observable<any> {
    const headers = new HttpHeaders().append('Content-Type', 'application/json');
    return this.httpClient.post(`${API_BASE_URL}register`, registerUser, { headers });
  }
}
