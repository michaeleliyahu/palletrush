import { Component, inject } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { catchError, EMPTY, exhaustMap, timer } from 'rxjs';
import { DriversService, OnCourseDriver } from '../../api/drivers.service';

@Component({
  selector: 'app-on-course',
  templateUrl: './on-course.component.html',
  styleUrl: './on-course.component.scss',
})
export class OnCourseComponent {
  private readonly driversService = inject(DriversService);

  readonly drivers = toSignal(
    timer(0, 2000).pipe(
      exhaustMap(() => this.driversService.getOnCourse().pipe(catchError(() => EMPTY))),
    ),
    { initialValue: [] as OnCourseDriver[] },
  );
}
