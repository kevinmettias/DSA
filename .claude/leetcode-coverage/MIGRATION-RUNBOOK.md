# LeetCode architecture migration — agent runbook

Companion to `RUNBOOK.md`. That job **adds** coverage for problems not yet solved;
this one **converts** already-covered problems from the pre-section-17 shape into
the tiered architecture. Both read the same manifest and are safe to alternate.

Nothing here depends on conversation history — paste a block below into any fresh
Claude Code session in `F:\repos\DSA`.

## What it converts

Before: the algorithm exists twice, inlined separately in the test and in the
benchmark, so the two drift and the benchmark's `[Benchmark(Baseline = true)]`
arm is asserted by nothing.

After (ARCHITECTURE.md §17):

```
DSAExperimentation.LeetCode/<Name>/<Name>Solution.cs   every strategy, <Operation>By<Strategy>
DSAExperimentation.Tests/.../<Name>Tests.cs            harness: TheoryData Examples + 1 method per strategy
DSAExperimentation.Benchmarks/.../<Name>Benchmarks.cs  harness: 1 one-line [Benchmark] per strategy
```

Fixtures the pair carried get re-tiered by §2's axes (Representation/Topology →
`DataStructures/`, Operations → `Algorithms/`, content-fixing → `Domain/`,
single-problem witness → the problem folder), and anything new landing in
`DataStructures/` or `Algorithms/` owes its own direct unit tests (§18).

## Run one batch (any new chat)

> Run one batch of the DSA LeetCode architecture migration in `F:\repos\DSA`:
> call `Workflow({ name: "leetcode-migration", args: { batchSize: 10, clusterSize: 2 } })`
> exactly once (fall back to
> `Workflow({ scriptPath: "F:\repos\DSA\.claude\workflows\leetcode-migration.js" })`
> if it isn't registered by name yet), then report the returned counts and stop.
> The call is self-contained: it reads `.claude/leetcode-coverage/manifest.json`,
> picks the next problems where `status == "done"` and
> `architectureTier != "section-17"`, migrates each, verifies with a real build
> and the FULL test suite, and tags the manifest — so no state carries between
> calls, sessions, or agents. If it returns `{ done: true }`, every covered
> problem is already migrated.

## Run it on a loop

```
/loop Run one batch of the DSA LeetCode architecture migration: call
Workflow({ name: "leetcode-migration", args: { batchSize: 10, clusterSize: 2 } })
exactly once, report the returned counts, and stop. If it returns { done: true },
say so and stop the loop with ScheduleWakeup({ stop: true }).
```

No interval means self-paced: each tick fires only after the previous batch
finishes, which is what you want since a batch is real build-and-test work.
`/loop 45m ...` pins a fixed cadence instead. Stop it from `/tasks`, or by asking.

## Tuning

- `batchSize` — problems per run (default 10). Lower it if batches are failing
  verify; the whole batch is rolled back to un-migrated when they do.
- `clusterSize` — problems per agent (default 2). Migration is read-heavy (two
  existing files plus their fixtures), so 2 keeps an agent's context honest.
  Raise to 3 only for clusters of trivial array/string problems.

## Checking progress

```
python3 -c "import json,io,collections; m=json.load(io.open('.claude/leetcode-coverage/manifest.json',encoding='utf-8')); print(collections.Counter((p.get('status'),p.get('architectureTier')) for p in m['problems']))"
```

Remaining = every entry with `status == "done"` and `architectureTier != "section-17"`.

## Between runs

- **Spot-check, don't just trust green.** The verify phase proves it compiles and
  the suite passes; it does not prove a strategy was named well or that a
  relocated fixture landed in the right tier. Read a couple of the new
  `<Name>Solution.cs` files each run.
- **Watch what lands in `DataStructures/` and `Algorithms/`.** The migration is
  allowed to move types down a tier, and that is where a bad judgment call is
  most expensive. `LayeringTests` catches direction, not taste.
- **Blocked entries keep their old tag** and are re-selected next run. If one
  keeps failing, read its note — a genuine missing primitive is a design
  decision for you, not something to force through.
- **The Nomos gate is skipped per-item** for throughput, as in the coverage job.
  Run it periodically over `DSAExperimentation.LeetCode/` and fix findings in a
  batch.
- **Commit deliberately.** `git status` / `git diff`, then ask before committing.
  Other sessions have untracked work in this repo; never `git checkout --`,
  `git restore`, `git reset --hard`, or `git clean` anything.
