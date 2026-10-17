# Architecture history

[ARCHITECTURE.md](ARCHITECTURE.md) states each rule as it stands. This file keeps how the rules got
there: the reorgs that moved the tree into its present shape, and the readings of a rule that were
wrong before they were right. Each part is filed under the section it explains, so the history of
§13.5 is under that number here. Paths are as they stood at the time; ARCHITECTURE.md §13.3 says
where everything lives now. Nothing here is a rule, and where this file and ARCHITECTURE.md
disagree, ARCHITECTURE.md is right.

The commit named beside a step is the one that wrote it down; `git show` it for the change itself.

## §1 Purpose, and the caveat §3–§12 carried

ARCHITECTURE.md began (08e02cda, 2026-08-24) as a description of a classification that already
governed `DSAExperimentation/Graph/**` without being written down, so that the next work —
`DSAExperimentation/Collections/**` — would follow it deliberately instead of by accident. §3–§12
were written against that domain-first tree: `Graph/**`, `Collections/Heap/**`, `Searching/**`,
`Sorting/**`, `Traversal/**`. Once §13's reorgs made the axes physical, each of those sections
opened with a note that its paths were as they stood before the reorg, and §2 said the same for all
of them at once. On 2026-10-16 every section was rewritten at current paths and the narrative moved
here; the section numbers did not change, because source comments cite them.

## §5 Retrofitted first, then run forward

`Searching/BinarySearch` (§9) was the first addition built by running the checklist forward, in
order. `Collections/Heap` (§4) and `Graph/**` (§3) arrived at the same shape retrofitted, with the
Topology witness pulled out after the fact rather than named first. Both routes landed in the same
place because the domains were small enough to fix in hindsight; the checklist exists so that stays
true as things grow.

## §6 The first gate pass over `Collections/**`

When `Collections/**` first went through the gate, the fixes were real: a shared `ArrayGrowth`
constant replaced three independent copies of the same tuning numbers, `HashMap`'s `Set`/`Remove`/
`Grow` were split into named helpers, and `HeapArrayIndex.RightChild` became `LeftChild + 1` instead
of a bare literal. `Sorting/MergeSort.cs` hit two genuine findings, a five-parameter `SortRange` and a
six-parameter `Merge`, both fixed by grouping the range into `SortBounds`, and a five-section `Merge`
was shortened to one counted loop (`MergeRunsIntoBuffer`) instead of three merge/drain loops.

The waivers that remained then were `check-class-size`'s "two disjoint call-graph clusters" on
`Heap`, `DynamicArray`, `CircularBuffer`, `Stack` and `Collections/DisjointSet/DisjointSetForest.cs`;
test-file `check-literals`/`check-function-cohesion`/`check-responsibility-extraction` findings,
reasoned as for `ContiguousGroupBufferTests.cs`/`TraversalTests.cs`; and `check-test-coverage`'s
attribution gap for generically named members (`Count`, `Get`) exercised only transitively. Of the
class-size waivers, only `Heap`'s and `CircularBuffer`'s remain, and the §18 entry below records
what became of the coverage waivers.

## §7 Three asymmetries, fixed

- `DSAExperimentation.Tests/Engines/**`, `Algorithms/**` and `Contracts/Ordering/**` used to drop the
  `Graph/` prefix segment the main project used under `DSAExperimentation/Graph/**`, a leftover of
  `48e4c0b3` (2026-08-24), which restructured the traversal library around the Graph/Dag/Tree tiers
  before the test tree followed. They moved under `DSAExperimentation.Tests/Graph/**`.
  `Collections/Heap`'s tests mirrored `Buffers/ContiguousGroupBufferTests.cs`'s convention instead,
  since `Buffers`, not `Graph`, was the sibling-domain precedent for a small standalone structure.
  §13's reorgs later made the whole test tree mirror `DataStructures/` and `Algorithms/` exactly.
- `Graph/Algorithms/ShortestPaths/ShortestPath.cs`'s Dijkstra/A* used the BCL
  `PriorityQueue<TNode,TWeight>` until they moved onto `Collections/Heap`'s `Heap<T,TOrder>`, through a
  `(TNode Node, TWeight Priority)` element ordered by a priority-only `IHeapOrder` witness. Neither
  `Heap` nor `HeapArray` needed a change.
- `IPathHeuristic.cs`'s contract used to require only an *admissible* heuristic (de17f14f), weaker
  than `ShortestPath.Traverse`'s settle-once design needs; it now requires a consistent one.
  `ShortestPath.cs` gained its note on the non-negative-edge-weight precondition in the same pass.

## §9 `BinarySearch`

`Searching/BinarySearch` (de17f14f) was the repo's first domain outside Graph and Collections, and
its first algorithm bound to a Representation contract it defined for itself. The array overloads of
§9.5 came from a survey of the solution tier: eighty-eight solution call sites spelled out
`MergeSort.Sort<T, ArrayIndexedSequence<T>>(new ArrayIndexedSequence<T>(items))`, and more than forty
solutions built an `ArraySequence` only to search it. `MonotonePredicateSearch` (§9.6, 3beca31e)
replaced twenty-odd solutions' spelling of binary search on the answer as an
`IRandomAccessSequence<bool>` handed to `LowerBound(sequence, true)`; three of them truncated or
threw on ranges LeetCode's own limits allow.

## §10 `DisjointSet`

`Collections/DisjointSet` (60007b0f) was the first equivalence-class structure, and §10.3's
namespace collision was the first time a non-generic type shared its folder's name. §10.4 predicted
that a generic consumer would most likely be a Kruskal's MST under `Graph/Algorithms`, and that the
right shape would be a `DisjointSet<T>` wrapper composing the dense forest plus a
`Dictionary<T,int>`. It arrived as `KeyedDisjointSet<TKey>`, composing `HashMap<TKey,int>` rather
than `Dictionary` for §7's self-hosting reason, and `MinimumSpanningTree.Kruskal` is its consumer.

`MergeInto` (§10.1, caa0d0ac) was added for LC 3161, which had driven `DisjointSetForest` directly
with its own copy of `Find` because `Union` could not link one way.

## §11.4 Element algebras, before the chain

Until 23bcd0ae (2026-10-03) each range structure declared its own algebra: `ICombineOperation` in
`SegmentTree/`, `IGroupOperation` in `FenwickTree/`, `IScaledGroupOperation` in `RangeFenwickTree/`
(the one that did refine its neighbour), and an `IRangeUpdateOperation` that redeclared
`Identity`/`Combine` rather than inherit them — each citing §11.1's domain separation. The cost was
concrete: two byte-identical `SumOperation`s, a third (`ScaledSumOperation`) with the same body plus
`Scale`, and a private `BuildCombine` adapter inside `LazySegmentTree` whose only job was to
re-present `TOperation`'s `Identity`/`Combine` under the other contract's name so both trees could
share one build recursion. The chain replaced one witness per structure with one per algebra, let
one `SumOperation` serve all three summing structures, and deleted `BuildCombine`. The structures'
own constraints read exactly as before.

## §12 `DepthFirstSearch`, and the third answer

`Traversal/DepthFirstSearch` (60007b0f) was the first algorithm with no Representation axis, built
entirely outside `Graph/**`. §12.3 first attributed its value-typed example to "the essay's own
implicit-graph illustration", a chess position generated on demand; the essay is not in this
repository, and the example now rests on `DepthFirstSearchTests`' `Position` struct.

Before §12.4's rule was written down (7e588e1e, 2026-10-03), five fold algebras read their per-call
values from a static `Prepare`, the in-order and level-grouped hooks kept theirs in `AsyncLocal`
slots, and every test fixture that recorded a walk needed a marker type per test to keep its static
log apart. Making those contracts instance members on a struct type parameter retired all three.
When the fold tiers were unified onto one recursion, the tree tier paid four runtime lookups per
node — 40% on `FoldTierBenchmarks.TreeTier` — until it got a recursion of its own back.

## §13 Physical layout: seven reorgs

Until 2026-08-25 the tree was domain-first. `Graph/**`, `Collections/Heap/**`, `Searching/**`,
`Sorting/**` and `Traversal/**` each held one domain's Representation, Topology and Operations files
together, and the three-axis classification was a convention the file tree didn't enforce. Seven
reorgs made it physical:

1. **Split by axis** (60007b0f, documented in 117bc80f). Every file in `DSAExperimentation/` and
   `DSAExperimentation.Tests/` moved under exactly one of `DataStructures/` (Representation and
   Topology) or `Algorithms/` (Operations). Both sides initially kept the same domain-name
   subfolder underneath: `DataStructures/Graph/Engines/Reducing/`,
   `Algorithms/Graph/Engines/Reducing/`, and so on.
2. **`Algorithms/` by utility, not domain** (117bc80f). A domain-name subfolder under `Algorithms/`
   was still domain-first, one level down: it filed `ShortestPath`/`ConnectedComponents`/`Reduce`/the
   `Fold` family under a top-level `Graph/` folder, as if they belonged to "Graph" the data
   structure, when what distinguishes them is what each one does. `DataStructures/` needed no second
   pass, since a structure's identity is its representation and topology.
3. **Operations rejoins Representation** (b14cc528). The utility-first rule still left nine
   Operations types under `Algorithms/`, each named after the one structure it belongs to:
   `Buffers`, `Heap`, `HashMap`, `DynamicArray`, `DisjointSet`, `Deque`, and — one level removed, each
   hardcoding one of those rather than a Storage class of its own — `Stack`, `Queue`, `Set`. They
   moved back into `DataStructures/`, beside their Storage or into a sibling folder next to the type
   each composes. `DisjointSet` dropped its `Collections/` wrapper in the same step.
4. **`Searching`/`Sorting` become `Sequence`** (7e3ab643). `DataStructures/Searching/` and
   `DataStructures/Sorting/` were named after the algorithms that consume them — the second reorg's
   mistake made in the opposite direction — and moved together to `DataStructures/Sequence/`. The
   check surfaced `SearchRange.cs` and `SortBounds.cs` sitting in those folders as if they were
   Representations of the sequence; they moved to `Algorithms/Searching/` and `Algorithms/Sorting/`,
   correcting a misclassification that predated every reorg.
5. **`BinaryTree` rejoins `Graph`; `ICache`** (0f690716). `BinaryTree` sat as a sibling of `Graph/`
   even though `BinaryTreeTopology.cs` implements `ITreeTopology` and its own doc comment said it
   "mirrors GridTopology exactly"; it moved to `Graph/Engines/Dags/Trees/`. `LruCache`/`LfuCache`,
   documented as "peer implementations of an almost-identical public contract", moved from top-level
   siblings into `Cache/` beside a new `ICache.cs`, which made that contract real.
6. **`LowercaseTrie`** (0f690716). `Trie.cs`, `BitTrie.cs` and `SuffixTree.cs` had all justified
   staying witness-less with some form of "nothing for a witness to parametrize". §13.8 named that as
   the wrong question and added `LowercaseTrie`, which had been rehearsed non-generically as a
   test-only fixture first; that fixture was deleted once `LowercaseTrieTests` and
   `SparseArrayChildrenTests` checked everything it had.
7. **`BitTrie` gets a witness** (0f690716). `BitTrie.cs`'s doc comment argued "no Topology witness
   here" because `BitTrieNode` is only ever created internally, so no external shape could violate
   the invariant — §13.8's wrong question again, not yet corrected for that file. Applying the
   two-part test of §13.9 promoted it.

Two test-tree changes rode along. A second, test-only trie fixture group, superseded by the
production `LowercaseTrie`, was deleted. `GraphTests.cs` was a cross-cutting test file whose every
test drove exactly one graph-safe entry point, so §18's pass dissolved it into those entry points'
own test classes, and its cyclic sample became `TestGraphs`.

### §13.4 `suppressions.json`

The waiver ledger was first built on 2026-08-24 (023cdce8). It was deleted at the start of the axis
reorg (60007b0f) and left deleted while files moved, so every judgment call it had waived — the
`check-class-size` "two clusters" pattern, several test-file literal findings, a
`check-test-coverage` attribution gap — resurfaced as a raw finding. It was rebuilt the same day
(56db09fc) and has been maintained since, but §13.4 went on saying it was deleted and not rebuilt
until 2026-10-16.

## §14 The order the decompositions were done in

The three Storage/Operations decompositions were done cheapest first, so the pattern was rehearsed
before it carried risk: `Buffers` (no consumers anywhere at the time), then `HashMap`, then
`DynamicArray`, the highest fan-out. Each was checked against the full test suite before the next.
`MarkCurrentKeyConsumed`'s keep-the-stale-key behaviour had no test until the `Buffers` split added
one, and `DynamicArray`'s consumers needed only a `using`-line repoint.

## §15 and §16 Naming and failure reporting

§15.3 (5cebd437) recorded `TopologicalSort` as the first `Algorithms/` folder whose category and sole
technique share a name. §16.3's first design (8b77a925) leaned on `ShortestPath.Dijkstra`/`.AStar`
and `MinimumSpanningTree.Kruskal` as precedent for an eponymous `FloydWarshall` method on the
descriptively named `AllPairsShortestPaths`; asking what licenses those three eponymous methods — an
uncheckable precondition — turned it into `TryComputeDistances`.

The dense matrix was later built by `DenseDistanceMatrix.cs` rather than inside the walk. §16.6 said
the `MaxValue` sentinel was a guard "no caller action can violate or satisfy". That was true of the
overflow guard and not of the sentinel itself, since seeding rejects an edge weight equal to
`TWeight.MaxValue` with an `ArgumentOutOfRangeException`, so the section now states both halves.

## §17 The tiers above the library

§17 was written (a7b4bb42, 2026-09-01) when the LeetCode coverage moved out of the test and
benchmark projects into a solution tier of its own.

### §17.1 What the solution tier was built to undo

A LeetCode coverage entry was a test *and* a benchmark, written independently, and before the tier
existed:

- **The solution was written twice, and drifted.** `AddBinaryTests` carried a
  `private static string Add(...)`; `AddBinaryBenchmarks` re-implemented the same carry walk as
  `StackBits`. `TwoSumTests` solved for the pair of *indices*; `TwoSumBenchmarks`'s `HashMapOnePass`
  returned a *bool*. The benchmark was not measuring the function the test proved correct, and
  nothing in the repo required it to.
- **Every naive baseline was untested.** A benchmark's `[Benchmark(Baseline = true)]` arm was never
  asserted against anything.
- **Domain types were copy-pasted across projects.** `Fixtures/LockNode.cs` in the benchmarks
  project opened with the comment *"Mirrors DSAExperimentation.Tests' OpenTheLock LockNode
  fixture"*. `WordNode`/`WordTopology` existed in two test folders, again as
  `WordLadderNode`/`WordLadderTopology` in the benchmarks project, and a fourth and fifth time as
  `GeneNode`/`GeneMutationNode` for LC 433 — five copies of one Hamming-distance graph, with
  `IsOneLetterApart`/`IsOneCharApart` written out four times.
- **Test-project code reached across its own layers.** `MinimumTimeVisitingAllPointsTests`, a
  LeetCode coverage test, imported `DSAExperimentation.Tests.Algorithms.ShortestPaths.Fixtures` to
  borrow `WeightedGridNode` and `ChebyshevHeuristic`.

The pilot migration alone took `TwoSum`, `AddBinary` and `OpenTheLock` from 4 assertions to 28.

### §17.6 The criterion that first got `Domain/` wrong

The first draft of §17.6 said a type earns `Domain/` when it is "(a) too specific to be a data
structure or algorithm, and (b) used by more than one problem." That was wrong, and the failure is
seductive: (b) is a sharing test, not a classification test. The duplication was the visible,
countable evidence — five copies of one Hamming graph — so it did all the work, (a) was never given
teeth, and every shared-but-not-obviously-generic type got swept into `Domain/`. Three
misclassifications followed, all caught on review:

| Was | Is now | Why the original was wrong |
| --- | --- | --- |
| `Domain/Grids/WeightedGridNode`, `WeightedGridTopology`, `WeightedGrid` | `DataStructures/Graph/Grids/` | `DataStructures/Graph/Grids/` already held `Grid`/`GridNode`/`GridTopology`/`GridChildren` — the unweighted (`IGraphTopology`) sibling of the same thing. Splitting siblings across two tiers is indefensible. |
| `Domain/Grids/ManhattanHeuristic`, `ChebyshevHeuristic` | `Algorithms/ShortestPaths/` | They implement `IPathHeuristic`, an `Algorithms/` contract, and belong beside `ZeroHeuristic`. `IPathHeuristic`'s own doc comment already read *"Every heuristic in this codebase (ZeroHeuristic, ManhattanHeuristic) is consistent"*. |
| `Domain/RootedTrees/*`, `Domain/Hamming/*` | `DataStructures/Graph/Engines/Dags/Trees/`, `DataStructures/Graph/Hamming/` (+ `Algorithms/ShortestPaths/Hamming/HammingDistances.cs`) | Pure Representation + Topology. `RootedTreeNode` is the n-ary sibling of `BinaryTreeNode`, already in that folder, and the Hamming graph's edge rule is a shape rule like `GridTopology`'s. |

That left three folders in `Domain/`: `Locks`, `Modular` and `SlidingPuzzle`.

### §17.6a How the tier order came to be enforced

The tiers were a convention the file tree implied and nothing checked, and four files under
`DataStructures/` were reaching up into `Algorithms/` unnoticed. Three only wanted
`AlgorithmConstants`' two integers, so that type moved down to `DataStructures/` despite its name.
The test projects were split the same way later: 1,159 solution-facing files had sat in the core
test project, and none of its 188 core files referenced them.

### §17.8 One measurement changed on purpose

`WordLadderIIBenchmarks` had both arms *count* shortest sequences rather than build them, to avoid
materializing a potentially exponential result set. Once both arms were the methods
`WordLadderIISolutionTests` proves correct, they built LeetCode's actual answer, and the harness
was changed to return it whole rather than its `.Count`, so the generic arm check compares the
sequences themselves — a deliberate change in what is measured.

### §17.9 Drafts the gate corrected

- `HammingSearch` first exported an `Unreachable = -1` sentinel, and `check-constant-placement` was
  right that a public constant on a search type is a smell; it returns `int?` instead.
- `Unreachable = -1` had also been copied into two unrelated solution classes, which
  `check-duplicate-constant` caught; `LeetCode/LeetCodeAnswer.cs` was created to declare it once.
- `LockWheels` was split out of `LockGraph` when the tier was created, doing exactly what
  `check-constant-placement` prescribes, and the check still fired on it.
- A first draft of §17.9 asserted that `check-test-coverage` demands `Test_<Member>_Should_...`
  names in a file beside the source, that no test in this repo used either, and therefore that
  writing tests could not clear the finding — and waived it on that basis. All three claims were
  wrong. The waivers were withdrawn and replaced with real tests: 15 new files covering all 23
  flagged members across `DataStructures/Graph/{Hamming,Grids}`, `DataStructures/Graph/Engines/Dags/Trees`,
  `Algorithms/ShortestPaths` and both `Domain/` folders. Two of those tests failed on first run, both
  bugs in the tests rather than the code.
- 620 benchmark companion test files that compared a class's arms only with each other were
  deleted once `BenchmarkArmsTests` made that check generic.
- The rule against `<remarks>` was established by the registry harness's `IBoundWorkload`, whose
  design essay was moved into this document rather than waived; the rule outlived the registry.
- §17.9 ended by noting two `check-nested-call` finding families in benchmark files of problems not
  yet migrated (`CountUnreachablePairsOfNodesInAnUndirectedGraph`, `DeleteGreatestValueInEachRow`,
  `LongestIncreasingSubsequenceII`, `SellingPiecesOfWood`), expected to clear as they migrated. All
  four had migrated by 2026-10-16, and the note was dropped.

### §17.11 The registry harness

For a time a second harness shape existed beside the hand-written one: a problem could add a
`<Problem>Registration` describing its strategies, cases and benchmark workloads, and one generic
test and one generic benchmark class ran every registered arm. It was built to carry the migration,
and the migration landed on the hand-written shape instead: ten problems were ever registered, each
kept its hand-written test and benchmark as well, and nothing else used it.

It was retired rather than finished (527a541d, 2026-10-03), for two reasons. Finishing meant
generating the 1,100 hand-written benchmark classes from registrations — and a registration sized
its workloads inside tier 4, which is the measurement decision §17.7 assigns to tier 5. And its one
unique enforcement, that a solution's every strategy is asserted, held for every problem through
`LeetCodeStrategyCoverageTests` rather than for the ten that were registered. The cases a
registration carried beyond its problem's own test were ported into that test before deletion, and
what every harness had used from it stayed, as `Conventions/`.

## §18 The coverage pass

§18 was written (a7b4bb42) with a pass that gave tiers 1–3 their direct unit tests. The repo had
**59 `check-test-coverage` waivers** across `DataStructures/` and `Algorithms/`, most arguing that
the members were exercised transitively and that the check was mis-attributing, and §17.9's first
draft claimed writing tests could not clear the check at all. The error compounded: one bad reading
of the tool produced a waiver, and the waiver then stood in for the missing tests. `HeapTests.cs`
had been passing the check all along with ordinary names.

Direct unit tests were written for about 50 source files in that pass — the whole
`Graph/Contracts/Ordering` witness family, the `Graph/Hamming` and weighted-`Grids` structures, the
rooted-tree primitives, `Walking/**` (which had no tests at all), `TopDownTraversal`/`TopDownWalk`
(likewise), the tree/trie/grid topology witnesses, the metric algebras, the three storage internals,
and both `Domain/` folders, for which `Tests/Domain/**` was created — taking the suite from about
2,900 to about 3,550 tests and retiring 35 waivers outright. The witness types were the largest
single block of untested surface.

The rest of that backlog was listed file by file in `suppressions.json`, each waiver naming its
members and stating that it recorded debt rather than an exemption. It closed three ways:
family-named test files were renamed after the type they test (`FoldTests.cs` became
`TreeFoldTests.cs`, `MetricsTests.cs` became `TreeMetricsTests.cs`); `TraversalTests.cs` and
`GraphTests.cs` were split into one class per entry point; and the tests that were genuinely missing
were written. The last member open was `HooksStep.Seed`, then nested in `BreadthFirstWalk` and
`DepthFirstWalk` and dead: each engine's void `Walk` overload started from `default(Unit)` and never
read it. It closed when traversal hooks became instance values that each traversal threads through
`Reduce` as the state; `HooksStep` moved into the traversals, and their `(root)` overloads start from
it, so `Seed` is the starting hook of every walk handed no hook of its own, and a test names it.

`LeetCodeStrategyCoverageTests` (§18.5) came later. Before it, the `check-test-coverage` waiver over
the solution tier called the tier "complete by construction" on a claim alone, and
`KthSmallestElementInABSTSolution` had no caller at all.
