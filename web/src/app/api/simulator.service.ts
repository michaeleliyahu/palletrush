import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class SimulatorService {
  private readonly http = inject(HttpClient);

  simulateRun(heatId: number, driverId: number) {
    return this.http.post<{ runId: string }>('/simulate/run', { heatId, driverId });
  }
}
