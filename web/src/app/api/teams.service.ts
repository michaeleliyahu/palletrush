import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';

export interface TeamDriver {
  id: number;
  name: string;
}

export interface TeamSummary {
  id: number;
  name: string;
  drivers: TeamDriver[];
}

@Injectable({ providedIn: 'root' })
export class TeamsService {
  private readonly http = inject(HttpClient);

  getAll() {
    return this.http.get<TeamSummary[]>('/api/teams');
  }
}
