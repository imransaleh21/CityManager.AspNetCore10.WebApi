import { Injectable } from '@angular/core';
import { City } from '../models/city';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from "rxjs";

const API_BASE_URL = 'https://localhost:1112/api/';

@Injectable({
  providedIn: 'root'
})
export class CitiesService {
  constructor(private httpClient: HttpClient) {
  }

  getCities(): Observable<City[]> {
    let headers = new HttpHeaders();
    headers = headers.append('Authorization', `Bearer ${localStorage['authToken']}`);
    return this.httpClient.get<City[]>(`${API_BASE_URL}v1/cities`, { headers: headers });
  }

  postCity(city: City): Observable<City> {
    let headers = new HttpHeaders();
    headers = headers.append('Authorization', `Bearer ${localStorage['authToken']}`);
    return this.httpClient.post<City>(`${API_BASE_URL}v1/cities`, city, { headers: headers });
  }

  putCity(cityId: string, city: City): Observable<any> {
    let headers = new HttpHeaders();
    headers = headers.append('Authorization', `Bearer ${localStorage['authToken']}`);
    return this.httpClient.put(`${API_BASE_URL}v1/cities/${cityId}`, city, { headers: headers });
  }

  deleteCity(cityId: string): Observable<any> {
    let headers = new HttpHeaders();
    headers = headers.append('Authorization', `Bearer ${localStorage['authToken']}`);
    return this.httpClient.delete(`${API_BASE_URL}v1/cities/${cityId}`, { headers: headers });
  }
}

