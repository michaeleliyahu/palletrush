import {
  afterNextRender,
  Directive,
  ElementRef,
  inject,
  Injector,
  input,
  OnChanges,
} from '@angular/core';

/** Focuses the host element after it renders, and again each time the bound value changes. */
@Directive({ selector: '[appAutofocus]' })
export class AutofocusDirective implements OnChanges {
  private readonly element = inject<ElementRef<HTMLElement>>(ElementRef);
  private readonly injector = inject(Injector);

  readonly appAutofocus = input<unknown>();

  ngOnChanges() {
    afterNextRender(() => this.element.nativeElement.focus(), { injector: this.injector });
  }
}
