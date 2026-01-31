import { Injectable } from '@angular/core';
import { City } from '../models/city';
import { HttpClient } from '@angular/common/http';
import { Observable } from "rxjs";


@Injectable({
  providedIn: 'root'
})
export class CitiesService {
  constructor(private httpClient: HttpClient) {
  }

  getCities(): Observable<City[]> {
    console.log('Making HTTP GET request to: https://localhost:1112/api/v1/cities');
    return this.httpClient.get<City[]>("https://localhost:1112/api/v1/cities");
  }

  getCityById(cityId: string): Observable<City> {
    return this.httpClient.get<City>(`https://localhost:1112/api/v1/cities/${cityId}`);
  }
}
