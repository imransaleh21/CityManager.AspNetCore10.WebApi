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
    this.cities = this.citiesService.getCities();
  }
}
