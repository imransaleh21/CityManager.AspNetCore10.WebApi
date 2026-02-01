import { Injectable } from '@angular/core';
import { City } from '../models/city';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from "rxjs";


@Injectable({
  providedIn: 'root'
})
export class CitiesService {
  constructor(private httpClient: HttpClient) {
  }

  getCities(): Observable<City[]> {
    let headers = new HttpHeaders();
    headers = headers.append('Authorization', 'BearerMytoken');
    return this.httpClient.get<City[]>("https://localhost:1112/api/v1/cities", { headers: headers });
  }

  getCityById(cityId: string): Observable<City> {
    return this.httpClient.get<City>(`https://localhost:1112/api/v1/cities/${cityId}`);
  }
}

