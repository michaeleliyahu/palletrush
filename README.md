# PalletRush - Take-home assignment

Welcome! PalletRush runs forklift rodeo leagues. This repo is a small, working slice of our (imaginary) platform: two .NET services, Postgres, Redis and an Angular app. It has bugs, and it's missing a feature. That's your job.

- **Time:** If you go past the time you were given, stop and write what's left in `NOTES.md` - clear reasoning about unfinished work counts.
- **AI tools:** use any you like. You are the author.

## Start here

1. Read [`DOMAIN.md`](DOMAIN.md) and [`ARCHITECTURE.md`](ARCHITECTURE.md). Everything you need to know is written there.
2. Run it - see *Running locally* in ARCHITECTURE.md.

## Part 1 - Two bugs from race control

**1.1 Overbooked heat.** *"Heat 7 showed 9 registered drivers - capacity is 8. Two teams registered at the same second."*
`RegistrationConcurrencyTests` reproduces it.

**1.2 Driver search.** Two reports about the Drivers page:
- *"When I type fast, the results sometimes don't match what's in the search box."* The list must always show results for the latest text typed.
- *"After I leave the Drivers page, the browser keeps calling `/api/drivers/on-course`."* Leaving a page must stop its background requests.

## Part 2 - Feature: cone penalties on the live leaderboard

League receives `ConeHit` events but ignores them (`ConeHitHandler` is a stub). Implement cone penalties following DOMAIN.md rules 3-5.

- **Handle `ConeHit`** in League API and store penalties.
- **API** - use these exact contracts, our tooling depends on them:
  - Extend `GET /api/heats/{heatId}/leaderboard` → array sorted by rank, finished runs only:
    `{ runId, driverId, driverName, teamName, rawTimeMs, penaltyCount, penaltySeconds, totalTimeMs, rank }`
  - Add `GET /api/runs/{runId}/penalties` → `{ coneId, seconds, sensorTs }[]`
- **UI:** the Leaderboard page shows penalties and total time, ranked by rule 5. Clicking a row shows that run's penalties. The page already refreshes itself.
- **A decision for you:** a `ConeHit` can arrive before the `RunStarted` of its run. Decide what League does in that case, implement it, and explain why in `NOTES.md`. There's more than one good answer.

## Part 3 - NOTES.md

Fill in the template in [`NOTES.md`](NOTES.md).

## Tests

The provided tests are a starting point, not a definition of done. Add the tests you think matter. When reviewing, we also run our own checks against the contracts above.

## What we care about

- Correct behavior under concurrency, retries and restarts.
- Small, focused changes that follow the existing conventions.
- Tests that would catch a real regression.
- **Not:** UI polish, new frameworks, or rewriting code that works.

## Submitting

The zip you received holds this repo with its git history: one commit on `main`.

1. Push it to a new **public** repository on your personal GitHub account.
2. Work on a branch, commit as you go, and open a pull request to `main` there.
3. Reply to the invitation email with the link to your pull request.
