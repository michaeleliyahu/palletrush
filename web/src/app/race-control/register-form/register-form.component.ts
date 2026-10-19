import { Component, input, output } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { TeamSummary } from '../../api/teams.service';
import { AutofocusDirective } from '../../shared/autofocus.directive';

@Component({
  selector: 'app-register-form',
  imports: [ReactiveFormsModule, AutofocusDirective],
  templateUrl: './register-form.component.html',
  styleUrl: './register-form.component.scss',
})
export class RegisterFormComponent {
  readonly teams = input.required<TeamSummary[]>();
  readonly heatId = input.required<number>();
  readonly register = output<number>();

  readonly form = new FormGroup({
    driverId: new FormControl<number | null>(null, Validators.required),
  });

  submit() {
    const driverId = this.form.controls.driverId.value;
    if (driverId !== null) {
      this.register.emit(driverId);
    }
  }
}
