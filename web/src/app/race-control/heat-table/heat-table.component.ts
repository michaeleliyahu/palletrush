import { DatePipe } from '@angular/common';
import { Component, input, output } from '@angular/core';
import { HeatSummary } from '../../api/heats.service';

@Component({
  selector: 'app-heat-table',
  imports: [DatePipe],
  templateUrl: './heat-table.component.html',
  styleUrl: './heat-table.component.scss',
})
export class HeatTableComponent {
  readonly heats = input.required<HeatSummary[]>();
  readonly picked = output<number>();
}
