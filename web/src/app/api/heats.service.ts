import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';

export interface HeatSummary {
  id: number;
  name: string;
  startsAt: string;
  capacity: number;
  registeredCount: number;
}

export interface RegistrationEntry {
  id: number;
  driverId: number;
  driverName: string;
  teamName: string;
  createdAt: string;
}

export interface LeaderboardEntry {
  runId: string;
  driverId: number;
  driverName: string;
  teamName: string;
  rawTimeMs: number;
  rank: number;
}

@Injectable({ providedIn: 'root' })
export class HeatsService {
  private readonly http = inject(HttpClient);

  getAll() {
    return this.http.get<HeatSummary[]>('/api/heats');
  }

  getRegistrations(heatId: number) {
    return this.http.get<RegistrationEntry[]>(`/api/heats/${heatId}/registrations`);
  }

  register(heatId: number, driverId: number) {
    return this.http.post<void>(`/api/heats/${heatId}/registrations`, { driverId });
  }

  getLeaderboard(heatId: number) {
    return this.http.get<LeaderboardEntry[]>(`/api/heats/${heatId}/leaderboard`);
  }
}
