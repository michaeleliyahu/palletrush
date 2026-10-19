import { Routes } from '@angular/router';
import { DriversComponent } from './drivers/drivers.component';
import { LeaderboardComponent } from './leaderboard/leaderboard.component';
import { RaceControlComponent } from './race-control/race-control.component';

export const routes: Routes = [
  { path: 'race-control', component: RaceControlComponent },
  { path: 'drivers', component: DriversComponent },
  { path: 'leaderboard/:heatId', component: LeaderboardComponent },
  { path: '', pathMatch: 'full', redirectTo: 'leaderboard/1' },
];
