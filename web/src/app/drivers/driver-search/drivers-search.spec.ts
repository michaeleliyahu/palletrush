import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { DriverSearchComponent } from './driver-search.component';
import { DriverSummary } from '../../api/drivers.service';

const ana: DriverSummary[] = [
  { id: 51, name: 'Diana Becker', teamName: 'Summit Distribution', licenseNo: 'FL-10051' },
  { id: 52, name: 'Diana Costa', teamName: 'Riverbend Storage', licenseNo: 'FL-10052' },
  { id: 101, name: 'Ivana Becker', teamName: 'Ironclad Pallets', licenseNo: 'FL-10101' },
];

const d: DriverSummary[] = [
  { id: 7, name: 'Daniel Ortiz', teamName: 'Summit Distribution', licenseNo: 'FL-10007' },
];

async function type(fixture: ComponentFixture<DriverSearchComponent>, text: string) {
  const element: HTMLElement = fixture.nativeElement;
  const input = element.querySelector<HTMLInputElement>('[data-testid="driver-search-input"]')!;
  input.value = text;
  input.dispatchEvent(new Event('input'));
  fixture.detectChanges();
  await vi.advanceTimersByTimeAsync(1000);
  fixture.detectChanges();
}

function renderedResults(fixture: ComponentFixture<DriverSearchComponent>) {
  const element: HTMLElement = fixture.nativeElement;
  return Array.from(
    element.querySelectorAll('[data-testid="driver-search-result"]'),
    (item) => item.textContent,
  );
}

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
    fixture.detectChanges();

    await type(fixture, 'ana');

    http.expectOne((req) => req.urlWithParams.includes('search=ana')).flush(ana);
    await vi.advanceTimersByTimeAsync(0);
    fixture.detectChanges();

    expect(renderedResults(fixture)).toEqual(
      ana.map((driver) => expect.stringContaining(driver.name)),
    );
  });

  it('shows results for the latest query when an earlier response arrives last', async () => {
    const fixture = TestBed.createComponent(DriverSearchComponent);
    const http = TestBed.inject(HttpTestingController);
    fixture.detectChanges();

    await type(fixture, 'd');
    const staleReq = http.expectOne((req) => req.params.get('search') === 'd');
    await type(fixture, 'ana');
    const latestReq = http.expectOne((req) => req.params.get('search') === 'ana');

    latestReq.flush(ana);
    await vi.advanceTimersByTimeAsync(0);
    // The slow response for the old query arrives last; a correct component has cancelled it.
    if (!staleReq.cancelled) {
      staleReq.flush(d);
    }
    await vi.advanceTimersByTimeAsync(0);
    fixture.detectChanges();

    expect(renderedResults(fixture)).toEqual(
      ana.map((driver) => expect.stringContaining(driver.name)),
    );
  });
});
