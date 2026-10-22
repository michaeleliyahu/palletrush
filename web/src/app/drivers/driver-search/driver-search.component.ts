import { Component, inject, signal } from '@angular/core';
import { toObservable, toSignal } from '@angular/core/rxjs-interop';
import { catchError, EMPTY, switchMap } from 'rxjs';
import { DriversService, DriverSummary } from '../../api/drivers.service';

@Component({
  selector: 'app-driver-search',
  templateUrl: './driver-search.component.html',
  styleUrl: './driver-search.component.scss',
})
export class DriverSearchComponent {
  private readonly driversService = inject(DriversService);

  readonly query = signal('');
  readonly results = toSignal(
    toObservable(this.query).pipe(
      switchMap((search) => this.driversService.search(search).pipe(catchError(() => EMPTY))),
    ),
    { initialValue: [] as DriverSummary[] },
  );
}
