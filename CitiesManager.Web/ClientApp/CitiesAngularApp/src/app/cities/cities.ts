import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CitiesService } from '../services/cities.service';
import { City } from '../models/city';

@Component({
  selector: 'app-cities',
  imports: [CommonModule],
  templateUrl: './cities.html',
  styleUrl: './cities.css',
})
export class CitiesComponent implements OnInit {
  cities: City[] = [];

  constructor(private citiesService: CitiesService) { }

  ngOnInit(): void {
    console.log('CitiesComponent ngOnInit called');
    console.log('About to call getCities service...');
    
    this.citiesService.getCities()
      .subscribe({
        next: (response: City[]) => {
          console.log('Received response from API:', response);
          this.cities = response;
        },
        error: (error: any) => {
          console.error('Error calling API:', error);
        },
        complete: () => {
          console.log('API call completed');
        }
      });
  }
}
