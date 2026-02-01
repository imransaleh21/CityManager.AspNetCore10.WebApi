import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CitiesService } from '../services/cities.service';
import { City } from '../models/city';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';

@Component({
  selector: 'app-cities',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './cities.html',
  styleUrl: './cities.css',
})
export class CitiesComponent implements OnInit {
  cities: City[] = [];
  postCityForm: FormGroup;
  isPostCityFormSubmitted: boolean = false;

  constructor(private citiesService: CitiesService) {
    this.postCityForm = new FormGroup({
      cityName: new FormControl(null, [Validators.required]),
    });
  }

  ngOnInit(): void {
    this.loadCities();
  }

  loadCities(): void {
    this.citiesService.getCities()
      .subscribe({
        next: (response: City[]) => {
          console.log('Received cities:', response);
          this.cities = response;
        },
        error: (error: any) => {
          console.error('Error:', error);
        }
      });
  }

  get postCity_CityNameControls(): any {
    return this.postCityForm.get('cityName');
  }

  submitPostCityForm(): void {
    this.isPostCityFormSubmitted = true;

    if (this.postCityForm.invalid) {
      return;
    }

    const newCity: City = {
      cityName: this.postCityForm.value.cityName
      // No cityId needed - server will generate it
    };

    this.citiesService.postCity(newCity)
      .subscribe({
        next: (response: City) => {
          console.log('✅ City created successfully:', response);
          // Add the new city to the list without reloading
          this.cities.push(response);
          // Reset the form
          this.postCityForm.reset();
          this.isPostCityFormSubmitted = false;
        },
        error: (error: any) => {
          console.error('❌ Error creating city:', error);
          alert('Failed to create city. Please try again.');
        }
      });
  }
}
