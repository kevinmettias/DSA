# DSA improvement worklist

Prioritised from three read-only surveys of this repository plus direct measurement,
2026-10-01. Items marked ✓ were measured first-hand; the rest are survey-reported and
should be re-checked before acting — one survey got a count wrong (it reported 4
problems with no benchmark; the measured answer is 11).

Scale for orientation: 6 projects, no `.sln`; 1,105 LeetCode problems; 1,105 coverage
tests; 1,111 benchmark classes (1,106 one-per-problem, 4 in `StrategySwaps/`, plus the
generic registry harness) each with a companion test; ~10,346-line core library
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
  1,106/1,106 per-problem benchmark classes have a companion test asserting their arms agree
  and their workloads rebuild identically.
- Every `[Benchmark]` arm name appears in its test file, so the "naive baseline was never
  tested" gap §17.1 describes is closed for the hand-written classes.
- The benchmark test project is doing real work — it caught the fact that a harness whose
  arms disagree is timing two different problems.
