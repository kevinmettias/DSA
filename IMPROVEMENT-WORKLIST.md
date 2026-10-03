# DSA improvement worklist

Prioritised from three read-only surveys of this repository (2026-10-01), then re-cut on
2026-10-03 against an external review of the testing and benchmarking layers. Every figure below
was measured on the tree, and where the review's count and the measurement differ, both are given.
Items are closed by the commits they name; what is still open says so.

---

## Testing

### The solution-tier waiver hid real gaps — FIXED

The review's claim, confirmed: `check-test-coverage` was waived over `DSAExperimentation.LeetCode/**`
as "complete by construction", but strategy coverage was enforced only for the ten registered
problems. Measured over all 1,105 solutions: 2 tests never named their solution
(`KthSmallestElementInABST`, `ConstructBinaryTreeFromInorderAndPostorderTraversal`), 18 of 2,204
strategy members had no test method named for them, and four tests asserted private copies.

- `af742e64` — KthSmallest's test and benchmark now call the solution (all three had carried their
  own copy); the Inorder/Postorder test calls `BuildByPostorderIndexMap`; InterleavingString's
  memoized arm is asserted directly (the worklist had called its private copy "additive" — it was
  the arm's only expected-value test); the catalog validator validates the real strategies.
- `df70a112` — `LeetCodeStrategyCoverageTests` holds every solution to §17.7: each
  `<Operation>By<Strategy>` member needs a test method whose first word is its name. The 18 gaps
  closed; the serialize/deserialize tests, whose value-only preorder could not tell two shapes
  apart, now compare LeetCode's level-order output.
- `b1939cc8` — the rename the review proposed: `<P>Tests` → `<P>SolutionTests` at the mirrored path
  in `DSAExperimentation.LeetCode.Tests`. The glob narrows to `DSAExperimentation.LeetCode/*/**`
  and its reason is now the measured truth: 2,197 findings, every one on a nested witness, a
  design problem's per-strategy class or an input-preparation helper (before the rename it
  absorbed about 4,590).

Open: the 29 public input-preparation helpers on solution classes (`BuildHopGraph`,
`BuildDistanceMatrix`, …) are not yet required to have a named test.

### P2 was half a rename job — DONE

Confirmed, and the waiver list was 23 files plus the glob, not 24 files. `7d3a6fe6` renamed
`MetricsTests`→`TreeMetricsTests`, `FoldTests`→`TreeFoldTests`, `GridTests`→`GridShortestPathTests`
(three waivers retired). `ef0f07df`/`40a10bd2` closed 18 more with real tests. The last two —
`HooksStep.Seed` in `BreadthFirstWalk`/`DepthFirstWalk`, dead because each void `Walk` overload
started from `default(Unit)` — went with the walk files in `f60ac220`, which runs both traversals
through `Reduce`. No `check-test-coverage` waiver on a core file remains.

### The two test projects — DONE (`b1939cc8`)

All solution-facing tests (1,159 files) moved to `DSAExperimentation.LeetCode.Tests`;
`DSAExperimentation.Tests` no longer references the solution tier, so the compiler enforces it.

### Answer equality — DONE

The review counted five implementations, the worklist two; measured, they were LeetCodeAnswers'
typed rules, three catalog adapters each re-implementing one (`af742e64` points them at
LeetCodeAnswers), and two renderers in the benchmark tests. `3d99a726` moves `AnswerGraphText`
beside LeetCodeAnswers in `Conventions/`, routes all 915 `AnswerText` calls through it, and deletes
`AnswerText` - which had rendered any non-sequence object by its type name, so comparing two tree
roots with it always passed. Nothing failed under the stricter renderer.

### List/tree builder copies — DONE

Measured: 78 files under the solution tier, its tests and the benchmarks declared a private
list/tree builder or reader (the review's ~160 counts each helper; `BuildList` alone was the
largest family). `LeetCodeWireFormat` moved out of `Harness/` into `Conventions/` and gained
`FromBinaryTree`, the missing direction (`af742e64`). `6004e3bd`, `1b4ef760` and `abd9bd40` replace
the copies in 66 files - 9 solutions, 37 tests, 20 benchmarks - and delete them. The other 10
are not the same translation and stay: seven build a BST by insertion, one lays out a heap-indexed
complete tree, one builds a cycle, one indexes nodes by value as it builds. Four solutions'
array-rebuild baselines now materialize their filtered values once more before rebuilding, a
constant-factor change to arms whose method is materializing values. The benchmarks' five
identical private tree `Clone` helpers became one `BinaryTrees.Clone` (`00424d13`).

### Redundant agreement theories — DONE (`5fdbd84c`)

31 `*_AgreeOnEveryExample` theories deleted, each verified implied by the per-strategy
expected-value tests on the same examples (two, SameTree and BalancedBinaryTree, by matching facts).

### Architecture tests — DONE (`cd71ef9e`)

The LeetCode layering row, the "migrated only" gate and the empty stragglers ledger could not fail
and are gone; base lists are read across lines; `IRecurrence` is a witness; hooks count as
witnesses in benchmarks. The stricter rules found two real offences, both fixed: FibonacciBenchmarks
(a private `IRecurrence` duplicating FibonacciNumberBenchmarks) and `Fixtures/WeightedGraphTopology`
(a copy of `NetworkTopology`).

---

## Benchmarking

### Arms return the full answer — DONE except two

The review's count, 112 classes, matches the first measurement (113: the union of a return-type
scan and a proxy-pattern scan). 105 of those now return what their strategy produced - the mutated
array, the whole list or tree (as `object?` where the type is internal), or for a design problem
every output its script observed, written into a buffer sized in `[GlobalSetup]`
(`f975454a`..`939a2020`, `62ab2943`). Six already returned their answer. A second scan, for loops
that overwrite one local and return it, found seven the first missed because their answer type was
already scalar - PrefixAndSuffixSearch, ComplexNumberMultiplication, ImplementRand10UsingRand7 and
four design replays - and they return every answer now (`22ae4d68`). Companions pin the full
answer from the workload's construction and no longer compare arms with each other. The two
`PopulatingNextRightPointers` classes keep `.Count`, documented: their strategies answer in a BCL
`Dictionary` and a repo `HashMap`, and the remedy - one answer type - belongs in the solution (open).

Returning whole answers exposed what proxies had hidden:

- `TrimByCollectAndRebuild` (LC 669) rebuilt survivors into a chain, breaking the problem's
  "keep the relative structure" rule; its tests checked only in-order values. Replaced by an
  iterative boundary walk, tests now compare LeetCode's level-order output (`3b8a6767`).
- RemoveSubFolders' generator drew duplicate paths, which LC 1233 forbids (fixed by the agent).
- KthLargestElementInAStream asked for the kth of fewer than k elements (`62ab2943`).
- DesignLinkedList and DesignFrontMiddleBackQueue replayed only void operations and returned a
  constant and a count; their scripts now end with the reads LeetCode would use (`62ab2943`).
- DesignBrowserHistory's script stepped two pages back after every visit, which always landed on
  the home page and held the history at three pages, though its comment promised append growth.
  It now steps one page back, so the history grows a page per iteration and every Back lands on a
  different page; restoring the old step fails both of its pins (`22ae4d68`).
- 8 `ArmAgreement` entries, each with evidence: 5 unordered answers LeetCode leaves free, 3
  incomparable (a free tie-break, shared subtrees, any valid BST).

### Measured code is tested code — DONE

KthSmallestElementInABSTBenchmarks re-implemented both strategies (`af742e64`); FibonacciBenchmarks
duplicated FibonacciNumberBenchmarks, whose extra arm became a tested strategy (`cd71ef9e`). A scan
for benchmark classes calling no solution found no others (its 18 other hits split the call across
a line break).

### Sizes that can show scaling — DONE where the problem leaves room

Measured: 155 classes topped out at a size of 20 or less (the review's 156 includes BurstBalloons,
which became the worked example). BenchmarkDotNet's `[Params]` are per class, so each arm now
names its own sizes through `[ArgumentsSource]`; `[GlobalSetup]` builds one workload per size, and
the generic check compares the arms at the smallest size they share (`302ba4b5`, §17.7).

71 classes now run their fast arm past the slow one: 37 + 18 + 6 + 7 + 2 across `afec5457`,
`91e50a45`, `b937d793`, `d33a9029`, `b75fee8f`, `79a5a601`, `09136562`, `d9a5c0be` and `f8f3879a`.
Each fast arm stops at LeetCode's own bound, never past it - past it an `int` answer can overflow
and the arms agree on garbage - and every converted class finished a dry run. DetectSquares, for
one, runs its grouped map to a 38×38 lattice, the largest within LC 2013's 3,000 calls, against
the list scan's 10.

85 stay as they were, each with its reason in the class comment, for one of four reasons:
LeetCode's bound is at or a step past the current top size; every arm is exponential in the same
way, or the answer itself is (Subsets, Permutations, the Catalan-many trees of
AllPossibleFullBinaryTrees); no arm is asymptotically worse (the same search under two engines, a
cap set by the answer's `int` range); or the fast arm has no measurable room inside the bound.
LeetCode's bounds keep many converted arms under the brief's 1-100 ms target at their largest size.

Open: MaximumStrictlyIncreasingCellsInAMatrix and RankTransformOfAMatrix could raise their shared
sizes (both arms are polynomial, and the baseline is not what caps them).
MaximumProfitFromValidTopologicalOrderInDag's DAG is 35% dense and admits too few orders for its
memo arm to show growth; a sparser workload would.

### Tiny examples and timed rebuilds — DONE

A scan for arms timing a published example or building their input inside the timed region
flagged 84 classes (the review's ~20 and 66 arms).

- 22 timed LeetCode's own example - a five-node tree, "aab", a four-house street - and now draw
  seeded workloads at two or three sizes up to the problem's cap (`a3d08b3c`, `f50fd3a7`,
  `6f93a722`). Six needed a generator that keeps the problem's promise - a count that fits an
  `int`, an RPN expression with no division by zero, a WordSearch word whose last letter is off the
  board so both searches exhaust it - and each generator's test pins that promise. Their companions
  re-derive every pin from the construction or a brute force, not from an arm.
- 8 keep a fixed input because the problem fixes it - a 32-bit word, a 9×9 board - and now say so.
- 7 copied or rebuilt input they did not have to: they now copy into a buffer allocated in
  `[GlobalSetup]`, or clone a prebuilt tree in O(n) instead of re-inserting it (`8031d354`,
  `eb470577`).
- 23 keep the rebuild because the strategy relinks the nodes it is given - 21 linked-list classes,
  DeleteNodeInABST, FlattenAMultilevelDoublyLinkedList - and their comments now say it is timed on
  purpose and paid by every arm (`7d44a631`, `40b656ec`). Two comments that said both arms mutate
  were wrong and are fixed.
- The rest were not rebuilds: a `readonly record struct` wrapper, or a strategy's own precompute
  that the class charges on purpose.

### Shared generators — DONE

258 inline `Enumerable.Range(0,n).Select(_ => r.Next(a,b)).ToArray()` draws - the review's count
exactly - now call `SeededDraws.Values`, making identical draws (`442eaccb`; the one-argument
`Next(max)` form was checked equal to `Next(0, max)` over 480,000 seeded draws). 131 of 152 fixtures
serve one class, which §17.7 now records as the intended shape - a problem's workload and its
promised property, tested in one place - rather than a failure to share. The one orphan,
`SuperstringWordWorkloads`, is gone.

### Registry harness — RETIRED (`527a541d`), resolving P1

"Don't generate the benchmark classes": finishing the registry meant generating 1,100 classes from
registrations that sized workloads in tier 4. It is gone; LeetCodeWireFormat and LeetCodeAnswers
survive in `Conventions/` (ARCHITECTURE §17.11).

### Categories — DONE (`67f48363`)

One hook, as the review said: `LibraryCategoryDiscoverer` files every benchmark under its folder and
every library namespace its class's arms reach (66 categories over 2,207 arms).

### Per-class boilerplate — DONE (`da0c03f5`)

`[MemoryDiagnoser]` (BenchmarkConfig adds it) and `using BenchmarkDotNet.Attributes` (now a global
using) removed from 1,111 files.

---

## Found along the way

- The weighted-adjacency node shape was written once per problem folder (`NetworkNode`, `CityNode`,
  `PointNode`, `SpecialRoadNode`, `AssignableEdgeNode`, `BranchNode`), each comment justifying
  itself against the others. Fixed in `ad8a353d`: 32 LeetCode graphs (17 weighted, 15 unweighted)
  now use the core `AdjacencyNode`/`WeightedAdjacencyNode`, kept by name through aliases.
- This session's new workloads and tree harnesses drew 50 nomos findings; all are fixed in code or
  waived in the ledger's existing form (`1cc9c535`). That pass found a gap in the uncommitted nomos
  exemption P6 depends on: it lists `ArgumentsSource`, but that attribute sits on the arm, so the
  142 `<Arm>Sizes` sources it names still read as untested public members. They are waived with an
  anchored glob for now; teaching the exemption to follow `nameof` to the provider would retire it.
- 19 waivers had outlived the code they excused - the example-tree constants and the deleted
  `PathSumExampleTree` - and three presumption waivers described an expression their file no
  longer has; CountPairsWithXorInARange's claimed every bit-trie node has both children, which a
  bit trie does not promise. Removed and rewritten (`34adab38`).
- UniquePaths timed an 18 × 18 grid whose count, 2,333,606,220, is past `int.MaxValue` and LC 62's
  promise, so both arms wrapped to the same wrong number and agreed on it; it stops at 17 now and
  pins both counts (`13761e7c`). 21 more classes, noticed while reading about 300 of them, run past
  a bound LeetCode states without overflowing - AmbiguousCoordinates (16 digits against 10),
  MatchsticksToSquare (32 sticks against 15), ZumaGame (36 against 16), SortColors (5,000 against
  300), IncreasingOrderSearchTree (20,000 nodes against 100) among them. Harmless for timing, but
  §17.7 does not say whether a harness may exceed the problem's bounds, and nobody has scanned the
  other 800. Open: decide, then either record the rule or trim.
- DesignATextEditor's script walks the cursor back over every chunk it adds, so every report is
  the empty window and its full answer tells the arms apart only by length. Open: a script that
  leaves text left of the cursor.

---

## Earlier items

- **P0, documentation truth — DONE.** §17.10 says the migration is complete (`527a541d`), §18.5
  describes the architecture tests that exist, §17.6 lists the Domain tier's three folders and
  `standards.json` names the catalog's network client (`a0af7a6f`), §13 is in order (`f032a29e`).
  The two pre-migration "777" figures are gone too: the responsibility-extraction waiver is now
  described by its real path and its measured reach, 196 of 1,105 solution files.
- **P1, registry harness — RETIRED** (`527a541d`), above.
- **P2, the named core waivers — DONE**, above.
- **P3, measurable benchmarking — DONE.** Its one open item, `[BenchmarkCategory]`, is the
  category discoverer above.
- **P4, scope gaps — DONE.** Its follow-on (registrations for six newly armed problems) is moot:
  the registry is gone and `LeetCodeStrategyCoverageTests` covers every solution.
- **P5, hygiene — DONE except one.** Neither `BenchmarkDotNet.Artifacts/` is tracked (both are
  ignored); `LeetCodeCatalog` lives in `DSAExperimentation.LeetCode.Tests` (§17.6a). Open, minor:
  the `InternalsVisibleTo` lists are still hand-maintained per project (5, 3 and 1 entries).
- **P6, the companion tree — two steps open.** The nomos exemption for BenchmarkDotNet entry points
  (`[Benchmark]`, `[GlobalSetup]`, `[Params]`, `[ArgumentsSource]`, …) is still uncommitted in
  `code-standards`, so committed nomos reports the deleted companions' classes as uncovered. And
  311 kept companion files still hold 623 methods named `…_AgreesWith…`, most of which only
  compare one arm with another - what `BenchmarkArmsTests` already asserts - beside the methods
  that pin something.

---

## What is healthy

Not everything here is debt, and the worklist should not obscure that:

- The tier order is enforced by tests that read the repo as text — `Architecture/LayeringTests.cs`,
  `Tier5WitnessTests.cs`, `RepositoryFiles.cs` — not by prose.
- Corpus coverage is complete: every one of the 1,105 solutions has a test class, and
  `LeetCodeStrategyCoverageTests` requires a named test method for every strategy it exposes.
- Every `[Benchmark]` arm is run and compared with its baseline by `BenchmarkArmsTests`, which
  asserts by reflection that the arms agree on the full answer and that the workload and answers
  rebuild identically, at the smallest size the arms share.
- The benchmark test project is doing real work — it caught the fact that a harness whose
  arms disagree is timing two different problems.
