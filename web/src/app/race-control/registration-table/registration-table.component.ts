import { DatePipe } from '@angular/common';
import { Component, input, output } from '@angular/core';
import { RegistrationEntry } from '../../api/heats.service';

@Component({
  selector: 'app-registration-table',
  imports: [DatePipe],
  templateUrl: './registration-table.component.html',
  styleUrl: './registration-table.component.scss',
})
export class RegistrationTableComponent {
  readonly registrations = input.required<RegistrationEntry[]>();
  readonly simulate = output<RegistrationEntry>();
}
