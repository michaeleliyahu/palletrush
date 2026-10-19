import { Pipe, PipeTransform } from '@angular/core';

/** Formats a duration in milliseconds as `m:ss.SSS`, e.g. 65230 → `1:05.230`. */
@Pipe({ name: 'runTime' })
export class RunTimePipe implements PipeTransform {
  transform(ms: number): string {
    const total = Math.round(ms);
    const minutes = Math.floor(total / 60_000);
    const seconds = Math.floor((total % 60_000) / 1000);
    const millis = total % 1000;
    return `${minutes}:${String(seconds).padStart(2, '0')}.${String(millis).padStart(3, '0')}`;
  }
}
