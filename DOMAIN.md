# PalletRush - Domain rules

PalletRush runs **forklift rodeo** leagues: drivers race a forklift carrying a pallet through an obstacle course. Fastest clean run wins.
You don't need any forklift knowledge - everything you need is on this page.

## Concepts

| Term | Meaning |
|---|---|
| **Team** | A warehouse company that enters drivers. |
| **Driver** | A forklift operator. Belongs to one team. |
| **Heat** | A scheduled session of the competition. Has a fixed **capacity** (usually 8 drivers). |
| **Run** | One driver's attempt at the course in a heat. Each driver gets one run per heat. |
| **Cone** | A course obstacle, IDs `C1`-`C12`. Knocking one over costs time. |
| **Course gateway** | The box at the course that collects sensor signals and sends them to Telemetry. |

## Rules

1. **Registration.** Drivers register for a heat until it reaches capacity. A heat must never exceed its capacity, and a registration may only be rejected when the heat is actually full.
2. **Raw time** = `RunFinished.sensorTs − RunStarted.sensorTs`. Sensor clocks are GPS-synced; server clocks are never used for timing.
3. **Cone penalty.** Knocking a cone adds **5 seconds**. A knocked cone stays down until the run ends, so **each cone can be penalized at most once per run**.
4. **Total time** = raw time + penalties.
5. **Ranking** (per heat): lowest total time first. Tie → fewer penalties ranks higher. Still tied → earlier `RunFinished.sensorTs` ranks higher.

## Sensor data - what to expect

- Events reach League **at least once**: the same signal can arrive more than once.
- Gateways retransmit when their connection drops. **Firmware v2** resends the same `eventId`. **Firmware v1** (still on about 30% of courses) generates a **new `eventId`** for each retransmission. Every event carries a `firmware` field.
- Events for the same run are **not guaranteed to be processed in order**.
