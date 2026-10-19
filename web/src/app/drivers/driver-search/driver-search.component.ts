import { Component, effect, inject, signal } from '@angular/core';
import { DriversService, DriverSummary } from '../../api/drivers.service';

@Component({
  selector: 'app-driver-search',
  templateUrl: './driver-search.component.html',
  styleUrl: './driver-search.component.scss',
})
export class DriverSearchComponent {
  private readonly driversService = inject(DriversService);

  readonly query = signal('');
  readonly results = signal<DriverSummary[]>([]);

  constructor() {
    effect(() => {
      const search = this.query();
      this.driversService.search(search).subscribe((r) => this.results.set(r));
    });
  }
}
