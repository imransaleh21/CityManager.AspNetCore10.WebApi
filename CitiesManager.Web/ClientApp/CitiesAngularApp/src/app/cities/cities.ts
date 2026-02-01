import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CitiesService } from '../services/cities.service';
import { City } from '../models/city';
import { FormsModule, ReactiveFormsModule, FormGroup, FormControl, Validators } from '@angular/forms';

@Component({
  selector: 'app-cities',
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './cities.html',
  styleUrl: './cities.css',
})
export class CitiesComponent implements OnInit {
  cities: City[] = [];
  editingCityId: string | null = null;
  editedCityName: string = '';
  
  // Add City Form
  postCityForm: FormGroup;
  isPostCityFormSubmitted: boolean = false;

  constructor(
    private citiesService: CitiesService,
    private cdr: ChangeDetectorRef
  ) {
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
          this.cities = [...response];
          this.cdr.detectChanges();
        },
        error: (error: any) => {
          console.error('Error:', error);
        }
      });
  }

  // Add City Form Methods
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
    };

    this.citiesService.postCity(newCity)
      .subscribe({
        next: (response: City) => {
          console.log('✅ City created successfully:', response);
          this.cities = [...this.cities, response];
          this.postCityForm.reset();
          this.isPostCityFormSubmitted = false;
          this.cdr.detectChanges();
        },
        error: (error: any) => {
          console.error('❌ Error creating city:', error);
          alert('Failed to create city. Please try again.');
        }
      });
  }

  // Edit/Delete Methods
  startEdit(city: City): void {
    console.log('Starting edit for city:', city);
    this.editingCityId = city.cityId!;
    this.editedCityName = city.cityName || '';
  }

  cancelEdit(): void {
    console.log('Canceling edit');
    this.editingCityId = null;
    this.editedCityName = '';
    this.cdr.detectChanges();
  }

  saveEdit(city: City): void {
    console.log('Saving edit for city:', city, 'New name:', this.editedCityName);
    
    if (!this.editedCityName || this.editedCityName.trim() === '') {
      alert('City name cannot be empty');
      return;
    }

    const updatedCity: City = {
      cityId: city.cityId,
      cityName: this.editedCityName.trim()
    };

    this.citiesService.putCity(city.cityId!, updatedCity)
      .subscribe({
        next: () => {
          console.log('✅ City updated successfully');
          const index = this.cities.findIndex(c => c.cityId === city.cityId);
          if (index !== -1) {
            this.cities[index] = { ...this.cities[index], cityName: updatedCity.cityName };
            this.cities = [...this.cities];
          }
          this.editingCityId = null;
          this.editedCityName = '';
          this.cdr.detectChanges();
        },
        error: (error: any) => {
          console.error('❌ Error updating city:', error);
          alert('Failed to update city. Please try again.');
        }
      });
  }

  deleteCity(city: City): void {
    if (!confirm(`Are you sure you want to delete "${city.cityName}"?`)) {
      return;
    }

    console.log('Deleting city:', city);

    this.citiesService.deleteCity(city.cityId!)
      .subscribe({
        next: () => {
          console.log('✅ City deleted successfully');
          this.cities = this.cities.filter(c => c.cityId !== city.cityId);
          this.cdr.detectChanges();
        },
        error: (error: any) => {
          console.error('❌ Error deleting city:', error);
          alert('Failed to delete city. Please try again.');
        }
      });
  }

  isEditing(city: City): boolean {
    return this.editingCityId === city.cityId;
  }
}
