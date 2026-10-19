# PalletRush - Architecture

```
 Course gateway (simulated)
          │ HTTP
          ▼
 ┌──────────────────┐   Redis stream      ┌──────────────────┐
 │  Telemetry API   │── "course-events" ─▶│    League API    │
 │  :5200           │                     │    :5100         │
 │  db: telemetry   │                     │    db: league    │
 └──────────────────┘                     └──────────────────┘
          ▲                                        ▲
          │ "Simulate run"                         │ REST
          └────────────── Angular web :4200 ───────┘
```

| Service | Owns | Tech |
|---|---|---|
| League API (`src/League.Api`) | teams, drivers, heats, registrations, runs, results | .NET 10, ASP.NET Core controllers, EF Core 10 + Npgsql |
| Telemetry API (`src/Telemetry.Api`) | raw sensor readings, course simulator | .NET 10, EF Core 10 + Npgsql |
| Web (`web/`) | race control, driver search, leaderboard | Angular 21 (standalone, signals, zoneless), Vitest |
| Shared contracts (`src/Shared.Contracts`) | event records | - |

Each service owns its database. Services never read each other's database.

## Events

- Telemetry publishes to the Redis stream `course-events`. Each entry has `type` and `payload` (JSON). Types: `RunStarted`, `ConeHit`, `RunFinished` - see `src/Shared.Contracts/CourseEvents.cs`.
- League consumes the stream with consumer group `league`. The loop is in `src/League.Api/Events/CourseEventsConsumer.cs`; each event type has a handler implementing `ICourseEventHandler<T>`.

## Production

- **League API runs as 3 replicas** behind a load balancer. All replicas consume `course-events` in the same consumer group, so any replica may process any event.
- Telemetry API runs as 2 replicas.
- Postgres and Redis are managed services.

## Running locally

```
docker compose up -d                          # postgres, redis, telemetry-api
dotnet run --project src/League.Api           # http://localhost:5100
cd web && npm install && npm start            # http://localhost:4200
```

- Everything in containers instead: `docker compose --profile full up -d --build`.
- Tests: `dotnet test` (needs Docker - integration tests use Testcontainers) · `cd web && npm test`.
- Simulate events: **Race control** page → pick a heat → **Simulate run**. Or `POST` any `{ type, payload }` to `http://localhost:5200/dev/events`.
- In Development, `GET /api/drivers` adds simulated latency (100-1500 ms) to mimic the production search service. Leave it in place.

## Conventions

- Controllers → services → `LeagueDbContext`. Migrations: `dotnet ef migrations add <Name> --project src/League.Api`; applied automatically on startup in Development.
- Angular: standalone components, signals for state. Tests use the `data-testid` attributes - keep them.
