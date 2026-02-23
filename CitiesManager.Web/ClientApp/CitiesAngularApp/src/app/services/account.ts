import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { RegisterUser } from '../models/register-user';
import { LoginUser } from '../models/login-user';

const API_BASE_URL = 'https://localhost:1112/api/v1/account/';

@Injectable({
  providedIn: 'root',
})
export class AccountService {

  public currentUserName: string | null = null;

  constructor(private httpClient: HttpClient) { }

  register(registerUser: RegisterUser): Observable<any> {
    const headers = new HttpHeaders().append('Content-Type', 'application/json');
    return this.httpClient.post(`${API_BASE_URL}register`, registerUser, { headers });
  }

  login(loginUser: LoginUser): Observable<any> {
    const headers = new HttpHeaders().append('Content-Type', 'application/json');
    return this.httpClient.post(`${API_BASE_URL}login`, loginUser, { headers });
  }

  logout(): Observable<any> {
    return this.httpClient.get(`${API_BASE_URL}logout`);
  }
  generateNewToken(): Observable<any> {
    var authToken = localStorage['authToken'];
    var refreshToken = localStorage['refreshToken'];
    const headers = new HttpHeaders().append('Content-Type', 'application/json');
    return this.httpClient.post(`${API_BASE_URL}generate-new-token`, { token: authToken, refreshToken: refreshToken }, { headers });
  }
}
