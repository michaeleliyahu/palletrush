import { Component } from '@angular/core';
import { DriverSearchComponent } from './driver-search/driver-search.component';
import { OnCourseComponent } from './on-course/on-course.component';

@Component({
  selector: 'app-drivers',
  imports: [DriverSearchComponent, OnCourseComponent],
  templateUrl: './drivers.component.html',
  styleUrl: './drivers.component.scss',
})
export class DriversComponent {}
