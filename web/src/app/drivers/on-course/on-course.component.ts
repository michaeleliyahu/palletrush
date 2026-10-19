import { Component, inject, signal } from '@angular/core';
import { DriversService, OnCourseDriver } from '../../api/drivers.service';

@Component({
  selector: 'app-on-course',
  templateUrl: './on-course.component.html',
  styleUrl: './on-course.component.scss',
})
export class OnCourseComponent {
  private readonly driversService = inject(DriversService);

  readonly drivers = signal<OnCourseDriver[]>([]);

  constructor() {
    setInterval(
      () => this.driversService.getOnCourse().subscribe((r) => this.drivers.set(r)),
      2000,
    );
  }
}
