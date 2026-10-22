# Notes

Keep it short - bullet points are fine.

## Decisions

**`ConeHit` before `RunStarted` - what League does, and why:**

**Other trade-offs you made:**

- **1.1 Overbooking** - the slot is claimed with a conditional atomic `UPDATE ... SET registered_count = registered_count + 1 WHERE registered_count < capacity`, in one transaction with the insert (`RegistrationService.RegisterAsync`).
  - League runs as 3 replicas, so an in-process lock can't work; the Postgres row lock does.
  - Rejected `xmin`/optimistic concurrency and SERIALIZABLE: they throw on contention, so requests would be rejected while slots are still free (DOMAIN rule 1) unless a retry loop is added.
  - Eligibility is checked after the claim; an ineligible driver returns without commit, so the claim rolls back.
- **1.2a Search** - `toObservable(query)` + `switchMap`: a new query cancels the previous request, so a slow stale response can never overwrite the list. No debounce; it isn't needed for correctness.
- **1.2b On-course polling** - `timer` + `exhaustMap` through `toSignal`, so polling stops when the component is destroyed. `exhaustMap` rather than `switchMap` so a slow poll isn't cancelled by the next tick (and slow polls don't pile up).

## AI usage

**Tools, and what you used them for:**

**One AI suggestion you rejected or changed, and why** (point to the file/line):

## Shortcuts and next steps

**What you skipped, and what you'd do next with more time:**

- Duplicate registrations on client retry: nothing stops the same driver registering twice for a heat. Next: unique index on `(heat_id, driver_id)` and treat the violation as an idempotent success.
- Existing overbooked data (heat 7 with 9 rows) needs a one-off repair script; the fix only prevents new cases.
- If `EnableRetryOnFailure` is turned on later, the explicit transaction in `RegisterAsync` must run inside `CreateExecutionStrategy()`.
