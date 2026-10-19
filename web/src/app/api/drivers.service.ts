import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';

export interface DriverSummary {
  id: number;
  name: string;
  teamName: string;
  licenseNo: string;
}

export interface OnCourseDriver {
  runId: string;
  heatId: number;
  driverId: number;
  driverName: string;
  teamName: string;
}

@Injectable({ providedIn: 'root' })
export class DriversService {
  private readonly http = inject(HttpClient);

  search(search: string) {
    return this.http.get<DriverSummary[]>('/api/drivers', { params: { search } });
  }

  getOnCourse() {
    return this.http.get<OnCourseDriver[]>('/api/drivers/on-course');
  }
}
