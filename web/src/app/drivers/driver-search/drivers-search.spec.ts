import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { DriverSearchComponent } from './driver-search.component';
import { DriverSummary } from '../../api/drivers.service';

const ana: DriverSummary[] = [
  { id: 51, name: 'Diana Becker', teamName: 'Summit Distribution', licenseNo: 'FL-10051' },
  { id: 52, name: 'Diana Costa', teamName: 'Riverbend Storage', licenseNo: 'FL-10052' },
  { id: 101, name: 'Ivana Becker', teamName: 'Ironclad Pallets', licenseNo: 'FL-10101' },
];

describe('DriverSearchComponent', () => {
  beforeEach(() => {
    vi.useFakeTimers();
    TestBed.configureTestingModule({
      imports: [DriverSearchComponent],
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });
  });

  afterEach(() => {
    vi.useRealTimers();
  });

  it('typing "ana" renders results for "ana"', async () => {
    const fixture = TestBed.createComponent(DriverSearchComponent);
    const http = TestBed.inject(HttpTestingController);
    const element: HTMLElement = fixture.nativeElement;
    fixture.detectChanges();

    const input = element.querySelector<HTMLInputElement>('[data-testid="driver-search-input"]')!;
    input.value = 'ana';
    input.dispatchEvent(new Event('input'));
    fixture.detectChanges();
    await vi.advanceTimersByTimeAsync(1000);
    fixture.detectChanges();

    http.expectOne((req) => req.urlWithParams.includes('search=ana')).flush(ana);
    await vi.advanceTimersByTimeAsync(0);
    fixture.detectChanges();

    const results = Array.from(
      element.querySelectorAll('[data-testid="driver-search-result"]'),
      (item) => item.textContent,
    );
    expect(results).toEqual(ana.map((driver) => expect.stringContaining(driver.name)));
  });
});
