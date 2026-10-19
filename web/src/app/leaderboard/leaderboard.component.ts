import { Component, inject, input, numberAttribute } from '@angular/core';
import { toObservable, toSignal } from '@angular/core/rxjs-interop';
import { catchError, EMPTY, switchMap, timer } from 'rxjs';
import { HeatsService } from '../api/heats.service';
import { RunTimePipe } from '../shared/run-time.pipe';

@Component({
  selector: 'app-leaderboard',
  imports: [RunTimePipe],
  templateUrl: './leaderboard.component.html',
  styleUrl: './leaderboard.component.scss',
})
export class LeaderboardComponent {
  private readonly heatsService = inject(HeatsService);

  readonly heatId = input.required({ transform: numberAttribute });

  readonly entries = toSignal(
    toObservable(this.heatId).pipe(
      switchMap((heatId) =>
        timer(0, 3000).pipe(
          switchMap(() => this.heatsService.getLeaderboard(heatId).pipe(catchError(() => EMPTY))),
        ),
      ),
    ),
    { initialValue: [] },
  );
}
