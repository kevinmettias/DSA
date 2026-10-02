# DSA improvement worklist

Prioritised from three read-only surveys of this repository plus direct measurement,
2026-10-01. Items marked ✓ were measured first-hand; the rest are survey-reported and
should be re-checked before acting — one survey got a count wrong (it reported 4
problems with no benchmark; the measured answer is 11).

Scale for orientation: 6 projects, no `.sln`; 1,105 LeetCode problems; 1,105 coverage
tests; 1,097 benchmark classes each with a companion test; ~10,346-line core library
(✓, across 211 files).

---

## P0 — Make the documentation true

The file that defines the architecture misreports the architecture's own state.

| Where | Says | Reality |
|---|---|---|
| `ARCHITECTURE.md` §17.10 (lines 1370–1377) | 8 problems migrated; "the bulk of the catalogue has not moved yet" | 1,105 solution folders (✓) |
| `ARCHITECTURE.md` §18.5 | "56 files still declare a stranded witness" | pre-migration |
| `ARCHITECTURE.md` line 1256 | "777 times", "~800 eventual problem classes" | pre-migration |
| `ARCHITECTURE.md` §17.6 | `Domain/` is "Two folders, not five" | three: `Domain/Locks/`, `Domain/Modular/` (with an unlisted `FactorialTable.cs`), `Domain/SlidingPuzzle/` |
| `standards.json` rationale | the repo has "no I/O, no services" | `Tests/LeetCodeCatalog/` is an HTTP GraphQL client |

Also: §13's headings are out of order — 13.7/13.8/13.9 sit at lines 681/711/752, ahead
of 13.4/13.5/13.6 at 827/835/862.

**Action:** correct each figure against the current tree; decide whether `SlidingPuzzle`
is legitimately Domain (by §17.6's axis rule) or belongs elsewhere, and record the call;
reword or delete the "no I/O" rationale.

**Done when:** every scale figure in `ARCHITECTURE.md` and `standards.json` matches a
count you can reproduce, and §13's headings are ordered.

**Note:** `ARCHITECTURE.md` is currently `MM` — another session has changes staged in it.
Coordinate before editing or the two will collide.

---

## P1 — Decide the registry harness: finish or retire

Two complete harnesses coexist. The tier/registry system (`ILeetCodeProblemRegistration`,
`LeetCodeProblemRegistry`, `IBoundWorkload`, the generic `LeetCodeProblemBenchmarks`) was
built to migrate the catalogue; the catalogue migrated, but not onto it.

- **11** of 1,105 problems are registered (~30 generated arms).
- `MinStack` is registered and declares **0 workloads** — no arm at all.
- `IBoundWorkload` has exactly **one** implementation (private nested in `TypedLeetCodeProblem`).
- All **1,097** hand-written `*Benchmarks.cs` classes still call solutions directly.
- The registered problems keep their hand-written classes too, so those 11 are double-covered.
- A parallel split exists in validation: `Harness/LeetCodeAnswers` + `LeetCodeProblemBuilder`
  versus `LeetCodeCatalog/LeetCodeSolutionValidator` + `ILeetCodeTestCaseAdapter` + three
  hand-written adapters, each re-implementing order-independent comparison.

**Action:** pick one. Finishing means replacing 1,097 working classes — justify it.
Retiring means deleting the registry path and rewriting §17 to describe the hand-written
shape that actually carries the load. Either way, resolve the duplicate answer-equality
systems to one owner.

**Done when:** one harness shape is the documented architecture, the other is gone, and
no code path has two owners.

---

## P2 — Close the 24 named core waivers

`suppressions.json` waives `check-test-coverage` for 24 core files that "owe direct tests"
(§18.3). They are not peripheral — they include the capability the README leads with:

- **Fold/reduce (the advertised thesis):** `Folding/CheckedFold.cs`, `Folding/Dags/DagFold.cs`,
  `Folding/Dags/Trees/TreeFold.cs`, `Folding/IterativeFoldEvaluation.cs`,
  `Folding/RecursiveFoldEvaluation.cs`, `Reducing/BreadthFirstReduceOrder.cs`,
  `Reducing/DepthFirstReduceOrder.cs`, `Reducing/Reduce.cs`
- **Traversal/walk entry points:** `Traversal/BreadthFirst/BreadthFirstTraversal.cs`,
  `LevelGroupedBreadthFirstTraversal.cs`, `Traversal/DepthFirst/DepthFirstTraversal.cs`,
  `Walking/BreadthFirstWalk.cs`, `Walking/DepthFirstWalk.cs`
- **Strings/graphs:** `StringMatching/Manacher.cs`, `StringMatching/ZFunction.cs`,
  `Connectivity/ConnectedComponents.cs`, `ShortestPaths/Grids/GridShortestPath.cs`
- **Metrics/paths/DS internals:** `Metrics/TreeMetrics.cs`, `Paths/AllRootToLeafPaths.cs`,
  `DataStructures/Graph/Grids/Grid.cs`, `HashMap/HashMapStorage.cs`, `IntervalSet/IntervalSet.cs`,
  `SuffixTree/SuffixTreeNode.cs`

Also thin: seam tests cover **16** problems; `Benchmarks/Program.cs` and
`.claude/workflows/*.js` have no tests.

**Action:** one `*Tests.cs` per file at the mirrored path, per §18.4. Retire each waiver
as its test lands — the waiver list is the progress bar.

**Done when:** `suppressions.json` carries no `check-test-coverage` waiver for a core file
that owes a direct test.

---

## P3 — Make benchmarking measurable

Nothing statistically meaningful has ever been recorded.

- Every artifact on disk is `Job=Dry` (or `Job=ShortRun`) — smoke runs from ad-hoc
  `--job` flags. `Program.cs` is a bare `BenchmarkSwitcher`; no `IConfig`/`ManualConfig`
  exists, so the default job is never the one captured.
- No stored baseline and no JSON exporter. `Baseline = true` (1,073 arms) is BDN's in-run
  ratio, not a baseline — so a regression is undetectable.
- No CI and no runner script; benchmarks run only by hand.
- No `[BenchmarkCategory]` anywhere, so results can't be sliced.
- 27 classes have a single arm: 7 are a lone `Baseline = true` (ratio 1.00 by construction);
  20 have one arm and no baseline.
- 98 classes have no `[GlobalSetup]`; 26 have no `[Params]` — timed-region input
  construction is not structurally prevented.

**Action:** add a `ManualConfig` with a job worth recording; enable a JSON exporter and
check in a baseline; add a runner script; tag classes with `[BenchmarkCategory]`; give the
27 single-arm classes a counterpart arm.

**Done when:** a documented command reproduces a run, its baseline is in the repo, and a
deliberate slowdown in one arm is detectable by comparing against it.

---

## P4 — Close the scope gaps

- **11 problems have neither a benchmark nor a benchmark test** (✓):
  `ClimbingStairs`, `CourseSchedule`, `ImplementTrie`, `KthLargestElement`,
  `LowestCommonAncestorOfBst`, `MergeTwoSortedLists`, `MinStack`, `NetworkDelayTime`,
  `RedundantConnection`, `Subsets`, `ValidParentheses`.
  README claims "one [benchmark] per problem" — add them or document the exceptions.
- Three benchmark suites have no problem folder (✓): `Fibonacci`, `KthLargest`,
  `ShortestPathAlgorithm` — plausibly deliberate library-level suites; record why.
- `NQueensBenchmarks` / `NQueensIIBenchmarks` declare `[Params(8)]` — a single value, so
  the parameter is never varied.

---

## P5 — Repository hygiene

- `suppressions.json.bak-prune-1789527222` (457 KB) is **tracked** — an unreferenced backup
  of linter output. Delete and extend `.gitignore`.
- `.claude/leetcode-coverage/manifest.json` (2.9 MB, 27k lines) is tracked agent-workflow
  intermediate state.
- `BenchmarkDotNet.Artifacts/` exists in two places (root and under `Benchmarks/bin/...`).
- `DSAExperimentation.Tests/LeetCodeCatalog/Fixtures.tmp_check/` is an empty leftover.
- **No `.sln`** across six projects: nothing at the root can build or test them all, and
  the `InternalsVisibleTo` lists are hand-maintained per project (5/4/1/0 entries), so
  adding a project means editing several `.csproj` files.

**Action:** remove the tracked backup and the stray temp dir; add a solution file; decide
where `LeetCodeCatalog` belongs — it is I/O infrastructure living in a test project.

**Done when:** `git ls-files` contains no backup or workflow-intermediate artifact, and a
single root command builds and tests all projects.

---

## What is healthy

Not everything here is debt, and the worklist should not obscure that:

- The tier order is enforced by tests that read the repo as text — `Architecture/LayeringTests.cs`,
  `Tier5WitnessTests.cs`, `RepositoryFiles.cs` — not by prose.
- Corpus coverage is essentially complete: 1,105/1,105 problems have a per-problem test, and
  1,097/1,097 benchmark classes have a companion test asserting their arms agree and their
  workloads rebuild identically.
- Every `[Benchmark]` arm name appears in its test file, so the "naive baseline was never
  tested" gap §17.1 describes is closed for the hand-written classes.
- The benchmark test project is doing real work — it caught the fact that a harness whose
  arms disagree is timing two different problems.
