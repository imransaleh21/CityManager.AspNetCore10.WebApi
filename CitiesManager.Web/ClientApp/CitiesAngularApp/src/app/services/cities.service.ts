import { Injectable } from '@angular/core';
import { City } from '../models/city';

@Injectable({
  providedIn: 'root'
})
export class CitiesService {
  private mockCities: City[] = [
    new City('4d2648e5-4820-4dea-b8de-e606e243c535', 'Dhaka'),
    new City('d25858f7-1566-47d8-80e5-40078b916140', 'Dinajpur'),
    new City('a1b2c3d4-e5f6-7890-abcd-ef1234567890', 'Chittagong'),
    new City('b2c3d4e5-f6a7-8901-bcde-f12345678901', 'Rajshahi'),
    new City('c3d4e5f6-a7b8-9012-cdef-123456789012', 'Khulna'),
    new City('d4e5f6a7-b8c9-0123-def1-234567890123', 'Sylhet')
  ];

  constructor() { }

  getCities(): City[] {
    return this.mockCities;
  }

  getCityById(cityId: string): City | undefined {
    return this.mockCities.find(city => city.cityId === cityId);
  }
}
