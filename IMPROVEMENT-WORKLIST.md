# DSA improvement worklist

Prioritised from three read-only surveys of this repository plus direct measurement,
2026-10-01. Items marked ✓ were measured first-hand; the rest are survey-reported and
should be re-checked before acting — one survey got a count wrong (it reported 4
problems with no benchmark; the measured answer is 11).

Scale for orientation: 6 projects, one root `DSA.slnx`; 1,105 LeetCode problems; 1,105 coverage
tests; 1,111 benchmark classes (1,106 one-per-problem, 4 in `StrategySwaps/`, plus the
generic registry harness), every one a case of the generic `BenchmarkArmsTests` and 497 of them
also with a companion test that pins more; ~10,346-line core library
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
- All **1,106** hand-written `*Benchmarks.cs` classes still call solutions directly.
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

**Done when:** a documented command reproduces a run, its baseline is in the repo, and a
deliberate slowdown in one arm is detectable by comparing against it. — **met.** The only
thing left below is the `[BenchmarkCategory]` tagging.

**One place a run is configured — DONE** (`4e270441`). `BenchmarkConfig.For(args)` pins the
recording job as explicit counts (6 warmups, 15 iterations, 1 launch) rather than as BDN's
`Default`, which is BDN's answer to give and has changed between versions, and adds
`JsonExporter.Full`, the only machine-comparable export. The pin is dropped when the command
line carries `--job`, `--job=` or `-j`, because BDN treats a command-line job as one MORE job:
with an assembly-level `[Config]`, `--job dry` ran the dry job and the full one back to back.
`BenchmarkConfigTests` covers that decision and nothing else, because it is the one that fails
invisibly.

**A baseline in the repo, and a command that checks a run against it — DONE.**
`DSAExperimentation.Benchmarks/baseline.tsv` is one tab-separated line per arm, sorted by name,
under a header naming the filter and job that produced it. `baseline record` and `baseline
compare` read and write it (`--report` reads existing reports instead of running anything), and
exit 0 within tolerance, 1 past it, 2 when there is no verdict. Text in, text out, so all of it
is tested without a benchmark run; `BaselineCommand` owns the paths, `BenchmarkBaseline` owns
the arithmetic.

**The load-bearing proof, performed rather than asserted.** With `ReduceOrderBenchmarks.
BreadthFirst` deliberately made to run its reduce twice, `baseline compare` reported that arm
at 2.05x and 2.01x and **exited 1**, while the untouched `DepthFirst` arms stayed within
tolerance and no other arm was named. `git hash-object` was `39d80d9d` before the mutation and
`39d80d9d` after the restore.

Two things the first real runs changed, both worth keeping because both were invisible in
synthetic data:

- **Allocation needs the same tolerance as time.** It is exact per RUN, not per operation:
  BDN divides the run's total by an operation count it picks afresh, so an unmodified arm
  reported 17,537,358 bytes and then 17,537,486 — while its *time* had improved 1.2%. The
  first version called that a regression. Allocation now has to move past `--tolerance`
  relative to the baseline, with no absolute floor, so an arm that allocated nothing and now
  allocates something is still caught.
- **The job does not end at the first space.** `...Recursive: Job-ABCDEF(IterationCount=15,
  LaunchCount=1, WarmupCount=6) [Size=10000]` — cutting at the first space keeps
  `(IterationCount=15,` and drops the counts. Worse, it fails *symmetrically*, so two different
  jobs still look identical and the check passes while doing nothing. Two unit tests and a
  hand-built fixture had agreed with the broken reader; only a real report disagreed.

Two fresh control runs were made (unchanged code, same filter, exit 0 expected): the first
found the allocation bug, the second was clean.

**Still open — one item**, measured rather than trusted from the first draft:

- **No `[BenchmarkCategory]` anywhere** across 1,111 classes, so results still cannot be
  sliced. One write per file; bulk multi-file rewrites are refused in this repo, so this wants
  a scripted pass or a decision to tag by folder.

**Single-arm classes — resolved this session.** The first draft's survey was wrong about the
size, in the direction that mattered, so the numbers below are re-measured:

- **48 → 24, and the 24 are each deliberate — DONE** (`a26df9c8`). The
  first draft said 27; the measured figure was 47 per-problem classes plus the generic
  `LeetCodeProblemBenchmarks` at the root (a registry harness, not a per-problem class). Of
  the 47, **18 carried a lone `[Benchmark(Baseline = true)]`**, which reports a ratio of
  exactly 1.00 by construction — not 7. 24 of the 47 were armed, leaving **23 per-problem
  single-arm (12 of them lone-baseline)**. Count `[Benchmark` **anchored to line start**:
  unanchored it also matches the prose in these very class comments ("the two `[Benchmark]`
  arms were unwired stubs") and inflates the count.
- **What "armed" means.** A genuinely different textbook approach was added as a second arm
  wherever the benchmark returns a BCL type. The pre-existing arm kept the work and took
  `Baseline = true`; the new arm sits first in the solution class (§17.3); every class gained
  the agreeing-property test a two-arm harness needs, and the solution test gained the same
  for the new strategy. `MajorityElement` and `MajorityElementII` were hand-written first as
  the pattern's proof, then 22 more by three parallel agents editing only. Full suite green:
  **11,323 + 117 + 3,789**, 0 failures.
- **The 23 that remain are deliberate, classified rather than assumed.** 11 return a
  solution-local or repo type (`object?` for a `ListNode`/`TreeNode`, or a mutation-typed
  answer) so a rival would have to be written in the harness and time something the
  architecture does not have — `ConstructBinaryTreeFrom{PreorderAndInorder,
  InorderAndPostorder}Traversal`, `ConvertSorted{Array,List}ToBinarySearchTree`,
  `CopyListWithRandomPointer`, `InsertionSortList`, `IntersectionOfTwoLinkedLists`,
  `LowestCommonAncestorOfABinaryTree`, `ReorderList`, `ReverseLinkedList`, `SortList`. 2
  are `void` because the mutation *is* the answer (`DeleteNodeInALinkedList`,
  `InvertBinaryTree`). 4 are stateful structures or one-tricks, not algorithms
  (`BinarySearchTreeIterator`, `ImplementQueueUsingStacks`, `ImplementTrie`,
  `TrafficSignalColor`). 4 have only a strawman alternate — `ProductOfArrayExceptSelf`'s
  division arm breaks on zeros, and `BinaryTreeMaximumPathSum`/`EvaluateReversePolishNotation`
  have no honest rival; `FruitsIntoBasketsII`'s proposed arm answers a *different* question
  (see below). 2 are deliberate: `SubsetsII` and `ValidateBinarySearchTree`.
- **`FruitsIntoBasketsII` — the arm I proposed was wrong, and it is worth recording why.**
  The natural alternate is a smallest-fit greedy; the problem requires *leftmost* fit, and
  they disagree. LC example 1: fruits `[4,2,5]` into baskets `[3,5,4]` leaves **1** unplaced
  (leftmost fit puts 4 in basket 1, 2 in basket 2, 5 nowhere), while smallest-fit leaves
  **0**. Forcing it in would have made the two arms time two different problems.
- **`ValidateBinarySearchTree` is left single-arm on its role, not its merits.** It is the
  only one of the ten registered problems with exactly one strategy, so it is the sole
  witness of the one-strategy path in both registry theories. Arming it would delete that
  coverage, not add a comparison. `SubsetsII`'s former `IterativeDedup` arm is a *documented
  removal* ("dead code wrapping the one real strategy"), not a survivor to extend.
- **Known redundancy, flagged not fixed.** `InterleavingStringTests` still exercises a
  private reimplementation of the memoized arm (a local `CanBuild : IRecurrence<…>`) in
  `IsInterleave_LeetCodeExamples_ReturnsExpected`, alongside the new theories that call the
  real arms. It is additive, not wrong, but it is a duplicate of arm 1 that could now be
  deleted.
- **README's "7,171 test methods"** counts `[Fact]`/`[Theory]` attributes; the three test
  projects hold 7,058 today. Close enough to be a definitional difference, so it is flagged
  rather than changed — the figure is a P0 doc-truth item.
- **The "98 classes have no `[GlobalSetup]`" item is not a gap — measured, and withdrawn.**
  There are 99 per-problem classes without one (`[BenchmarkCategory` occurrences: 0, so that
  half of the item stands). But reading them shows why: a solution whose input is a scalar
  takes it from `[Params]` directly (`AddDigitsByArithmetic(Value)`), so there is nothing to
  build and nothing constructed inside the timed region. `[GlobalSetup]` is for the classes
  that *do* build a tree, grid or list — which is exactly the ones that have it. The real
  question is not "how many lack a setup" but "does any timed method construct a non-trivial
  input", and that is unmeasured. 26 classes have no `[Params]`; that one is real, and those
  are candidates for the same `[Params(8)]` treatment P4 gave the two N-Queens suites.

---

## P4 — Close the scope gaps — DONE

**The 11 unbenchmarked problems — DONE.** All eleven now have both a benchmark class
and a companion benchmark test. Two of the eleven (`KthLargestElement`,
`NetworkDelayTime`) were already covered by differently-named library-level suites
(`KthLargestBenchmarks`, `ShortestPathAlgorithmBenchmarks`), and both of those already
carried two arms. The other nine needed new work.

Eight of the nine got a **second arm**, because a benchmark with one arm is a ratio of
1.00 by construction. The new arm follows the repo's own precedent (`KthLargestElement`):
a textbook strategy added to the solution file, not a rival structure invented in the
harness.

| Problem | arm added | measured against |
| --- | --- | --- |
| `ClimbingStairs` | `CountWaysByIterativeRollingTotals` | `CountWaysByMemoizedRecurrence` |
| `ValidParentheses` | `IsValidByRepeatedPairRemoval` | `IsValidByBracketStack` |
| `Subsets` | `FindAllSubsetsByBitmask` | `FindAllSubsetsByBacktrackSearch` |
| `RedundantConnection` | `FindRedundantEdgeByPathSearch` | `FindRedundantEdgeByDisjointSet` |
| `CourseSchedule` | `CanFinishByDepthFirstColoring` | `CanFinishByTopologicalSort` |
| `LowestCommonAncestorOfBst` | `FindLcaByBstValueComparison` | `FindLcaByAncestryWalk` |
| `MergeTwoSortedLists` | `MergeByRecursiveSelection` | `MergeByDummyHeadSplice` |
| `MinStack` | `CreateBySingleListScan` | `CreateByStackPrimitive` |

Two decisions worth keeping:

- **`ImplementTrie` is deliberately single-arm.** The solution *returns* the repo's own
  `DataStructures.Trie.Trie<bool>` — a shape `TrieReturnedBySolutionSeamTests` exists to
  pin. A rival node-chain trie written in the harness would time a structure the
  architecture does not have. `BinarySearchTreeIteratorBenchmarks` is the same case.
- **`MinStack` grew a nested `IMinStackOperations`** so LeetCode's four-call shape reaches
  both arms identically, the same move `ApplyDiscountEveryNOrdersSolution` makes for its
  two cashiers. Both factories now return the interface; the registration and the
  per-problem test were retyped to match.

**Newly discovered while doing this:** `LeetCodeStrategyCoverageTests` requires a
registration to name *every* strategy its solution exposes ("Every strategy is measured
and asserted, or none of them is"). Only two of the eight newly-armed problems own a
registration (`MergeTwoSortedLists`, `MinStack`), and both were updated. The other six —
`ClimbingStairs`, `ValidParentheses`, `Subsets`, `RedundantConnection`, `CourseSchedule`,
`LowestCommonAncestorOfBst` — have **no registration file at all**, so nothing but their
own per-problem test file asserts their second arm. Adding those registrations is the
natural follow-on.

**The three folderless suites — DONE.** Two are genuinely library-level, and now say so in
their own class comments: `FibonacciBenchmarks` covers a recurrence shared by LC 509 and
LC 70, `ShortestPathAlgorithmBenchmarks` an algorithm choice forced by both LC 743 and
LC 787. Neither belongs under one problem folder.

The third was not deliberate at all — `KthLargestBenchmarks` was simply named shorter than
the solution folder it covers. Renamed to `KthLargestElementBenchmarks`, with its companion
test, so README's "one benchmark per problem" is now literally true. No string in the repo
referenced the old name except the generated coverage manifest.

**`[Params(8)]` — DONE.** Both N-Queens suites now vary the axis: `NQueensBenchmarks` over
(6, 8, 10) and `NQueensIIBenchmarks` over (8, 10, 12), multiplicative rather than
arithmetic because the search tree is super-exponential in size. Measured on a dry run, the
ratio the suite exists to show moves from 1.45 to 2.09 across the first arm's sizes and
2.53 to 2.79 across the second's — under a single value it was a single number that could
not have shown that.

**One more cleanup found while doing this.** `KthLargestElementBenchmarksTests` still
carried a true mirror pair (`FullSort_…_AgreesWithSizeKMinHeap` /
`SizeKMinHeap_…_AgreesWithFullSort`) — the same defect `d98947ae` removed from the other
benchmark tests. One arm dropped, matching the rest. A repo-wide sweep for the pattern
(`A_Scenario_AgreesWithB` beside `B_Scenario_AgreesWithA`) now finds none; the remaining
`_AgreesWith` tests in the folder are distinct scenarios naming the same direction.

---

## P5 — Repository hygiene

**Mostly done in `7e0911e2`** (re-checked 2026-10-02): the tracked backup is deleted and
`*.bak-prune-*` gitignored; `manifest.json` is untracked and gitignored but kept on disk for
the workflows; `DSA.slnx` wires all six projects; `Fixtures.tmp_check/` no longer exists.
What remains:

- `BenchmarkDotNet.Artifacts/` exists in two places (root and under `Benchmarks/bin/...`).
- The `InternalsVisibleTo` lists are hand-maintained per project (5/4/1/0 entries), so
  adding a project means editing several `.csproj` files.

**Action:** remove the tracked backup and the stray temp dir; add a solution file; decide
where `LeetCodeCatalog` belongs — it is I/O infrastructure living in a test project.

**Done when:** `git ls-files` contains no backup or workflow-intermediate artifact, and a
single root command builds and tests all projects.

---

## P6 — Replace the 1:1 benchmark companion tree with one generic check

`Benchmarks.Tests/ProblemSolutions/` holds one `<Benchmark>Tests` class per benchmark —
1,112 classes, 53,259 lines — and 92.5% of their 3,071 test methods assert one of two
properties in different words: the workload rebuilds identically (958) and the arms agree
(1,884). The tree is 1:1 because `check-test-coverage` attributes a subject by test class
name, not because the tests need it.

**Step 1 — DONE (`85fb3200`).** `BenchmarkArmsTests` asserts both properties once, over
every class with a `[Benchmark]` method, alongside the existing tree. 2,220 rows, 13 s,
stable across three runs; five deliberate breaks each caught and restored hash-identical.

The measurement that step was for: **23 of 2,222 rows failed on the first run, none of them
setup determinism**, and **21 table entries** in `ArmAgreement` account for them — not the
~122 predicted (54 unordered + 60 node-returning + 8 void):

| Entry | Count | Why |
| --- | --- | --- |
| Unordered | 9 | LeetCode leaves the order free (AccountsMerge, ThreeSum, Subsets, …) |
| Random draw | 6 | each arm consumes the seeded generator differently |
| Discarded result | 3 | `DeleteNodeInALinkedList`, `FlattenBinaryTreeToLinkedList`, `InvertBinaryTree` rewrite a copy and drop it |
| Answers differ by design | 1 | `InsertIntoABinarySearchTree` returns the root of a different valid tree per arm |
| Workload outside the contract | 1 | `TopKFrequentElements` (below) |
| Excluded | 1 | the registry harness — its arms are `[ParamsSource]` values |

The 60 node-returning arms needed nothing: `AnswerGraphText` walks fields, with cycles
back-referenced. 5 of the 8 void arms compare by the harness state they leave. PowXn's
last-bits difference is absorbed by a general rule (doubles to 12 significant digits).
45 of the 55 classes whose companions compare unordered happen to agree in order today, so
the theory holds them to exact order — stricter than the problem; a future reordering arm
fails loudly and costs one table entry.

**Found by it — FIXED since:**

- **`TopKFrequentElements`' workload tied 23 values at the top-10 boundary**, outside LC
  347's "it is guaranteed that the answer is unique" — the same species as the old
  AccountsMerge name collision. The workload now plants its answer (the values 0–9 each
  outnumber every other value by construction, then a seeded shuffle), keeping the
  distinct-value count at the old scale; the companion pins the planted set at both
  benchmarked lengths, and setting the margin to 0 fails all four pins and the generic check.
  It moved from "incomparable" to "unordered".
- **The 3 discarded-result void arms** (`DeleteNodeInALinkedList`, `FlattenBinaryTreeToLinkedList`,
  `InvertBinaryTree`) now return what they build as `object?`, as the 60 node-returning arms do,
  and are held to exact agreement. Flatten's companion replays the solution methods rather than
  the arms, so it stayed green when one arm was made to skip flattening; the generic check failed.

**Step 2 — deletion manifest — DONE (`f924c5f0`, 620 files, 25,985 lines).** Built from
assertion content with Roslyn, not from test names. A companion test method counts as
subsumed only when every assertion is `Assert.Equal` of the *same* projection of two arm
answers — `f(armA)` vs `f(armB)`, or `f(arm)` vs `f(arm)` on two separate harnesses — at
the smallest `[Params]` value, with no assertion inside a helper. Anything else keeps the
file.

| Companion files | Count | Why |
| --- | --- | --- |
| **Deletable** | **620** | every assertion is arm-vs-arm at the smallest parameters |
| Pin something | 454 | an expected value, a shape, or an `Assert.True`/`InRange` the theory cannot replace |
| `ArmAgreement` entry | 20 | the theory does not hold their arms to exact agreement |
| Other parameters | 14 | they test at a size other than the smallest (e.g. PowerOfFour on both sides of 4^15) |
| Not a companion | 7 | tests for helper types (`RankHooks`, `SeededRandomRand7`, …) |
| Other | 2 | one asserts an arm twice on one harness; one file holds two classes |

The 739 determinism methods compare an arm's *answer* on two fresh harnesses, which the
theory's workload comparison did not strictly imply, so the theory gained a third row
(`Arms_RebuiltHarness_AnswerTheSameAgain`) before the manifest was trusted. Verified two
ways without touching the tree: a copy of the test project without the 620 files builds
and passes 5,477 tests (7,117 − 1,640), and a full repo mirror without them gives the
testing phase identical findings.

**Found by the new row — FIXED since:** `RandomPickIndexSolution` and
`GenerateRandomPointInACircleSolution` each constructed an unseeded `new Random()`, so even
one arm answered differently on every run. Every strategy now takes its seed from the caller,
the shape `RandomPickWithBlacklistSolution` already had; `ArmAgreement.UnseededAnswers` is
gone rather than left empty, so a future unseeded arm fails the row instead of being listed.

`ArmAgreement` after these fixes: **10 unordered, 7 incomparable** (6 random draws and
`InsertIntoABinarySearchTree`, whose arms build different valid trees by design), **1 excluded**
— 18 entries, down from 21 plus the 2 unseeded.

**The coverage gate depends on uncommitted nomos work.** The `check.exe` in
`code-standards` is built from a working tree whose `csharp_test_coverage.go` carries an
uncommitted exemption: members with a BenchmarkDotNet lifecycle attribute (`[Benchmark]`,
`[GlobalSetup]`, `[Params]`, …) are runner entry points, not promises to a caller. Measured
`check-test-coverage` live findings:

| Tree | committed nomos (`f0d82072`) | working-tree `check.exe` |
| --- | --- | --- |
| repo today | 204 (all in `DSAExperimentation.Benchmarks/`) | 10 (`Baseline/`, P3) |
| without the 620 | 1,843 | 10 |

So with the exemption landed, the deletion needs no waiver — and the 194 benchmark findings
the repo already carries under committed nomos disappear too. Without it, a glob waiver on
`DSAExperimentation.Benchmarks/ProblemSolutions/*Benchmarks.cs` is the scoped form: all
1,106 files it matches hold a `[Benchmark]` class the theory discovers, and it leaves out
the four non-benchmark helpers in that folder. The 10 `Baseline/` findings are live under
either binary and are P3's.

**Remaining steps:**

1. ~~Apply the deletion~~ — done in `f924c5f0`; the tree passes 5,477 benchmark tests.
2. **Land or drop the nomos exemption**, which decides whether a waiver is needed. Until it
   lands, committed nomos reports the 620 deleted companions' classes as uncovered.
3. ~~`ARCHITECTURE.md`~~ — done: the convention was written down nowhere, though 485 kept
   companions cite "ARCHITECTURE 17.9" for it. §17.9 now carries it, which makes those
   citations true rather than churning 485 comments.
4. **The kept files still hold 629 arm-only methods beside their pinned ones** (of 1,376
   classified there); pruning those is a method-level bulk edit, a later pass.

**Done when:** the companion tree holds only tests that pin something the theory cannot,
the coverage gate accounts for the rest by a stated rule, and §17 describes that shape.

---

## What is healthy

Not everything here is debt, and the worklist should not obscure that:

- The tier order is enforced by tests that read the repo as text — `Architecture/LayeringTests.cs`,
  `Tier5WitnessTests.cs`, `RepositoryFiles.cs` — not by prose.
- Corpus coverage is essentially complete: 1,105/1,105 problems have a per-problem test, and
  every benchmark class is a case of `BenchmarkArmsTests`, which asserts by reflection that its
  arms agree and its workload and answers rebuild identically.
- Every `[Benchmark]` arm is run and compared with its baseline by `BenchmarkArmsTests`, so
  the "naive baseline was never tested" gap §17.1 describes is closed for the hand-written
  classes — by construction now, rather than by each arm's name appearing in a companion.
- The benchmark test project is doing real work — it caught the fact that a harness whose
  arms disagree is timing two different problems.
