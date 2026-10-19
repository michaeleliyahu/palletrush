import { HttpErrorResponse } from '@angular/common/http';
import { Component, computed, inject, signal } from '@angular/core';
import { toObservable, toSignal } from '@angular/core/rxjs-interop';
import { RouterLink } from '@angular/router';
import { catchError, EMPTY, map, switchMap } from 'rxjs';
import { HeatsService, RegistrationEntry } from '../api/heats.service';
import { SimulatorService } from '../api/simulator.service';
import { TeamsService } from '../api/teams.service';
import { HeatTableComponent } from './heat-table/heat-table.component';
import { RegisterFormComponent } from './register-form/register-form.component';
import { RegistrationTableComponent } from './registration-table/registration-table.component';

@Component({
  selector: 'app-race-control',
  imports: [RouterLink, HeatTableComponent, RegistrationTableComponent, RegisterFormComponent],
  templateUrl: './race-control.component.html',
  styleUrl: './race-control.component.scss',
})
export class RaceControlComponent {
  private readonly heatsService = inject(HeatsService);
  private readonly teamsService = inject(TeamsService);
  private readonly simulatorService = inject(SimulatorService);

  private readonly heatId = signal<number | undefined>(undefined);
  readonly message = signal('');
  private readonly version = signal(0);

  readonly heats = toSignal(
    toObservable(this.version).pipe(
      switchMap(() => this.heatsService.getAll().pipe(catchError(() => EMPTY))),
    ),
    { initialValue: [] },
  );

  readonly selectedHeat = toSignal(
    toObservable(computed(() => ({ heatId: this.heatId(), version: this.version() }))).pipe(
      switchMap(({ heatId }) =>
        heatId === undefined
          ? EMPTY
          : this.heatsService.getRegistrations(heatId).pipe(
              map((registrations) => ({ heatId, registrations })),
              catchError(() => EMPTY),
            ),
      ),
    ),
  );

  readonly heatName = computed(
    () => this.heats().find((heat) => heat.id === this.selectedHeat()?.heatId)?.name,
  );

  readonly teams = toSignal(this.teamsService.getAll().pipe(catchError(() => EMPTY)), {
    initialValue: [],
  });

  pick(heatId: number) {
    this.heatId.set(heatId);
    this.message.set('');
  }

  register(heatId: number, driverId: number) {
    this.heatsService.register(heatId, driverId).subscribe({
      next: () => {
        this.message.set('Registered.');
        this.version.update((version) => version + 1);
      },
      error: (error: HttpErrorResponse) => this.showError(error),
    });
  }

  simulate(heatId: number, registration: RegistrationEntry) {
    this.simulatorService.simulateRun(heatId, registration.driverId).subscribe({
      next: () => this.message.set(`Simulated a run for ${registration.driverName}.`),
      error: (error: HttpErrorResponse) => this.showError(error),
    });
  }

  private showError(error: HttpErrorResponse) {
    this.message.set(error.error?.title ?? error.message);
  }
}
