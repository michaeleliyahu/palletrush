import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { OnCourseComponent } from './on-course.component';
import { OnCourseDriver } from '../../api/drivers.service';

const onCourse: OnCourseDriver[] = [
  { runId: 'run-1', heatId: 1, driverId: 2, driverName: 'Diana Becker', teamName: 'Summit Distribution' },
];

describe('OnCourseComponent', () => {
  beforeEach(() => {
    vi.useFakeTimers();
    TestBed.configureTestingModule({
      imports: [OnCourseComponent],
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });
  });

  afterEach(() => {
    vi.useRealTimers();
  });

  it('polls /api/drivers/on-course while shown', async () => {
    const fixture = TestBed.createComponent(OnCourseComponent);
    const http = TestBed.inject(HttpTestingController);
    fixture.detectChanges();

    await vi.advanceTimersByTimeAsync(2000);
    http.match('/api/drivers/on-course').forEach((req) => req.flush(onCourse));
    await vi.advanceTimersByTimeAsync(2000);

    expect(http.match('/api/drivers/on-course').length).toBeGreaterThan(0);
    fixture.destroy();
  });

  it('stops polling after the component is destroyed', async () => {
    const fixture = TestBed.createComponent(OnCourseComponent);
    const http = TestBed.inject(HttpTestingController);
    fixture.detectChanges();

    await vi.advanceTimersByTimeAsync(2000);
    http.match('/api/drivers/on-course').forEach((req) => req.flush(onCourse));

    fixture.destroy();
    await vi.advanceTimersByTimeAsync(10_000);

    http.expectNone('/api/drivers/on-course');
    http.verify();
  });

  it('keeps polling after a failed request', async () => {
    const fixture = TestBed.createComponent(OnCourseComponent);
    const http = TestBed.inject(HttpTestingController);
    fixture.detectChanges();

    await vi.advanceTimersByTimeAsync(0);
    http
      .expectOne('/api/drivers/on-course')
      .flush(null, { status: 503, statusText: 'Service Unavailable' });
    await vi.advanceTimersByTimeAsync(2000);

    http.expectOne('/api/drivers/on-course').flush(onCourse);
    await vi.advanceTimersByTimeAsync(0);
    expect(fixture.componentInstance.drivers()).toEqual(onCourse);
    fixture.destroy();
  });
});
