# Architecture

## 1. Purpose

This document states the rules that decide what a type in this repository is, where its file goes,
and what it may depend on. Each section states the rule in force and names files at their current
paths. How a rule came to be — the reorgs that moved the tree into its present shape, and the
drafts that got a rule wrong before it was right — is in
[ARCHITECTURE-HISTORY.md](ARCHITECTURE-HISTORY.md), filed under the number of the section it
explains.

Section numbers are stable. Source comments cite them ("ARCHITECTURE 17.9", "§17.3"), so a section
is rewritten in place and never renumbered, and a new one takes the next free number or a letter
suffix (§17.6a). `DSAExperimentation.Tests/Architecture/SectionCitationTests.cs` fails when a
citation anywhere in the code, the documents or the waiver ledger names a number no heading here
carries (§18.5). A numbered step inside a section is not a section: cite it as "§5 step 5", never
with a dotted number of its own.

§2 defines the three axes everything else applies. §5 is the checklist for adding anything, §8 the
rule for binding an algorithm to what it needs, §13 the physical layout, §17 the tiers above the
library and §18 the testing policy. §3, §4, §9–§12, §14–§16, §19 and §20 are worked examples: each
records a classification call and the reason for it, so that the next similar call can be made the
same way.

## 2. The three axes

Every data structure in this repo is designed along three independent axes:

- **Representation** — physical storage. Contiguous array, pointer/list-linked, arena+indices,
  sparse slot array, geometry-computed. This is *where things live* and *how they're reached* —
  and, just as importantly, *what reaching them costs*. That cost is a real contract even though
  C#'s type system can't state it: `IChildren<TNode>.Get(int)` is one syntactic contract, but
  `ListChildren.Get` is O(1) (a `List<T>` indexer) while `SparseArrayChildren.Get`/`GridChildren.Get`
  are O(k) bounded scans (a slot array / the 4 grid directions). No graph engine depends on an O(1)
  `Get` — each sweeps `0..Count-1` in order (§13.8) — but the obligation is real, and §8 says why a
  cost belongs in the contract.
- **Topology** — the admissible relation/shape. General graph, DAG (acyclic), tree (acyclic +
  unique ancestry), heap-order, BST-order. A stronger topology promise lets algorithms skip
  runtime defense (cycle checks, visited-tracking, rebalancing) at compile time instead of
  paying for it at run time.
- **Operations** — the algorithm/API surface built on top of a Representation constrained by a
  Topology.

The axes are also the physical layout. Representation and Topology files live under
`DSAExperimentation/DataStructures/`, together with any Operations type hardwired to one of this
repo's Representations (§5 step 7); every other Operations type lives under
`DSAExperimentation/Algorithms/`. §13 says where within each.

A fourth axis, "Invariants," was considered and folded into Topology: a topology's
admissible-relation-set already *is* its invariants (acyclicity, unique ancestry, heap order,
BST order are all statements about which relations are legal), so tracking it separately would
just be re-describing Topology from a different angle.

**The illustrating example**: a binary heap has tree *topology* (each slot has at most two
children, no cycles, one root) but is conventionally given array *representation* instead of
pointers — `parent(i) = ⌊(i-1)/2⌋`, `left(i) = 2i+1`, `right(i) = 2i+2`. Same logical shape,
completely different physical layout and cost profile. This is exactly why Representation and
Topology have to be tracked as separate axes rather than conflated, and §4 is that heap:
`DataStructures/Heap`.

## 3. Worked example: the graph framework

The graph framework's Representation and Topology contracts live in
`DSAExperimentation/DataStructures/Graph/**`, and the engines and facades built on them in
`DSAExperimentation/Algorithms/**`.

### 3.1 Representation

- [`DataStructures/Graph/Contracts/Ordering/IChildren.cs`](DSAExperimentation/DataStructures/Graph/Contracts/Ordering/IChildren.cs) —
  an indexable view over a node's children. Deliberately *not* `IReadOnlyList<TNode>`: `Get` is a
  named method, not an indexer, specifically so implementations stay thin structs that JIT-specialize
  to direct, non-virtual calls with no boxing.
- [`ListChildren.cs`](DSAExperimentation/DataStructures/Graph/Contracts/Ordering/ListChildren.cs) — pointer/`List`-backed.
- [`SparseArrayChildren.cs`](DSAExperimentation/DataStructures/Graph/Contracts/Ordering/SparseArrayChildren.cs) —
  fixed slot-array backed (e.g. a slot per letter of a small alphabet), scans past nulls instead of
  storing a materialized list.
- [`DataStructures/Graph/Grids/GridChildren.cs`](DSAExperimentation/DataStructures/Graph/Grids/GridChildren.cs) —
  a third representation: children aren't stored at all, they're computed on demand from geometry
  (row/col offsets filtered by what the grid says is in bounds and passable). `DataStructures/Heap`'s
  index arithmetic (§4) is the same move.
- `IEdges.cs`/`IChildOrder.cs` round out the representation contracts: edge-aware children
  (`ListEdges`, `EdgeTargets`) and order-independent-of-topology traversal (`NaturalChildOrder`,
  `ReverseChildOrder`, `ReversedChildren` — allocation-free reversal via index arithmetic over an
  existing `IChildren`). Every engine entry point that takes the `TOrder, TOrderedChildren` pair
  also has an overload without it, forwarding `NaturalChildOrder<TNode, TChildren>, TChildren` — the
  topology's own order, which nearly every caller wants — so the pair is spelled out only where the
  order is a real choice. The overloads differ in generic arity, so with type arguments always
  explicit no call can bind the wrong one; the strategy and engine methods that implement an
  interface (`IReduceOrderStrategy`, `IFoldEvaluationStrategy`, the walks) keep the pair.

### 3.2 Topology

A refinement chain of phantom interfaces — each adds zero members, only a stronger promise:

```text
IGraphTopology<TNode,TChildren>   bare adjacency, no acyclicity promise
        |
IDagTopology<TNode,TChildren>     + acyclicity (shared descendants OK, cycles are not)
        |
ITreeTopology<TNode,TChildren>    + unique ancestry (no sharing at all)
```

- [`IGraphTopology.cs`](DSAExperimentation/DataStructures/Graph/Contracts/Topologies/IGraphTopology.cs) — the
  general contract every walker/fold/reduce actually needs: given a node, what's adjacent to it.
  Algorithms constrained on this bare interface can't assume acyclicity and must defend against
  cycles/sharing themselves.
- [`IDagTopology.cs`](DSAExperimentation/DataStructures/Graph/Engines/Dags/IDagTopology.cs) — promises acyclicity.
  This is exactly what a fold needs and exactly what bare `IGraphTopology` doesn't promise, which is
  why `CheckedFold` has to accept any `IGraphTopology` and defend itself at runtime while `DagFold`,
  constrained on this tier, skips that defense entirely.
- [`ITreeTopology.cs`](DSAExperimentation/DataStructures/Graph/Engines/Dags/Trees/ITreeTopology.cs) — additionally
  promises unique ancestry, which is what lets the tree-only walkers skip visited-tracking/memoization
  and stay zero-cost.

The payoff is concrete, not just documentation: [`UnguardedVisit.cs`](DSAExperimentation/Algorithms/Walking/UnguardedVisit.cs)
(stateless, tree tier) vs. `TrackedVisitGuard.cs` (`HashSet`-backed, graph tier); `TreeFold.cs` vs.
`DagFold.cs` vs. `CheckedFold.cs` in `Algorithms/Folding/` — three tiers of the same catamorphism,
each paying only the defense its topology tier doesn't already rule out.
`IEdgeTopology.cs`/`EdgeTopologyAsGraphTopology.cs` show topology-to-topology *composition*
(projection, not inheritance); `Grids/GridTopology.cs` shows a topology witness sitting over an
arithmetically-computed representation.

### 3.3 Operations

Two layers: generic **engines** (`Algorithms/Reducing/Reduce.cs`'s `Tree`/`Graph` entry points,
`Algorithms/Traversal/{BreadthFirst,DepthFirst,TopDown}/**`, `Algorithms/Walking/**`,
`Algorithms/Folding/**`) and user-facing **facades** built by closing those generics over concrete
node/topology types (`Algorithms/{Ancestry,Connectivity,Metrics,Paths,ShortestPaths}/**`).

[`IPathHeuristic.cs`](DSAExperimentation/Algorithms/ShortestPaths/IPathHeuristic.cs)/`ZeroHeuristic.cs`
is the template for "one algorithm, one injected axis, not two parallel implementations" —
`ShortestPath.Dijkstra` is `ShortestPath.AStar` closed over `ZeroHeuristic`. `DataStructures/Heap`'s
`IHeapOrder` uses the same idiom (§4).

## 4. Worked example: `DataStructures/Heap`

`Heap` is where the framework meets a structure that is not a graph, and it doesn't transplant
1:1 — the mismatch is worth stating plainly. In Graph, Topology is generic over an
*independently-pluggable* children representation (`IGraphTopology<TNode,TChildren>`), because a
graph node's children exist as a separate thing from any one way of storing them. A heap has no
separate node type: "children" *is* index arithmetic over the same flat array. There's nothing for
a representation type parameter to vary independently of.

The resolution: heap-order Topology narrows to a relation between two stored values, generic over
the element type alone — no representation parameter at all, a strictly simpler shape than
`IGraphTopology<TNode,TChildren>`, and deliberately so.

| File | Axis | Why |
| --- | --- | --- |
| `DataStructures/Heap/IHeapOrder.cs` | Topology | The ordering invariant (min vs. max), generic over `T` only |
| `DataStructures/Heap/MinHeapOrder.cs`, `MaxHeapOrder.cs` | Topology | Closed-set witnesses of `IHeapOrder<T>` |
| `DataStructures/Heap/HeapArrayIndex.cs` | Representation | Pure `parent`/`left`/`right` index arithmetic — the array *is* a complete binary tree; parallels `GridChildren`'s coordinate arithmetic |
| `DataStructures/Heap/HeapArray.cs` | Representation | The growable backing store |
| `DataStructures/Heap/Heap.cs` | Operations | `Push`/`TryPop`/`TryPeek` — the sift walk that combines the arithmetic and the comparison; owns neither on its own |

Representation contracts here are **domain-local**, not shared with `Graph.Contracts.Ordering`.
`IChildren`'s own doc comment already explains why: it exists as an interface only because Graph
needed two genuinely different shapes (`ListChildren`, `SparseArrayChildren`) from day one.
`Heap` has exactly one representation, so it gets a concrete class (`HeapArray<T>`), not an
interface — see §5 step 3 for when that should change.

`Algorithms/ShortestPaths/ShortestPath.cs` is `Heap`'s consumer: Dijkstra/A*'s
frontier is `Heap<(TNode Node, TWeight Priority), ByPriorityOrder<TNode,TWeight>>`, where
[`ByPriorityOrder.cs`](DSAExperimentation/DataStructures/Graph/ShortestPaths/ByPriorityOrder.cs) projects
`IHeapOrder<T>` down to "compare `.Priority`, ignore `.Node`" — the same projection move
`EdgeTopologyAsGraphTopology` makes for `IGraphTopology`, just for a different domain's Topology
witness. Note this doesn't violate "domain-scoped contracts": `ByPriorityOrder` *implements*
`DataStructures.Heap`'s public `IHeapOrder<T>` as a client, the same way any consumer of a library
type would — it isn't Graph reaching in to redefine or reuse Heap's own Representation.

### 4.1 The other containers

Each follows §5's recipe rather than repeating Heap's exact shape — none of them has an
independently-pluggable Topology witness, because none needed one:

| Structure | Representation | Operations | Notes |
| --- | --- | --- | --- |
| `DataStructures/DynamicArray/` | itself (a manually-doubled `T[]`, held by `DynamicArrayStorage`, §14.3) | `Add`/`Insert`/`RemoveAt`/`Get`/`Set`, each with a `Try` form | No Topology axis at all — the only invariant is "how it's stored," which §5 step 2 classifies as Representation, not Topology |
| `DataStructures/Stack/` | `DynamicArray<T>` (composed, not duplicated) | `Push`/`TryPop`/`TryPeek` | A stack is a sequence plus a LIFO *access constraint* — an Operations-level restriction over an existing Representation, not a new physical layout |
| `DataStructures/CircularBuffer/` (`CircularBuffer` + capacity witnesses) | a wraparound array (`_head`/`Count` masked by a power-of-two length, §20) | `TryPushFront`/`TryPushBack`, `TryPopFront`/`TryPopBack`, `TryPeekFront`/`TryPeekBack`, `IsFull` | A second, distinct Representation (wraparound, not flat) — earned by needing O(1) at *both* ends, which `DynamicArray`'s flat layout can't give the front. Its capacity is an injected invariant, unbounded or bounded at k (§20) |
| `DataStructures/Deque/` (`Deque` + `BoundedDeque`) | `CircularBuffer<T, …>` (derived, its capacity fixed) | `PushFront`/`PushBack` (`TryPush*` when bounded), `TryPopFront`/`TryPopBack`, `TryPeekFront`/`TryPeekBack` | Both ends of the buffer, unbounded or bounded: the configurations of §20 that keep every operation, so each is the buffer itself |
| `DataStructures/Queue/` (`Queue` + `BoundedQueue`) | `CircularBuffer<T, …>` (composed) | `Enqueue` (`TryEnqueue` when bounded), `TryDequeue`, `TryPeek`/`TryPeekRear` | FIFO is the buffer restricted to one end each — the same "sequence + constraint" relationship Stack has with `DynamicArray`, just against the wraparound Representation |
| `DataStructures/HashMap/` (`HashMap` + `HashMapEntry` + `HashMapStorage`) | bucket array + separate-chaining entries array with a free list | `Set`/`TryGetValue`/`TryRemove`/`HasKey` | The associative primitive from §2 in code: no Topology witness, because "at most one value per key" is what a map *is*, not a variable choice the way min-vs-max-heap is |
| `DataStructures/Set/` | `HashMap<T,bool>` (composed) | `TryAdd`/`Has`/`TryRemove` | Backed by `HashMap` the way `java.util.HashSet` is backed by `HashMap<E,Object>` — membership is "is this key present," so the value is an unused placeholder |

`Stack`/`Queue` composing `DynamicArray`/`CircularBuffer` rather than each managing their own array
is the point of this table made concrete: *a stack isn't a unique physical structure — it's a
sequence plus a LIFO access constraint.* Representation is shared because the constraint, not the
storage, is what varies.

## 5. How to add anything here — a capability-first checklist

The lever for decoupling an algorithm from a data structure isn't more abstraction, it's naming
what's essential *before* anything concrete exists to tempt you into skipping the naming.
Retrofitted decoupling — writing the concrete representation first, extracting a capability
interface afterward — is exactly where representation assumptions leak into an algorithm's body
unnoticed, because nothing forced you to state them first. So whether the next thing is a new
algorithm, a new structure, or both, run this in order:

1. **Name the operations (`O`)** the algorithm actually calls — the minimal method surface, not
   the concrete type you'd reach for by habit. `BinarySearch` needs `Length`/`Get(i)`; it doesn't
   need `Array<T>`. `DFS` needs "enumerable successors"; it doesn't need `TreeNode`.
2. **Name the laws (`L`)** — everything the algorithm assumes but can't check — and classify each
   one before deciding how to encode it:

   | Question | Classification | Example |
   | --- | --- | --- |
   | Does the algorithm *actively re-derive* it every step, and does the result change control flow? | **Topology witness** — a closed set of `struct`-constrained, `static abstract` interfaces, generic over the element/key type only, never over storage | `IHeapOrder.HasPriority`, checked on every sift comparison |
   | Is the space of valid choices open-ended (any caller-supplied rule, not a set this library enumerates itself)? | A plain runtime object, not a witness | `IComparer<T>`, `IEqualityComparer<TKey>` |
   | Is it established once before the call and never re-touched — including "how it's stored"? | **Representation**, or an unenforced **precondition law** stated in a doc comment | Sortedness (`BinarySearch`), non-negative edge weights (`ShortestPath`), "how it's stored" generally |
   | Does violating it change correctness, or only performance, silently? | **Complexity law** — state the *obligation* on the contract, the *consequence* on the algorithm | `Get` assumed O(1); a slower representation degrades big-O with no compiler error |

3. **Only then write the Representation.** If exactly one physical layout exists today, make it a
   concrete class, not an interface — don't introduce the interface until a second implementation
   exists to justify it. `SparseArrayChildren` earned `IChildren`'s existence by being a second
   consumer, not the first; an interface with one implementation is speculative cost (virtual
   dispatch, boxing) with no present benefit. `IRandomAccessSequence` (§9) is the exception that
   proves the rule — it launched with two witnesses (`ArraySequence`, `DynamicArraySequence`) from
   day one, so it earned the interface immediately instead of waiting.
4. **Operations** is the mutable instance type (or a static engine, for algorithms that walk an
   externally-owned structure) that composes Representation and Topology via generic constraints —
   never the reverse; a representation should never be built *around* one algorithm.
5. **Never reuse another domain's Representation or Topology contracts as your own.** Replicate
   the *pattern*, not the *type* — a fold's node-list and a heap's backing array are different
   enough shapes that forcing a shared interface between them tends to produce a leaky common
   surface instead of a useful one. Composing another domain's already-public concrete
   *representation* (as `DynamicArraySequence` does with `DataStructures/DynamicArray`, §9.4) is a
   different thing and is fine — what's prohibited is forcing that domain's type to implement
   *your* interface.
6. **Place the file by axis first, then by the right second-class label for that axis** (§13). On
   the `DataStructures/` side that second label is the data structure's own name — a data
   structure's identity *is* its representation/topology, so `DataStructures/<Structure>/` is
   already correct. On the `Algorithms/` side, the top-level folder is **what the algorithm does**
   (`Algorithms/ShortestPaths/`, `Algorithms/Folding/`), never what data structure or topology tier
   it happens to need — that's real, but subordinate, information, expressed only as a nested
   subfolder, and only once a second tier of the same utility actually exists to justify one (§13.1
   restates this "don't build structure before it's earned" rule for topology tiers specifically).
7. **Not everything that composes a Representation and adds checks belongs in `Algorithms/`.**
   When an Operations type is hardwired to exactly one concrete Representation — no interface, no
   second implementation possible — it isn't decoupled from a data structure the way
   `ShortestPath`/`MergeSort`/`Reduce`/`Fold` are (each generic over an interface with genuine
   multiple implementations); it *is* the data structure, with its bounds-checking layer split out
   for the reasons in step 4. That pairing co-locates under `DataStructures/<Structure>/`
   (`Heap.cs` beside `HeapArray.cs`, `Stack.cs` beside the `DynamicArray.cs` it hardcodes) — never
   under `Algorithms/`. `Algorithms/<Utility>/` is for Operations that are not hardwired that way:
   a type generic over a capability interface with real substitutability, or an algorithm over
   plain inputs that has no Representation of this repo's own to belong to (§13.5).

## 6. Gate-compliance notes

This repo's Nomos code-standards gate (`standards.json` and `suppressions.json` at the repository
root, engine at `kevinmettias/code-standards`) shapes every file toward one public type per file,
small single-cluster classes, closed-set modeling over loose bool/enum flags, and composition over
inheritance. When the gate flags something, the fix comes first, because it is almost always real:
`ArrayGrowth` is one shared constant rather than three independent copies of the same tuning
numbers, `HashMap`'s `Set`/`TryRemove`/`Grow` are split into named helpers,
`HeapArrayIndex.RightChild` is `LeftChild + 1` rather than a bare literal, and `MergeSort`'s range
helpers take a `SortBounds` rather than five and six parameters (§11.3).

A finding that a fix would make worse is waived in `suppressions.json` with its reason (§13.4).
The library tiers' standing precedents:

- **`check-class-size`'s "two disjoint call-graph clusters" on a small container** whose methods
  share only the backing store. The metric discounts that store as plumbing and then splits the
  methods by which private helper they call — `Heap`'s insert path from its remove path,
  `CircularBuffer`'s front end from its back end — which is not a responsibility boundary. Add a
  real call edge first where one exists (a throwing member delegating to its `Try` sibling); waive
  only the clusters that remain.
- **`check-responsibility-extraction`'s shared-prefix finding** on a `Try` family — the BCL's
  attempt-returns-success convention, which the check's own table of conventional prefixes happens
  to omit (`Deque`, `DynamicArray`, `DisjointSetForest`) — and on an overload family that crosses
  one subject with axes the file keeps together on purpose (`BinarySearch`'s three searches, each
  over a witness or an array, with or without a comparer; `Manacher`'s `Compute*Radii`).
- **`check-function-cohesion` on a container's test** that performs one mutation and asserts
  several independent facets of it, or asserts between mutations because the count is the
  subject. The check's own doc calls the rule advisory.
- **`check-mutable-struct` on a hook or algebra that accumulates by value** (§12.4), and on the
  test fixtures that prove a walk hands that value back.

§17.9 records the notes the solution and harness tiers add.

## 7. Mirrors, self-hosting and stated preconditions

Three rules that every addition keeps:

- **The test tree mirrors the source tree exactly.** `DSAExperimentation.Tests/` repeats
  `DataStructures/` and `Algorithms/` path for path, so a type's test is found where its source is
  (§18.1). A test folder that drops or adds a path segment is a defect, not a style choice.
- **Library code uses this repository's own structure once one exists.** `ShortestPath`'s
  Dijkstra/A* frontier is `Heap<T,TOrder>`, ordered by `ByPriorityOrder`, not the BCL
  `PriorityQueue`; `KeyedDisjointSet` composes `HashMap`, not `Dictionary`. Two kinds of code are
  exempt: scratch state local to one call (§16.5), and baselines, which write the simple form by
  hand and use no library implementation (§17.5).
- **A contract states the strongest law its consumer relies on.** `IPathHeuristic` requires a
  *consistent* heuristic (`h(u) <= cost(u,v) + h(v)` on every edge), not merely an admissible one,
  because `ShortestPath.Traverse` settles a node on its first pop, and only consistency guarantees
  that pop already carries the true shortest distance. An admissible but inconsistent heuristic can
  settle a node at a non-optimal distance with nothing to catch it. `ShortestPath.cs` likewise
  states its non-negative-edge-weight precondition, which neither `Dijkstra` nor `AStar` checks.

## 8. Capability contracts: binding algorithms to what they need, not what they're given

An algorithm can be isolated from a *concrete* data structure, but not from the structural
properties it actually depends on. Precisely: an algorithm requires a set of operations `O` and a
set of laws `L` — semantic invariants *and* complexity guarantees — and any representation that
implements `O` while satisfying `L` is a valid model for that algorithm. The goal is binding to the
weakest sufficient requirement, not to a concrete type.

There are three kinds of independence here, in decreasing order of how safely they can be assumed:

- **Semantic independence** — how this repo works, and the safest kind.
  `IGraphTopology<TNode,TChildren>.GetChildren` doesn't care whether children come from
  `ListChildren`, `SparseArrayChildren`, or `GridChildren`; `IPathHeuristic.cs`'s own doc comment
  (requiring a *consistent* heuristic, not merely an admissible one — §7) is a stated semantic
  *law*.
- **Implementation independence** — often true, not universally guaranteed. The same `Reduce`/
  `Fold`/`Walk` code runs unchanged over every `IChildren` implementation in this repo. It's
  not a law, though — a future structure could need a genuinely different optimal algorithm per
  representation (dense vs. sparse matrix multiplication is the classic case elsewhere), so this
  independence is a frequent convenience, not something to assume by default.
- **Performance independence** — the dangerous one. `DataStructures/Heap`'s `Heap<T,TOrder>`
  push/pop is O(log n) *only because*
  [`HeapArray<T>`](DSAExperimentation/DataStructures/Heap/HeapArray.cs)'s `Get`/`Set`/`Swap` are O(1).
  A syntactic contract like `Get(index)` says nothing about that cost. Swap `HeapArray<T>`'s backing
  store for something with O(n) indexed access and `Heap`'s complexity claim silently degrades to
  O(n log n) — no compiler error, no test failure, since correctness tests still pass regardless of
  how long they take.

**The actionable rule** (extends §5's recipe): when an Operations-layer type's complexity guarantee
depends on a specific cost from its Representation layer, say so in that type's doc comment. The
type system can't enforce Big-O, but a written dependency is what stops a well-intentioned
representation swap from silently regressing performance instead of failing loudly.
[`Heap.cs`](DSAExperimentation/DataStructures/Heap/Heap.cs) and
[`HeapArray.cs`](DSAExperimentation/DataStructures/Heap/HeapArray.cs) carry exactly this cross-reference
as the worked example.

## 9. Worked example: `Algorithms/Searching/BinarySearch`

`BinarySearch` is bound to a Representation contract that the searching algorithms define for
themselves rather than one they are handed. Where `Heap` needed a Topology witness (`IHeapOrder`)
over an already-obvious Representation (a flat array), binary search needs the opposite: no
Topology witness at all (sortedness isn't a variant to choose between — see §9.1), but its own
Representation contract, because "the sequence being searched" is the one part of this problem
genuinely open to more than one physical layout.

| File | Axis | Why |
| --- | --- | --- |
| [`DataStructures/Sequence/IRandomAccessSequence.cs`](DSAExperimentation/DataStructures/Sequence/IRandomAccessSequence.cs) | Representation | An indexable view over an already-sorted sequence, generic over the element type alone — the search's only physical-layout requirement |
| [`ArraySequence.cs`](DSAExperimentation/DataStructures/Sequence/ArraySequence.cs), [`DynamicArraySequence.cs`](DSAExperimentation/DataStructures/Sequence/DynamicArraySequence.cs) | Representation | Two witnesses satisfying the O(1)-`Get` obligation above, for two different physical reasons; `OffsetSequence` (a window into an array) and `MatrixSequence` (a row-major matrix read as one run) are two more |
| [`Algorithms/Searching/BinarySearch.cs`](DSAExperimentation/Algorithms/Searching/BinarySearch.cs) | Operations | `Find`/`LowerBound`/`UpperBound` — the bisection loop; owns no Representation witness |

### 9.1 Why sortedness is a law, not a Topology witness

Sortedness is modeled as a class-doc-comment precondition, the same shape as `ShortestPath.cs`'s
non-negative-edge-weight law (§3.3, §8) — established once by the caller before `Find` is ever
called, never rechecked. The test for which shape a given requirement earns isn't "is it used more
than once," it's *does the algorithm actively re-derive it at every step, or does it lean on it once
and trust it from then on*. `IHeapOrder<T>.HasPriority` is re-derived: `Heap<T,TOrder>`'s sift walk
calls it on every comparison, and the result of that call decides what happens next — the witness
*is* the algorithm's control flow. `Find`'s comparisons never do anything analogous for sortedness:
`comparer.Compare` decides which half to search *assuming* the sequence is already ordered by that
same comparer, but nothing about that comparison verifies, re-derives, or depends differently on
whether the assumption holds anywhere upstream. Get it wrong and `Find` still runs to completion and
returns an answer — just a silently wrong one, with no failing build, exactly the failure mode
`ShortestPath.cs`'s own comment already names for a negative edge weight.

### 9.2 Why the comparer stays a runtime object, not a witness

`Find`'s comparer is a plain `IComparer<T>` method parameter — `HashMap<TKey,TValue>`/
`ContiguousGroupBuffer<TItem,TKey>`'s precedent (§4.1) — not a `static abstract` witness like
`IHeapOrder<T>`. The discriminator is not how often the comparer gets called: `HashMap.TryGetValue`
consults its comparer on every probe of a bucket's chain, exactly as often as `Find`'s
`comparer.Compare` runs on every bisection step, and `HashMap` is still correctly modeled as an open
runtime object. That `HashMap` calls the default comparer directly for a value-type key (§14.2)
changes how the call is dispatched, not which comparers are valid. The discriminator is whether the
space of valid choices is closed. `IHeapOrder<T>` is
closed to exactly two shapes this library enumerates itself (`MinHeapOrder`, `MaxHeapOrder`), with
`ByPriorityOrder` only ever a *projection* of that same binary choice onto a different field, never a
third fundamentally different ordering. `IComparer<T>` is not closed at all — any `T`, any total
order a caller wants, is a valid comparer, the same openness that already justifies `HashMap`'s and
`ContiguousGroupBuffer`'s choice for equality. `Find`'s default overload additionally constrains
`T : IComparable<T>` before delegating to `Comparer<T>.Default` — the same
compile-time-safe-default-over-runtime-only-default shape `HashMap`'s parameterless constructor
already uses for `EqualityComparer<TKey>.Default`.

### 9.3 Stating the O(1) assumption three times, not once

`HeapArray.cs`/`Heap.cs` (§8) state the O(1) assumption twice, because there's one concrete
Representation class making the claim true and one Operations class whose complexity depends on it.
`IRandomAccessSequence<T>` splits Representation into several implementations (see §9.4),
so the assumption is stated three times instead, each from a different angle so none of them repeat:
`IRandomAccessSequence<T>`'s own doc comment states the *obligation* every implementation is expected
to satisfy — new relative to `IChildren<TNode>`, whose doc comment never had to say this, since
no graph engine depends on `Get`'s cost (§2). Each witness's doc comment (`ArraySequence<T>`,
`DynamicArraySequence<T>`, `OffsetSequence<T>`, `MatrixSequence<T>`) states *why its own backing
store actually satisfies it*. `BinarySearch.cs`'s doc comment states the *consequence* for its own
complexity claim if some future implementation doesn't.

### 9.4 A second Representation, earned on day one

Unlike `HeapArray<T>` (§4 — one implementation, so no interface, per §5's recipe),
`IRandomAccessSequence<T>` launched with two implementations, `ArraySequence<T>` and
`DynamicArraySequence<T>` — the interface's existence was earned from the start rather than deferred
the way `IChildren` waited for `SparseArrayChildren` to justify it.

`DynamicArraySequence<T>` composes `DataStructures.DynamicArray.DynamicArray<T>` directly,
unmodified, and that is a distinct kind of cross-domain reuse. It isn't `ByPriorityOrder`
implementing `DataStructures.Heap`'s public `IHeapOrder<T>` as a client (§4 — one domain implementing
another's interface without ever touching its Representation). It isn't `Stack`/`Queue` composing
`DynamicArray<T>`/`Deque<T>` either (§4.1 — one container restricting another container's access).
It is a contract owned by the search algorithms adapting another structure's own Representation
*type* — not an interface belonging to it — into a witness for a contract the search side defines
for itself. That's consistent with §5 step 5: what's prohibited is reusing another domain's
*contract* (forcing `DynamicArray<T>` itself to implement `IRandomAccessSequence<T>`, coupling it to a
domain it doesn't belong to), not consuming its already-public concrete type as one of several
witnesses for a contract the consuming domain wrote itself. `DynamicArray.cs` stays completely
unaware `IRandomAccessSequence<T>` exists; `DynamicArraySequence<T>` absorbs all of the coupling,
one-directionally, with zero changes to `DynamicArray.cs` — exactly the demonstration §8's
"implementation independence" argument calls for, using a structure this repo already had for an
unrelated reason instead of a contrived second example.

### 9.5 Array overloads: the witness picked for the caller

Every search, and `MergeSort.Sort` (§11), also takes a plain `T[]` and wraps it in
`ArraySequence`/`ArrayIndexedSequence` itself. Without them a caller spells out
`MergeSort.Sort<T, ArrayIndexedSequence<T>>(new ArrayIndexedSequence<T>(items))`, because a sequence
type argument gives inference nothing to read `T` from; eighty-eight solution call sites once did,
and more than forty solutions built an `ArraySequence` only to search it. This is the move `Reduce`'s
"in the topology's own child order" overloads already make with `NaturalChildOrder`: one axis
defaults to the witness the caller would have written anyway. The loops still bind to
`IRandomAccessSequence`/`IIndexedSequence` alone and never see the array, so the instantiation that
runs is the one the long spelling produced. What keeps a call from reaching the overload it did not
mean is §17.4's argument, not arity: a `T[]` fails `TSequence`'s `struct` constraint, and no sequence
witness converts to a `T[]`.

### 9.6 Binary search on the answer is a rule over an interval, not a sequence

Searching for the smallest (or largest) value a feasibility rule accepts — a ship capacity, an
eating speed, a day — is `MonotonePredicateSearch.FirstTrue`/`LastTrue` over an integer interval
stated by two numbers, generic over `IBinaryInteger`, so a range past `int` is searched in `long`.
Spelling it as an `IRandomAccessSequence<bool>` whose `Get(i)` asks the rule about `floor + i`,
handed to `LowerBound(sequence, true)`, borrows `BinarySearch`'s laws (a sorted sequence, a
comparer) for a problem that has neither, and borrows an `int` index too: it truncates or throws on
ranges LeetCode's own limits allow.

The rule is `IMonotonePredicate<Integer>`. Its space is open — §12.1's successor relation, not a
set this library enumerates — so §5's second row applies; §12.4's question then makes
`IsSatisfiedBy` an instance member on a struct type parameter, because the arrays and budgets a rule
reads never arrive through its signature. `IGridCellFilter` is the precedent. Monotonicity is a
precondition law, like sortedness (§9.1). It stays a separate class rather than a `BinarySearch`
method, and `LowerBound` is not re-expressed through it: for a reference-type element the rule would
be shared generic code, and every probe would pay the runtime lookup §12.4 measured.

## 10. Worked example: `DataStructures/DisjointSet`

`DisjointSet` is the equivalence-class structure, and it shows that a choice which *looks* exactly
like Heap's Min-vs-Max axis can still fail to earn a Topology witness. Where `Heap` needed a
Topology witness over an obvious Representation, and `BinarySearch` needed a new Representation
over an already-settled Topology, Union-Find needs neither: the representation is a concrete
two-array forest, and the one axis that looks variable turns out to be a Complexity law instead.

| File | Axis | Why |
| --- | --- | --- |
| [`DataStructures/DisjointSet/DisjointSetForest.cs`](DSAExperimentation/DataStructures/DisjointSet/DisjointSetForest.cs) | Representation | Parallel `parent`/`rank` `int[]` fields, fixed-size at construction — concrete, not an interface, since exactly one physical layout exists (§5 step 3) |
| [`DataStructures/DisjointSet/DisjointSet.cs`](DSAExperimentation/DataStructures/DisjointSet/DisjointSet.cs) | Operations | `Find`/`Union`/`IsConnected`/`MergeInto` — path compression and union-by-rank, both hardcoded, not swappable |

### 10.1 Why the linking policy is a Complexity law, not a Topology witness

Union-by-rank vs. union-by-size vs. naive/arbitrary linking looks, on the surface, exactly like
Heap's Min-vs-Max choice: a decision re-made on every call (`Union`, here; `HasPriority`'s
comparison, there) that branches control flow. §9.1 already warns that call-frequency isn't the
discriminator — but the sharper test this example adds is: **does the choice change the
observable output, or only the cost of producing it?** `MinHeapOrder` vs. `MaxHeapOrder` changes
*which value `TryPop` returns* — a real, caller-visible semantic difference. No linking policy ever
changes *which partition `Find`/`IsConnected` report* for a given sequence of calls — every policy,
including a deliberately bad one, computes the identical equivalence classes. Only tree height,
and therefore amortized cost, differs: O(n) worst case with neither optimization, O(log n)
amortized with only one, O(α(n)) — effectively constant — with both path compression and
rank-guided linking applied together. That is §5's Complexity-law row verbatim ("`Get` assumed
O(1); a slower representation degrades big-O with no compiler error"), so the policy is hardcoded,
unconditionally, inside `Union` rather than exposed as a generic witness. Path compression itself
isn't even a law in this sense — no external caller could violate it, since nothing outside
`DisjointSet` ever touches `SetParent` — it's pure Operations-internal mechanics, the same status
`Heap`'s `SiftUp`/`SiftDown` already have.

`MergeInto(id, target)` does not reopen this argument. Its contract names the representative — the
root `Find(target)` returned before the call survives — where `Union`'s never does, so it is a
second operation with its own output, not a fourth linking policy: the test above asks whether a
choice *inside `Union`* changes what a caller can observe, and it still cannot. The direction is a
runtime argument on each call, not a closed set chosen by type, so it needs no witness either. It
exists for callers that read the representative as an answer — "the nearest obstacle still active
at or before x" in LC 3161. It never consults rank, so the O(α(n)) bound holds only for a forest
`Union` alone has joined; with the two mixed, path compression alone gives O(log n) amortized, and
the doc comment says so.

### 10.2 Why the partition itself needs no Topology witness at all

Separately from the linking policy, the equivalence-class invariant that `Find`/`Union` maintain —
reflexive, symmetric, transitive by construction — needs no witness either, for the same reason
`HashMap`'s "at most one value per key" doesn't (§4.1): there is no `MinDisjointSet` vs.
`MaxDisjointSet`, no second variant this library could enumerate. A correct `Find`/`Union`
implementation *cannot* produce a non-equivalence-relation result, and no caller input can make it
try — a stronger guarantee than `BinarySearch`'s sortedness precondition (§9.1), which a caller
*can* violate by passing an unsorted sequence. `DisjointSet` therefore has an Operations layer and
a Representation layer, but no Topology axis of its own at all — the same shape `HashMap`/`Set`
already have in §4.1's table.

### 10.3 A non-generic type named like its folder, and the C# wrinkle that comes with it

Every other multi-file container (`Heap<T,TOrder>`, `Stack<T>`, `Deque<T>`,
`HashMap<TKey,TValue>`) is generic, so a bare reference to its own name is never ambiguous: a
type-argument list (`Heap<int, MinHeapOrder<int>>`) is something no namespace can have, so the
compiler always resolves it as the type. `DisjointSet` is dense-int-indexed by design (§10.4), not
generic over an element type, so a bare `new DisjointSet(3)` reference from
`DSAExperimentation.Tests.DataStructures.DisjointSet` has no such disambiguator — namespace-member
lookup finds the test file's own enclosing namespace (a nested member of
`DSAExperimentation.Tests.DataStructures`) before any `using` directive is ever consulted, and fails
with "`DisjointSet` is a namespace but is used like a type." A same-named `using` alias doesn't
help, since that lookup order is unaffected by which names a `using` brings in; only a
*differently*-named alias (`DisjointSetOperations` in `DisjointSetTests.cs` and in
`KeyedDisjointSet.cs`) resolves it. This is a consequence of keeping the Representation
dense-int-indexed rather than generic, not a naming mistake to fix. §15.3 meets it again, and the
LeetCode tier avoids it by construction with a `Solution` suffix (§17.3).

### 10.4 Dense-int-indexed, with a keyed wrapper beside it

`DisjointSetForest` is fixed-size at construction with no growth path — a real departure from
every other container Representation (`HeapArray`/`DynamicArray`/`HashMap` all start empty
and grow), because a disjoint-set forest's universe of ids is conventionally known upfront, the
same way CLRS's own "disjoint-set forest" is presented. It is not generic over an element type `T`
either: ids are plain `int`s in `[0, Count)`, assigned by the caller before any `Find`/`Union`
call — an unchecked precondition the same shape as `BinarySearch`'s sortedness (§9.1).

A caller whose elements are not dense ints uses `DataStructures/KeyedDisjointSet/KeyedDisjointSet<TKey>`,
which *composes* `DisjointSet` with a `HashMap<TKey,int>` id map rather than generalizing the
forest — composition over redesign, the same relationship `Stack`/`Queue` have with
`DynamicArray`/`Deque` (§4.1), and the same reason it has a folder of its own beside the type it
composes. `Algorithms/MinimumSpanningTrees/MinimumSpanningTree.Kruskal` is its consumer.

## 11. Worked example: `Algorithms/Sorting/MergeSort`

`MergeSort` shows that "the same capability shape" is not grounds to reuse another domain's
Representation contract. Both it and `BinarySearch` need indexed access to a sequence; only one of
them needs to write it back.

| File | Axis | Why |
| --- | --- | --- |
| [`DataStructures/Sequence/IIndexedSequence.cs`](DSAExperimentation/DataStructures/Sequence/IIndexedSequence.cs) | Representation | Indexed get/set view, generic over the element type alone — earns the interface on day one via two witnesses, same §5-step-3 exception `IRandomAccessSequence<T>` used |
| [`DataStructures/Sequence/ArrayIndexedSequence.cs`](DSAExperimentation/DataStructures/Sequence/ArrayIndexedSequence.cs), [`DynamicArrayIndexedSequence.cs`](DSAExperimentation/DataStructures/Sequence/DynamicArrayIndexedSequence.cs) | Representation | Two witnesses satisfying the doubled O(1) obligation, for the same two physical reasons `ArraySequence`/`DynamicArraySequence` already do |
| [`Algorithms/Sorting/SortBounds.cs`](DSAExperimentation/Algorithms/Sorting/SortBounds.cs) | Operations-internal state | Groups the `[Low, High]` range being sorted — an immutable value bundle, not a mutable shared cursor, and not a Representation of the sequence (§13.6) |
| [`Algorithms/Sorting/MergeSort.cs`](DSAExperimentation/Algorithms/Sorting/MergeSort.cs) | Operations | `Sort` — top-down recursive split/merge; `SortedCopy` runs it on a copy; owns neither witness |

### 11.1 Domain separation is the reason for a new contract, not read/write alone

`IIndexedSequence<T>` exists because sorting is a distinct domain from searching, per §5's
domain-reuse rule (never reuse another domain's Representation or Topology contracts as your own) —
that is the primary reason, and it holds regardless of operation-set overlap. Stating "it needs
`Set` and `IRandomAccessSequence<T>` doesn't have one" as the *sole* justification would wrongly
imply that a future sorting algorithm needing only `Get` could then reuse the search side's
interface; §5's domain-reuse rule fires on domain ownership, not on whether the method lists happen to
differ. The read/write mismatch here is real and reinforcing — `IRandomAccessSequence<T>` could
not satisfy `Set` even if reuse were otherwise permitted — but it is secondary to the domain
argument, not a replacement for it.

### 11.2 A law a read-only contract never had to state

`IRandomAccessSequence<T>`'s only obligation is that `Get` be O(1) (§9.3). `IIndexedSequence<T>`
doubles that (`Get` *and* `Set`), and adds one `Get`-only contracts never needed: because a
`TSequence` witness is a `struct` passed **by value** into every recursive call `MergeSort` makes,
`Set`'s mutation is only visible across those copies if the copied struct still aliases the same
backing store — true for both witnesses here (`T[]` and `DynamicArray<T>` are reference types),
but not guaranteed by the contract itself. A hypothetical future witness wrapping a value-type
field directly would compile fine and silently drop every `Set` made through a copy — a
correctness law, not a performance one, and one only a read/write contract can even have.

### 11.3 No `SearchRange`-style mutable cursor, and no witness for the comparer

`SortBounds` is an immutable `readonly record struct`, not a mutable struct narrowed in place by
`ref` the way `BinarySearch`'s `SearchRange` is. The difference is structural, not stylistic:
`BinarySearch` narrows *one shared* range across loop iterations; `MergeSort`'s recursion splits
into *two independent, non-shared* sub-ranges per call, so there is nothing for a shared,
in-place-mutated cursor to buy — inventing one anyway would be pattern-mimicry without the
underlying need. `SortBounds` exists purely as this repo's standard "group related parameters
into a type" recipe (§6), reducing `SortRange`/`Merge`/`MergeRunsIntoBuffer`/
`CopyBufferIntoSequence` back under the parameter-count gate's limit.

The comparer stays a plain `IComparer<T>` parameter, same §9.2 reasoning as `BinarySearch`'s: any
`T`, any total order, is a valid comparer, an open-ended space no witness could usefully close
over. There is no precondition law analogous to `BinarySearch`'s sortedness, either — sortedness
is `MergeSort`'s postcondition, not an assumed input. `Sort` takes a plain `T[]` as well, for §9.5's
reason; an array meets §11.2's aliasing law because it is a reference type. `SortedCopy` copies a span
into a new array and sorts that, for a caller whose input is not its own to reorder: a solution's
input is a benchmark's workload, reused by every invocation. It adds no axis, only the copy that
thirty solutions' composed arms wrote by hand, six of them as a helper of their own.

### 11.4 Element algebras: one chain across four structures, and why §11.1 does not split it

`SegmentTree`, `FenwickTree`, `RangeFenwickTree` and `LazySegmentTree` each need an algebra over
their elements, and they share one chain in `Domain/Mathematics/Algebra/` — `IMonoid`
(monoid) ← `IAbelianGroup` (+ `Invert`) ← `IIntegerModule` (+ `Scale`) — with `Addition` at the
deepest position its arithmetic reaches, plus `Minimum`, `Maximum` and `BitwiseXor`. `IRangeUpdateOperation` refines `IMonoid` but stays in
`LazySegmentTree/`.

**Why §5 step 5 does not fire.** Step 5 forbids reusing another domain's *Representation or
Topology* contract, because a fold's node list and a heap's array are different enough shapes that a
shared interface leaks. These contracts are neither: they are laws on the element type — identity,
associativity, inverse, scaling — the way `INumber<T>` is, which every `Addition` already
depended on. No structure reuses another structure's contract; all four consume a neutral one,
the same arrangement as `ByPriorityOrder` implementing `Heap`'s `IHeapOrder` for `ShortestPath`.
One contract per structure would cost what it once did: byte-identical `Addition`s per
structure, and an adapter re-presenting one contract's `Identity`/`Combine` under another's name so
two trees could share a build recursion.

**Why this is not §13.6's case.** `Sequence/` also gathers two contracts into one folder and still
keeps `IRandomAccessSequence` and `IIndexedSequence` apart, citing §11.1. The difference is what the
contracts describe. Those two are capability levels of a *Representation*, each owned by the
algorithm family that consumes it, and the write-capable one carries a law (§11.2's aliasing) the
read-only one never states. Here the overlapping members are identical in name, signature and law,
so keeping them apart would buy nothing but an adapter.

**The rule that keeps the chain honest.** A member only one structure needs never joins it; it goes in
a structure-local refinement. `IRangeUpdateOperation`'s update tags are the example: lazy
propagation is `LazySegmentTree`'s business, so `SegmentTree` and `FenwickTree` never see them.
The structures' own constraints say which link each needs — a Fenwick tree over `Minimum` is a
compile error, because it needs `Invert` — and the reason is stated once, on `IAbelianGroup`.
Dispatch is unaffected: every operation is a static-abstract witness on a struct type parameter.

## 12. Worked example: `Algorithms/Traversal/DepthFirst/DepthFirstSearch`

`DepthFirstSearch` is an algorithm with no Representation axis at all, and it shows that a
capability which looks exactly like a Graph `Topology` contract can still resolve to a plain runtime
object once it leaves Graph's domain. It uses none of the graph framework's contracts: it is generic
over nothing but a bare successor function.

| File | Axis | Why |
| --- | --- | --- |
| [`Algorithms/Traversal/DepthFirst/DepthFirstSearch.cs`](DSAExperimentation/Algorithms/Traversal/DepthFirst/DepthFirstSearch.cs) | Operations | `Traverse` — the only file this algorithm needs; there is no Representation to write |

### 12.1 Why the successor relation is a runtime object, not a witness

`successors: Func<TNode, IEnumerable<TNode>>` is called on every visit and its result drives what
gets pushed next — superficially an exact match for §5's table row 1 ("actively re-derived every
step, changes control flow"), the same shape `IGraphTopology<TNode,TChildren>.GetChildren` has
inside Graph. Reading the table that naively would be a mistake. The actual discriminator, per
§9.2, is not call frequency but **whether the space of valid choices is closed**:
`IGraphTopology`/`IDagTopology`/`ITreeTopology` is closed because Graph itself chose to formalize
exactly three adjacency tiers, each with a differentiated algorithm (`CheckedFold`/`DagFold`/
`TreeFold`, `TrackedVisitGuard`/`UnguardedVisit`). A standalone successor relation has no such
enumerable set of "kinds" to close over — a chess-move generator, an infinite lattice, a cyclic
closure are arbitrary caller logic, not named library variants — so it lands in the same open
bucket as `IComparer<T>`, row 2, not row 1. There is also nothing to close a witness over even if
one were wanted: the only tiered hierarchy for "successor relation kinds" anywhere in this
codebase is Graph's own, and reusing it is exactly what §5's domain-reuse rule forbids.

### 12.2 No Representation axis at all — the mirror image of `DynamicArray`

A `Func<TNode, IEnumerable<TNode>>` has no alternate physical layout to abstract over the way a
sequence does — it is already the atomic, opaque capability, with nothing for a second
implementation to vary. The explicit stack and visited set `Traverse` allocates are
Operations-internal scratch state, the same bucket as Graph's `TrackedVisitGuard` or
`BinarySearch`'s `SearchRange`, not a Representation of the input. `DynamicArray` is Representation
with no Topology axis (§4.1); `DepthFirstSearch` is the mirror image — Operations plus an
open-runtime-object law, with no Representation axis at all. Because it never gets a promise
stronger than "arbitrary function," it also never earns Graph's `UnguardedVisit`/
`TrackedVisitGuard` tiering — the visited-set guard here is unconditional, permanently, with no
faster tier possible.

### 12.3 `notnull`, deliberately not `class` — and the precondition this algorithm can't check

`Traverse<TNode>` constrains `TNode : notnull` and nothing more. Graph's own node-generic code
constrains `TNode : class`, but that is a Graph-domain assumption about reference identity flowing
through `IChildren` — not an inherent property of "successor-shaped" algorithms. An implicit graph
generated on demand — a chess position — is naturally value-typed, and `DepthFirstSearchTests`
proves it directly with a `readonly record struct Position`. `Traverse` also carries a precondition
law: `successors` must produce a *finite* reachable set from `start`. An infinite one — with no
cycle for the visited set to catch — makes `Traverse` run forever, with no compiler or runtime error
to catch it, the same unchecked-but-real shape as `BinarySearch`'s sortedness (§9.1) or
`ShortestPath`'s non-negative-edge-weight assumption (§7).

### 12.4 A third answer: instance members on a struct type parameter

§9.2 and §12.1 choose between a static-abstract witness (a closed set of choices, fixed by type) and
a runtime object (an open space, passed as a value). Hooks and fold algebras need a third answer.
Their *kind* is closed and chosen at the call site by type — a `RankHooks` is one type, a
`CoinPointsAlgebra` another — but an instance of the kind needs values only the call knows: the
rank to count down from, the coins each node holds, the list a walk collects into. A
static-abstract member has no channel to such a value, so every workaround — a static `Prepare`
step, an `AsyncLocal` slot, a marker type per test to keep a static log apart — smuggles the value
past the contract.

The rule is one question: **does every runtime value the contract needs arrive through its member
signatures?** If it does, the contract stays a static-abstract witness — `IReduceAlgebra`,
`ITopDownHooks`, `IPathHeuristic`, `IChildOrder`, `IHeapOrder`, the element algebras of §11.4. If it
does not, its members are instance members on a `struct` type parameter — `IFoldAlgebra`,
`IDepthFirstHooks`, `IBreadthFirstHooks`, `ILevelGroupedHooks`, `IInOrderHooks`, `IGridCellFilter`,
`IMonotonePredicate` and `IVisitGuard`. The engine takes the struct as a value; each kind is still
its own instantiation, and its members are constrained calls, so nothing boxes and nothing
dispatches through an interface. Three consequences follow:

- **Every member is required.** A default interface member reached through a struct type parameter
  boxes the struct, so an empty body is how a hook ignores an event.
- **A walk returns the hook value it finished with.** A hook that holds a reference — a list — can
  be read through it afterwards; one that holds counters by value is read from the returned value.
  Those hooks are mutable structs on purpose, and each is waived against `check-mutable-struct`
  with the reason it must be one.
- **An algebra or hook is never reached through `in` or a readonly field.** The defensive copy
  drops what it accumulates, which is why `ZipFoldAlgebra` is a plain struct, not a readonly one.

`AmbientStateTests` (§18.5) holds the line: no `AsyncLocal`, `ThreadLocal` or `[ThreadStatic]` slot
in the library or the solutions.

**What "zero cost" means here, measured.** Every node type in this library is a reference type, so
the engines run as shared generic code, and a call into a struct type argument that is itself
generic over the node type (`SizeAlgebra<TNode>`, a fold's memo policy) goes through a runtime
lookup the JIT does not inline. A type argument that is exact — `RoomWaysAlgebra`,
`BinaryTreeTopology<int>`, every in-order hook at `TValue = int` — gets a direct call. This is why
the tree tier keeps a fold recursion of its own (`RecursiveFoldEvaluation`) rather than sharing the
DAG and graph tiers' `FoldRecursion`: sharing it cost four such lookups per node, 40% on
`FoldTierBenchmarks.TreeTier`. Measure a new axis before calling it free.

## 13. Physical layout

The three axes (§2) are the physical split. Every file in `DSAExperimentation/` lives under
`DataStructures/` or `Algorithms/`, and `DSAExperimentation.Tests/` mirrors both (§7). The tiers
above the library — the solutions and the harnesses — live in projects of their own (§17).

- **`DataStructures/` is organized by structure.** A data structure's identity is its
  Representation and Topology, so its folder is its own name (§5 step 6, §13.6). It also holds the
  structure's Operations type when that type is hardwired to the one Representation (§5 step 7,
  §13.5).
- **`Algorithms/` is organized by utility** — what the algorithm does (`ShortestPaths/`,
  `Folding/`), never the structure or the topology tier it needs (§5 step 6). It holds the
  Operations that are not hardwired to one of this repo's Representations (§13.5), nested by tier
  only once a second tier exists (§13.1).

The seven reorgs that produced this layout are recorded in
[ARCHITECTURE-HISTORY.md](ARCHITECTURE-HISTORY.md) under §13.

### 13.1 Namespace mapping rule and the tiering rule

Namespaces mirror the physical path throughout: insert `DataStructures.` or `Algorithms.`
immediately after `DSAExperimentation.`, then follow the rest of the path exactly.

Within `Algorithms/`, a second rule governs how deep a topology/strategy tier gets to nest:
**when two or more tiered implementations of the same utility exist, nest them under folders that
mirror the actual refinement relationship** (`Folding/Dags/Trees/TreeFold.cs` nests under `Dags/`
because `ITreeTopology` refines `IDagTopology`, per §3.2's chain; `ShortestPaths/Grids/` and
`ShortestPaths/Hamming/` nest as structure-specific tiers beside the general edge-weighted one).
**When only one implementation exists, the file stays flat** — the same "don't build
structure before a second case earns it" spirit §5 step 3 already applies to Representation. This
is why `Metrics/TreeMetrics.cs`, `Ancestry/LowestCommonAncestor.cs`,
`Connectivity/ConnectedComponents.cs`, and `Paths/AllRootToLeafPaths.cs` stay flat even though each
is constrained to a specific topology tier — that constraint lives in the file's own doc
comment/generic bound, not a folder, until a second tier actually shows up.

### 13.2 Two axis-classification judgment calls

- **`ByPriorityOrder`** implements `Heap`'s `IHeapOrder<T>` (§4) — the same contract
  `MinHeapOrder`/`MaxHeapOrder` implement, classified as Topology. For consistency with its own
  contract's classification, it lives at `DataStructures/Graph/ShortestPaths/`, not alongside
  `ShortestPath.cs` in `Algorithms/` — even though `ShortestPath.cs` is its only consumer. Locality
  is real but secondary to axis-purity here, since axis-purity is what the layout exists to show.
- **`IVisitGuard`/`UnguardedVisit`/`TrackedVisitGuard`** superficially resemble a closed-set
  Topology witness — the same two-variant, struct-witness shape as `IHeapOrder`, though its member is
  an instance method because the graph guard carries its visited set — but the actual
  discriminator is *what the closed choice is about*, not its shape (§9.1, §12.1): `IHeapOrder`
  decides a relation on stored data, while `IVisitGuard` decides how the *algorithm executes*
  (whether to track revisits) — the same execution-strategy axis as `IFoldEvaluationStrategy`/
  `IReduceOrderStrategy`, uncontroversially Operations. This tier lives at `Algorithms/Walking/`,
  alongside `BreadthFirstWalk`/`DepthFirstWalk`/`TopDownWalk` — a utility folder, not a topology or
  domain one, exactly where §13's utility rule puts it.

### 13.3 Where everything lives now

The tables name every folder, and the types a reader comes looking for; a folder's remaining files
are its own helpers.

**`DataStructures/`** is organized by structure. Every Operations type hardwired to one of these
Representations (§5 step 7, §13.5) co-locates with it:

| Folder | Holds |
| --- | --- |
| (root) | `AlgorithmConstants`, `ArrayGrowth`, `RangeBounds` — constants and checks read by more than one structure, at the lowest tier that reads them (§17.6a) |
| `AhoCorasick/` | `AhoCorasick`, `AhoCorasickNode`, `AhoCorasickMatch` — witness-less, for §13.8's reasons |
| `Alphabets/` | `IAlphabet`, `LetterRun`, `LetterTable`, `ILetterSet`, `LetterSet` — an alphabet as an index scheme, each letter in a dense slot, with presets as struct witnesses; its two layouts, a run of character codes and letters spelled out in order; and a named subset of an alphabet's letters as a mask over its slots (§19.6). The presets are Domain's |
| `Buffers/` | `ContiguousGroupBuffer`, `ContiguousGroupBufferStorage` (§14.1) |
| `CircularBuffer/` | `CircularBuffer`, `IBufferCapacity`, `UnboundedCapacity`, `BoundedCapacity` — the wraparound structure the queue family configures, with its capacity a witness (§20) |
| `CountedBitTrie/` | `CountedBitTrie` — a bit trie over a multiset, whose values can be removed: every node carries a count, and a walk reads the counts to tell a live branch from a dead one. It stands beside `BitTrie` rather than inside it, so `BitTrie`'s insert-only users pay for no counts, and it has no `ITreeTopology` witness for both of §13.9's reasons: its nodes are slots in shared arrays, and after a removal it still holds paths whose values are gone |
| `CursorSequence/` | `CursorSequence` — a sequence edited and read at a cursor between two `DynamicArray`s, the composition `Stack` makes with a cursor in place of LIFO (§4.1). LC 2296's text editor uses it; LC 1472's browser history declined it, measured, because its jumps of k pages cost O(k) transfers here against an index's O(1) |
| `Deque/` | `Deque`, `BoundedDeque` — `CircularBuffer` unbounded and bounded (§20) |
| `DisjointSet/` | `DisjointSetForest`, `DisjointSet` (§10) |
| `DoublyLinkedList/` | `DoublyLinkedList`, `DoublyLinkedListNode` (§13.7) |
| `DynamicArray/` | `DynamicArrayStorage`, `DynamicArray` (§14.3) |
| `FenwickTree/` | `FenwickArray`, `FenwickTree` |
| `Graph/Adjacency/` | `AdjacencyNode`, `AdjacencyTopology`, `WeightedAdjacencyNode`, `WeightedAdjacencyTopology` — the general-graph sibling of `RootedTreeNode`/`RootedTreeTopology`, graph tier only: an adjacency list promises no acyclicity, so a DAG or tree witness over it is written by the caller who can vouch for one. A LeetCode problem whose node is exactly this shape uses it through a per-file `using` alias that keeps the problem's own name (`using CourseNode = …AdjacencyNode;`), and the file that wires the edges says what an edge means there, since that is the caller's reading of the list, not a property of the node |
| `Graph/Contracts/Ordering/` | `IChildren`, `IEdges`, `IChildOrder` and their witnesses (§3.1) |
| `Graph/Contracts/Topologies/` | `IGraphTopology`, `IEdgeTopology`, `EdgeTopologyAsGraphTopology` (§3.2) |
| `Graph/Engines/Dags/` | `IDagTopology` (§3.2) |
| `Graph/Engines/Dags/Trees/` | `ITreeTopology`; the binary tree (`BinaryTreeNode`, `BinaryTreeChildren`, `BinaryTreeTopology`, `ChildSide`, `BinarySearchTree`, `InOrderTraversal`, `IInOrderHooks`, `FindClosest`); the bit trie with its real witness (`BitTrie*`, §13.9); the rooted tree (`RootedTreeNode`, `RootedTreeTopology`, `ParentArrayTree`, `EdgeListTree`, `PreOrderTour`, `PreOrderLowestCommonAncestor`); the quad tree (`QuadTreeNode`, `QuadTreeGrid`). `EdgeListTree` roots LeetCode's undirected edge list with a breadth-first walk, returning the parent array `ParentArrayTree` reads and, for each node, the index of the edge that joins it to its parent - the one thing a parent array cannot recover. `PreOrderTour` lays a `RootedTreeNode` tree out in pre-order, so every subtree is one contiguous range of positions; `PreOrderLowestCommonAncestor` reads ancestors off that layout with one range minimum, O(log n) per query after O(n) preparation. It is hardwired to the tour, so it lives here rather than beside `Algorithms/Ancestry/LowestCommonAncestor`, which is generic over `ITreeTopology` (§13.5) |
| `Graph/Engines/Dags/Trees/Tries/` | The one trie (§13.8): `IPrefixMap`, the map's contract, and `IPrefixSet`, the set's, which the trie meets through `PrefixSet`, its view of a map to `bool`; `Trie<TStore, TValue>`, its key walk written once, and `TrieNode`; and the child store each configuration injects, `ITrieChildStore`, or `IOrderedTrieChildStore` for a store whose children have an order. `AlphabetSlots` (with `AlphabetSlotsEnumerator`) keeps a node's children in slots over any `IAlphabet` of up to 64 letters, in letter order, so its trie has a real witness (`TrieTopology`, `TrieChildren`); `HashedChildren` takes any `char` and has no order, so its trie has none. `ITrieLayout`, `DenseTrieLayout`, `PackedTrieLayout` — where the slot store keeps its children, a type argument each trie names |
| `Graph/Grids/` | `Grid`, `GridNode`, `GridChildren`, `GridSize`, `GridNeighbors`, `IGridCellFilter`, `GridTopology`, `IGridNodeSource` with `NodePerCell` and `NodePerStep`, and the weighted `WeightedGrid*` (§17.6). The steps they walk by, `GridStep` and `GridDirections`, are facts about the square lattice, in `Domain/Mathematics/Lattices`; a `Grid` carries the table its cells are joined by (`Steps`, the orthogonal four unless it is built with others, such as the octilinear eight LC 1091 moves by), and `GridChildren` reads which of them lead somewhere open into a mask once per node, so its `Count` is a population count and `Get` a set bit. `GridSize` is a board's extent and its one bounds rule, the bounded rectangle's (`Domain/Mathematics/Surfaces`); a walk's `IGridNodeSource` says whether a cell's node is the board's one node for it or a new one per step; `GridNeighbors` is a cell's on-board neighbours in a direction table's order, filtered by an `IGridCellFilter`, for a `DepthFirstSearch.Traverse` successor whose "open" is decided by the problem's cells rather than a fixed map like `Grid`'s |
| `Graph/Hamming/` | `HammingNode`, `HammingTopology`, `HammingGraph`, `HammingSearch`, `MutationStart`, `MutationTarget` (§17.6); the letters a mutation writes are an `IAlphabet` type argument (§19.6) |
| `Graph/ShortestPaths/` | `ByPriorityOrder` (§13.2) |
| `Graph/States/` | `IStateSpace`, `IStateFilter`, `EveryState`, `StateNode`, `StateTopology`, `StateGraph`. A state space numbers its states and writes a state's successors into a buffer; `StateGraph` makes the explicit graph of the states a filter keeps, which a breadth-first walk then searches. The spaces are Domain models' to state: a combination lock's turns (`LockTurns`), a sliding puzzle's slides (`PuzzleSlides`) |
| `HashMap/` | `HashMapStorage`, `HashMapEntry`, `HashMap` (§14.2) |
| `Heap/` | `IHeapOrder`, `MinHeapOrder`, `MaxHeapOrder`, `HeapArrayIndex`, `HeapArray`, `Heap` (§4) |
| `IntervalSet/` | `IntervalSet` — the one sanctioned upward reference, to `Algorithms/Searching` (§17.6a) |
| `KeyedDisjointSet/` | `KeyedDisjointSet` — composes `DisjointSet` and `HashMap` (§10.4) |
| `LazySegmentTree/` | `LazySegmentTree`, `IRangeUpdateOperation` and its witnesses (§11.4) |
| `MonotonicDeque/` | `IWindowOrder`, `MaxWindowOrder`, `MinWindowOrder`, `MonotonicDeque` — composes the unbounded `CircularBuffer` (§19.1, §20); `MinMaxWindow` — one `MonotonicDeque` per order (§19.1) |
| `PrefixSums/` | `PrefixSums`, `MatrixPrefixSums` — consume `Domain/Mathematics/Algebra`'s `IAbelianGroup` (§19.2) |
| `Queue/` | `Queue`, `BoundedQueue` — `CircularBuffer` unbounded and bounded, restricted to FIFO's ends (§20) |
| `RangeFenwickTree/` | `RangeFenwickTree` |
| `RollingHash/` | `RollingHash`, `RollingHashLane`, `RollingHashValue` |
| `SegmentTree/` | `SegmentTree` and its array, build, index and range helpers |
| `Sequence/` | `IRandomAccessSequence`, `ArraySequence`, `DynamicArraySequence`, `OffsetSequence`, `MatrixSequence`, `IIndexedSequence`, `ArrayIndexedSequence`, `DynamicArrayIndexedSequence` (§9, §11, §13.6) |
| `Set/` | `Set` — composes `HashMap<T,bool>` (§4.1) |
| `SinglyLinkedList/` | `SinglyLinkedListNode`, `RandomLinkedListNode`, `CycleDetection` (§13.7) |
| `Stack/` | `Stack` — composes `DynamicArray` (§4.1) |
| `SuffixArray/`, `SuffixAutomaton/`, `SuffixTree/` | `SuffixArray`; `SuffixAutomaton`, `SuffixAutomatonState`; `SuffixTree`, `SuffixTreeNode` — witness-less, for §13.8's reasons |

**`Algorithms/`** is organized by utility, and holds the Operations not hardwired to one of this
repo's Representations (§13.5). `Searching` and `Sorting` have no topology axis (§4.1/§10.2) but
still stay under `Algorithms/`, since `IRandomAccessSequence`/`IIndexedSequence` each have more than
one real implementation — the distinguishing question is substitutability, not the topology axis
specifically:

| Utility | Files | Topology/strategy tier |
| --- | --- | --- |
| `Ancestry/` | `LowestCommonAncestor` | flat — only one tier exists |
| `Backtracking/` | `Backtrack`, `BacktrackingSteps` | flat — generic over a search state and its steps, no topology axis |
| `Bipartiteness/` | `BipartiteCheck` | flat — graph tier only |
| `Connectivity/` | `ConnectedComponents`, `StronglyConnectedComponents`, `BridgesAndArticulationPoints` | flat — three algorithms, all on the graph tier |
| `DynamicProgramming/` | `IRecurrence`, `Memoizer`, `DenseMemoizer` | flat — generic over a state and its recurrence, no topology axis. `DenseMemoizer` runs the same recurrences over int states below a stated bound, with flat arrays as the cache: it saves memory when the states reached fill their range, and time only while the range is small |
| `Folding/` | `CheckedFold`, `CycleCheckedFold`, `FoldRecursion`, `FoldStack`, `IFoldAlgebra`, `IFoldEvaluationStrategy`, `IFoldMemo`, `IterativeFoldEvaluation`, `MemoizedFold`, `RecursiveFoldEvaluation`, `ZipFoldAlgebra` (general tier; `FoldRecursion` is the one recursion the DAG and graph tiers share, with the memo policy as the axis they differ by, while the tree tier's `RecursiveFoldEvaluation` keeps a recursion with nothing to remember, §12.4) + `Dags/DagFold` (DAG tier) + `Dags/Trees/TreeFold` (tree tier) | three nested tiers, mirroring §3.2's refinement chain |
| `Geometry/` | `ConvexHull` | flat — points in the plane have no topology to be generic over; tier 2 because it composes `Sorting/MergeSort`, the reasoning that put `HammingDistances` in `Algorithms/` (§17.6) |
| `Metrics/` | `DiameterAlgebra`, `HeightAlgebra`, `HeightDiameterState`, `SizeAlgebra`, `TreeMetrics` | flat — Tree is the only tier |
| `MinimumSpanningTrees/` | `MinimumSpanningTree` | flat — `Kruskal` over `IEdgeTopology`, composing `KeyedDisjointSet` (§10.4) |
| `NumberTheory/` | `GreatestCommonDivisor`, `GcdOperation`, `LeastCommonMultiple`, `Primality`, `PrimeFactorization`, `PrimeSieve`, `ModularPower`, `ModularInverse`, `FactorialTable<TModulus>`, `IntegerSquareRoot`, `SquareExceeds`, `PascalTriangle` | flat — no topology axis; the integer algorithms are generic over `IBinaryInteger<T>` (int and long each JIT-specialized), except `ModularPower`, which is `long`-only because squaring a residue must stay inside the type. `ModularInverse` and `FactorialTable<TModulus>` take their modulus as a struct witness (`Domain/Mathematics/Modular`'s `IPrimeModulus`), LeetCode's being `LeetCodeModulus` (§17.6) |
| `Paths/` | `AllRootToLeafPaths` | flat — only one tier exists |
| `Reducing/` | `BreadthFirstReduceOrder`, `DepthFirstReduceOrder`, `InOrderReduceOrder`, `DistanceMapReduceAlgebra`, `IReduceAlgebra`, `IReduceOrderStrategy`, `NodeCountReduceAlgebra`, `Reduce`, `ZipReduceAlgebra` | flat — order strategy (BFS/DFS) is a separate axis from topology tier |
| `Sampling/` | `PrefixSumSampler`, `AliasSampler` | flat — an index drawn with probability proportional to its weight, from a ticket the caller draws: prefix sums and a binary search, O(log n) per draw, or Vose's alias table, O(1); both exact, so a test enumerates every ticket |
| `Searching/` | `BinarySearch`, `SearchRange`, `NearestBoundary` (§19.7), `MonotonePredicateSearch`, `IMonotonePredicate` (§9.6), `CompressedCoordinates` (§19.3) | flat — no topology axis at all; `BinarySearch` is generic over `Sequence.IRandomAccessSequence<T>` (§13.6), `MonotonePredicateSearch` over a rule on an integer interval (§9.6) |
| `ShortestPaths/` | `IPathHeuristic`, `ZeroHeuristic`, `ManhattanHeuristic`, `ChebyshevHeuristic`, `ShortestPath`, `BellmanFord`, `AllPairsShortestPaths`, `DenseDistanceMatrix` (general edge-weighted tier, §16) + `BreadthFirstDistances` (unweighted, §19.4) + `Grids/{GridShortestPath,GridEarliestArrival,IArrivalRule,WaitInPlaceArrival,ParityBounceArrival}` (grid tier; `GridEarliestArrival` is the time-dependent search, where a move's cost depends on the second it leaves at and a static-abstract `IArrivalRule` prices it) + `Hamming/HammingDistances` (Hamming tier, §17.6) | structure-specific tiers nested beside the general one (§13.1) |
| `Sorting/` | `MergeSort`, `SortBounds` | flat — no topology axis at all, generic over `Sequence.IIndexedSequence<T>` instead (§13.6) |
| `StringMatching/` | `PrefixFunctionSearch`, `ZFunction`, `RollingHashSearch`, `Manacher` | flat — over a `ReadOnlySpan<char>` with an optional `IEqualityComparer<char>`, no topology axis and no Representation of this repo's own |
| `TopologicalSort/` | `TopologicalSort` | flat — only one tier exists (§15) |
| `Traversal/` | `BreadthFirst/{BreadthFirstTraversal,LevelGroupedBreadthFirstTraversal,IBreadthFirstHooks,ILevelGroupedHooks}`, `DepthFirst/{DepthFirstSearch,DepthFirstTraversal,IDepthFirstHooks}`, `TopDown/{TopDownTraversal,ITopDownHooks,NodePosition}` | by traversal order; `DepthFirstSearch` is the weakest tier (no topology witness at all, a bare `Func`, §12), beside `DepthFirstTraversal` |
| `Walking/` | `BreadthFirstWalk`, `DepthFirstWalk`, `TopDownWalk`, `IStopCheck`, `AlgebraStopCheck`, `NoStopCheck`, `IVisitGuard`, `TrackedVisitGuard`, `UnguardedVisit`, `Unit` | flat — the guard tier (tree vs. graph) is a constructor parameter, not a file split (§13.2), and the stop check a type argument (§19.10) |

**`Mechanisms/`** holds state its caller owns, kept to a policy the caller can observe (§17.6b):

| Folder | Holds |
| --- | --- |
| `Cache/` | `ICache`, `CacheConstants`, `LruCache/LruCache`, `LfuCache/LfuCache` (§13.7) |
| `ExpiringSet/` | `ExpiringSet` — keys that lapse a fixed time after their life last started, counted at a moment the caller names; LC 1797's authentication manager |
| `SmallestFreeAllocator/` | `SmallestFreeAllocator` — hands out the smallest whole number not currently held, from a first one upward, and takes back the ones it holds; LC 1845's seats, LC 2336's infinite set |

**`Domain/`** is organized by subject, and holds what problems say about their world (§17.6):

| Folder | Holds |
| --- | --- |
| `Computing/Binary/` | `BitWidths` — how many bits a machine integer holds, read by the bit tries and by LC 191, LC 190 and LC 137 |
| `Computing/Filesystem/` | `FolderPaths`, `WindowsPaths` — the folder-path notation, `/` between names with the root first, read by LC 1233's sort-then-scan arm through `NamePath<FolderPaths>`, and Windows', `\` with the drive first |
| `Computing/Networking/` | `DomainNames` — labels joined by `.` with the root last, the root-last notation `NamePath` reads at the back |
| `Computing/Paths/` | `IPathNotation`, `NamePath` — a path through a tree of names in a notation, and whether another path lies inside it, one rule for root-first notations (folder paths) and root-last ones (domain names) |
| `Devices/Compass/` | `CompassHeading`, `CompassStep`, `CompassPoint`, `CompassRules` — four headings clockwise from North, quarter turns, and each heading's named step (`CompassStep.Northward` is (0, 1)), paired as named points that the step table is built from, shared by LC 874 and LC 1041. The steps and the turns are the square lattice's (`Mathematics/Lattices/`), read with north up. LC 2069's `Edge` is a side of its rectangle, not a heading, LC 657's U/D/L/R are moves with no heading, and LC 885 and LC 959 have none either. None of them uses it |
| `Devices/Locks/` | `ILockSpec<TWheel>`, `CombinationLock<TSpec, TWheel>`, `LockTurns<TSpec, TWheel>`, `FourDigitLock` — any rotary combination lock, general over its wheel count and the alphabet each wheel carries, with the four-digit lock LC 752 opens as a preset: four wheels of `DecimalDigits` |
| `Devices/Telephone/` | `PhoneKey`, `KeyPress`, `IKeypadLayout`, `Keypad<TLayout>`, `E161Keypad`, `ClassicNorthAmericanKeypad` — a keypad layout as its ten named keys, read both ways: a key's letters, read by LC 17's backtracking arm, and the key and press count that type a letter by multi-tap |
| `Games/Chess/` | `ChessRules`, `ChessPiece`, `PieceMove`, `MoveConditions`, `ChessSide`, `BetzaNotation` — the 8x8 board, and each piece as a named value carrying its moves as Betza notation writes them (the steps, a reach, and when each may be made), the pawn facing its side; the rook's, bishop's and queen's lines shared by LC 999, LC 1222, LC 2056 and LC 3001's baseline. LC 2596 and LC 3283 move a knight on boards other than chess's, so they keep `GridDirections.LShaped`; LC 3001's same-line tests are grid geometry with one user; and LC 51 and LC 52 track attacks by diagonal index, which is their strategy. None of them uses it |
| `Games/Dice/` | `IDie`, `DieFace`, `Die<TDie>`, and the presets `FourSidedDie` through `TwentySidedDie`, `PercentileDie`, `FudgeDie`, `DoublingCube`, `SichermanSmallDie`, `SichermanLargeDie` — a die as a list of faces, each a number with a weight, with the six-sided die the preset LC 2318's memoized arm rolls and LC 528's weights a weighted die its samplers draw from |
| `Games/Life/` | `ILifeLikeRule`, `LifeLike<TRule>`, `ConwayLife`, `HighLife` — Life-like rules written in Life's own B/S notation, with Conway's B3/S23, which LC 289's Set arm plays, and HighLife's B36/S23 as presets |
| `Games/SlidingPuzzle/` | `IPuzzleBoard`, `SlidingPuzzle<TBoard>`, `PuzzleSlides<TBoard>`, `EightPuzzle`, `FifteenPuzzle` — the sliding puzzle on any rectangular board: its slides, its solved board and its parity law, with the 8- and 15-puzzles as presets; LC 773 configures its own 2x3 board |
| `Games/Sudoku/` | `ISudokuGrid`, `Sudoku<TGrid>`, `ClassicSudoku`, `SudokuDigits` — Sudoku of any box size n, the board n² a side, with the classic 9x9 grid of 3x3 boxes and its digits '1'–'9' the preset LC 36 and LC 37 play |
| `Games/TicTacToe/` | `TicTacToeRules`, `TicTacToePlayer`, `TicTacToeOutcome` — LC 1275's board, turn order, diagonals and outcome rule |
| `Games/Zuma/` | `IZumaRule`, `ZumaRow<TRule>`, `ClassicZuma` — the cascading collapse of touching same-coloured balls, general over the group length that vanishes, with Zuma's own three the preset LC 488's BFS arm uses |
| `Language/` | `CaseFolded` — a cased alphabet with case set aside, a letter and its capital in one slot, exact only where case pairs letter for letter (§19.6); `Multigraph`, `IMultigraphSet`, `MultigraphSet<TSet>` — letters that together spell one sound, and a language's set of them read once into a table looked up by span, in either case; `NumberWord` — a number and the word a language names it with, from which a table of words is built; `TimeUnitWord` — a unit of time, by its length, and the word a language names it with |
| `Language/English/` | `EnglishLowercase`, `EnglishUppercase` — the 26 letters in each case, read by the count tables and the trie, and by LC 424's and LC 621's capitals; `EnglishLetters` — both cases folded, with English's numbering, A as 1 through Z as 26, read by LC 2156's rolling-hash arm and LC 91's memoized arm; `EnglishNumberWords` — English's number words, zero to nineteen, the tens, a hundred and the short scale's powers of a thousand, each a `NumberWord` with its number, from which LC 273 builds its capitalized tables and reads the numbers it splits by; `EnglishTimeUnitWords` — English's words for the units of time, the microsecond to the week, each a `TimeUnitWord` carrying its unit's length from `TimeUnit`, which no problem reads: LC 1348 keeps its own frequency names. LC 639's letter count sits in a helper its baseline shares and reads none of them |
| `Language/English/Orthography/` | `EnglishVowels` — the five written vowels as a letter set over `EnglishLetters`, shared by LC 1641, LC 2063 and LC 3227; `EnglishConsonants` — the other 21 letters, y among them, read from the vowels, which no problem uses yet; `EnglishMultigraphs` — the digraphs ch, sh, th, ph, ng and ck and the trigraph tch, each a named value, read into `MultigraphSet` |
| `Language/English/Phonology/` | `ReceivedPronunciation` — Received Pronunciation's 44 phonemes as a configuration of the IPA, each vowel named by its lexical set; `LetterY` — y, a consonant letter that spells the consonant j and the vowels of KIT and PRICE |
| `Language/Phonetics/` | `IPhonemeInventory`, `PhonemicTranscription<TInventory>`; in `Vowels/` `Vowel`, `VowelHeight`, `VowelBackness`, `Rounding`, `VowelLength`, `Monophthong`, `Diphthong`, `IpaVowels`; in `Consonants/` `Consonant`, `Voicing`, `Place`, `Manner`, `IpaConsonants` — the IPA's vowel chart, its pulmonic consonant chart with the chart's other symbols and the common affricates, each sound a named value; a language's phonemes as a selection of them; and a transcription read back into an inventory's phonemes |
| `Language/Scripts/Latin/` | `Diacritic`, `DecomposedLetter`, `LatinScript`, `IAccentedAlphabet`, `AccentedLetters<TAlphabet>`, and the presets `SpanishAlphabet`, `FrenchAlphabet`, `GermanAlphabet` — the Latin script's 17 named diacritics, a letter taken apart into its basic letter and marks through Unicode's canonical decomposition, and the accented alphabets that reach it; English's alphabets never do |
| `Mathematics/Algebra/` | `IMonoid`, `IAbelianGroup`, `IIntegerModule`, `Addition`, `Minimum`, `Maximum`, `BitwiseXor` — the laws a range structure needs of its elements, one chain under the mathematics' names (§11.4), drawn on by the segment trees, the Fenwick trees and `PrefixSums` |
| `Mathematics/Geometry/` | `EuclideanPolygons`, `EuclideanPlane`, `ConstantCurvatureSurface`, `AngleUnits` — when sides close into a polygon and a triangle's angle sum on the flat plane, read by the composed arms of LC 2971, LC 976, LC 3024 and LC 3899; a triangle's angle sum on any surface of constant curvature; and a turn's degrees, read by LC 1610 |
| `Mathematics/Lattices/` | `IPlanarLatticeStep<TSelf>`, `LatticeSymmetry`, `GridStep`, `GridDirections` — a step of the square lattice a grid's cells sit on, each named (`Up` is (-1, 0)), with the lattice's algebra (steps add and negate, a quarter turn and a mirror), its orbits, its neighbour tables in a fixed order (orthogonal, diagonal, octilinear, L-shaped, and the distance neighbourhoods), and the clockwise turning cycle. Read by tier 1's grid engines, `ChessPiece`'s moves and the grid solutions, baselines among them |
| `Mathematics/Surfaces/` | `ISurface`, `GluedRectangle<TRows, TCols>`, `MirroredAxes`, the presets `BoundedRectangle`, `Cylinder`, `Torus`, `MobiusStrip`, `KleinBottle`, and in `Gluings/` `IEdgeGluing`, `Unglued`, `Straight`, `Twisted` — a board's global shape, a rectangle with each pair of opposite edges left open, glued straight or glued twisted; `GridSize.HasCell` is the bounded rectangle's rule |
| `Mathematics/Modular/` | `IModulus`, `IPrimeModulus` — a modulus as a struct witness, and a prime one, under which every nonzero residue has a Fermat inverse; the algorithms over them are `Algorithms/NumberTheory`'s, and LeetCode's 10^9 + 7 is `LeetCodeModulus` at the LeetCode tier's root |
| `Mathematics/Numerals/` | `DecimalDigits` — the ten digits as an alphabet whose slot is a digit's value, carried by `FourDigitLock`'s wheels and read by `LifeLike`'s rulestrings; `IPartsPerNotation`, `Percent`, `Permille`, `Permyriad`, `PartsPerMillion` — a proportion written as parts of a fixed whole, with the per cent LC 1357's discount and LC 2303's tax rates are stated in |
| `Mathematics/NumberTheory/` | `Primes` — the smallest prime, which every sieve and factorization starts from, read by `PrimeSieve` and eleven problems |
| `Science/Dna/` | `DnaBase`, `DnaBases` — DNA's four bases as named values and as an alphabet, the one LC 433's benchmark mutates its genes over and the letters LC 187's workload strings are spelled in. LC 187's 2-bit code per base is its strategy and stays in its folder, so no solution reads it |
| `Science/Time/` | `ITimeUnit`, `TimeUnit` — units of time, the microsecond to the week, each a struct whose length is a constant read from .NET's `TimeSpan`, so a count of one unit in another, such as LC 1348's minute, hour and day in seconds, is a constant expression; their English names are `Language/English`'s |
| `Science/Time/Calendars/` | `ICalendar`, `Calendar<TCalendar>`, `GregorianCalendar`, `JulianCalendar`, `CalendarMonth`, `CalendarMonths`, `CalendarDate`, `RataDie` — the Julian family of calendars: one leap rule of three cycles held as constants on struct witnesses, the Gregorian's 4, 100 and 400 and the Julian's 4, 100 and 100, and the twelve months' lengths. From them come leap years, month and year lengths, a date's day of the year, its day on the rata die count every calendar shares, the days between two dates and the weekday. No solution reads them yet |

`DSAExperimentation.Tests/` mirrors every tree one level deeper (§7, §18.1). Fixture files go to the
utility folder matching the interface they implement, not a shared grab-bag: the `Recording*Hooks`
and `Counting*Hooks` fixtures sit under
`DSAExperimentation.Tests/Algorithms/Traversal/{BreadthFirst,DepthFirst,TopDown}/Fixtures/` by which
hook interface each implements. The graph fixtures `TestNode`, `TestTopology`, `TestTrees` and
`TestGraphs` sit at `DSAExperimentation.Tests/DataStructures/Graph/Fixtures/` (`TestTopology` is
itself a Topology fixture) and are referenced from `Algorithms/`-side test classes too — harmless for
test-only code. A test that exercises more than one utility together sits at the root they share
rather than being forced into one utility's folder: `Tests/Algorithms/ZipTests.cs` (Fold and Reduce
in one file), `Tests/Algorithms/Folding/SharedDescendantFoldTests.cs` (the fold tiers compared).

### 13.4 `suppressions.json`

`suppressions.json` at the repository root is the Nomos gate's waiver ledger, beside
`standards.json`, which anchors it. It is live: the gate reads it on every run, and a waived finding
leaves the gate's output entirely. Inline suppression markers are not a substitute: the gate's
marker policy, which `standards.json` leaves at its default, honours only safety markers in code, so
a waiver goes in the ledger. Each entry names the check, a
path (one file or a glob), the reason, an owner and an expiry date; an entry narrowed to one finding
also names its message or anchor.

- **Fix first (§6).** A waiver records a finding a fix would make worse, or a check that reads one of
  this document's conventions as a defect (§17.9's tier-wide entries). It never stands in for a
  missing test (§18.2).
- **A reason must be true of everything it absorbs.** The tool checks only that a path matches, so
  a glob is where a reason rots unnoticed; when the code under a glob changes, re-read the reason
  against the findings it now absorbs.
- **A waiver moves with its file.** Renaming or moving a file orphans its path-scoped entries.
  Rewrite each under the new path in the same change and re-run the checks they covered, including
  checks from phases the change was not about.

### 13.5 Operations rejoins Representation when it is hardwired to one

§2's literal definition of Operations ("the algorithm/API surface built on top of a Representation
constrained by a Topology") covers `Heap`, `HashMap`, `DynamicArray`, `DisjointSet`,
`CircularBuffer`, `ContiguousGroupBuffer` — and, one level removed, `Stack`, `Deque`, `Queue`, their
bounded forms, `Set` and `MonotonicDeque`, each hardcoding another structure rather than a Storage
class of its own. That isn't the discriminator that
matters. `ShortestPath`/`MergeSort`/`BinarySearch`/`Reduce`/`Fold` are generic over an *interface*
with genuine multiple implementations (`IEdgeTopology`, `IRandomAccessSequence`, `IIndexedSequence`,
`ITreeTopology`/`IDagTopology`/`IGraphTopology`) — decoupled from any one representation, and free
to run over a different one tomorrow. The containers are hardwired 1:1 to exactly one concrete
type, with no interface and no second implementation possible — `Heap.cs` only ever composes
`HeapArray`, `Stack.cs` only ever composes `DynamicArray`. They aren't algorithms decoupled from a
data structure; they *are* the data structure, with the bounds-checking layer split out (§5 step 7).

So they live in `DataStructures/`, beside their Storage (`DataStructures/Heap/Heap.cs` beside
`HeapArray.cs`), or, for a type composing another structure, in a folder named for itself beside the
type it composes (`DataStructures/Stack/` beside `DynamicArray/`). The same holds for a structure's
own walks: `InOrderTraversal` and `FindClosest` sit beside the binary tree they walk, and
`HammingSearch` beside the Hamming graph, because each depends on nothing above tier 1.

Everything else that is Operations lives in `Algorithms/`: the types generic over a capability
interface with real substitutability, and the algorithms that run over plain inputs with no
Representation of this repo's own to be hardwired to — `StringMatching` over a `ReadOnlySpan<char>`,
`NumberTheory` over `IBinaryInteger<T>`, `Backtracking` and `DynamicProgramming` over a
caller's state. `BinarySearch`/`MergeSort` are the case that looks like a container but isn't: they
have no *topology* axis either, but `IRandomAccessSequence`/`IIndexedSequence` each have more than
one real implementation, so they stay in `Algorithms/`, decoupled from any one.

The one thing that overrides the hardwired rule is the tier order (§17.2): a type that would belong
in `DataStructures/` by this section but composes an `Algorithms/` engine moves up to `Algorithms/`,
because tier 1 may not depend on tier 2. `HammingDistances` (§17.6) and `CompressedCoordinates`
(§19.3) are filed this way.

### 13.6 `DataStructures/` folders are named for identity: `Sequence/`

A `DataStructures/` folder is named for what its types *are*, not for the algorithm that consumes
them (§5 step 6). `IRandomAccessSequence` with its witnesses and `IIndexedSequence` with its
witnesses are both "sequence access contracts," at two different read/write capability levels — not
"a searching thing" and "a sorting thing" — so both sets co-locate under `DataStructures/Sequence/`,
the way `MinHeapOrder`/`MaxHeapOrder` sit as sibling witnesses under `Heap/`. They remain two
separate, non-reused domains per §5's domain-reuse rule (sorting never reuses searching's contract,
§11.1), grouped by theme rather than split across two algorithm-named folders.

A type that is an algorithm's own cursor or range-grouping state is not a Representation of the
sequence being searched or sorted, wherever it was first written. `SearchRange` and `SortBounds`
are Operations-internal (their own doc comments say so), the same bucket §12.2 puts
`TrackedVisitGuard` in, and they sit beside their algorithms in `Algorithms/Searching/` and
`Algorithms/Sorting/`.

### 13.7 What joins `Graph/`, and `ICache`

A structure that implements one of the graph topology contracts lives in `DataStructures/Graph/`,
beside the contract it witnesses. `BinaryTreeTopology` implements
`ITreeTopology<BinaryTreeNode<TValue>,BinaryTreeChildren<TValue>>`, so the binary tree sits at
`Graph/Engines/Dags/Trees/` — the same relationship `Grids/` has with `IGraphTopology`.

A tree-*named* structure is not thereby a tree witness. `SegmentTree`, `FenwickTree`,
`LazySegmentTree`, `RangeFenwickTree`, and `Heap` (§4) are all array + index-arithmetic
representations with no separate node identity for a `TChildren` parameter to vary over — the same
reason §4 gives for keeping `Heap` out of `IGraphTopology`. Promoting any of these to a real
`ITreeTopology`/`IDagTopology` witness — and re-deriving their hand-written recursion as instances
of Graph's generic `Fold`/`Walk` engines — is a per-structure algorithm redesign, not a file
move, and is a deliberate future exercise. `SinglyLinkedList`/`DoublyLinkedList` stay out for the
same reason: no `IChildren`, no `ITreeTopology`, and their O(1) pointer-splice operations
(`AddFront`/`Remove`/`PopBack`, Floyd's cycle detection) don't route through Graph's generic
traversal engines. Tries are decided case by case (§13.8, §13.9).

`LruCache`/`LfuCache` sit in `Mechanisms/Cache/LruCache/` and `Mechanisms/Cache/LfuCache/` beside
`Mechanisms/Cache/ICache.cs`, which both implement; §17.6b says why they are mechanisms rather than
data structures. They are peer implementations of one public contract, and `ICache<TKey,TValue>`
makes that contract real. It is an interface, not a shared base class, per §6's
composition-over-inheritance precedent (`IHeapOrder`, `IMonoid`, `IAbelianGroup` are
all interfaces) — the two caches share no method bodies to inherit, only this public shape, and
LRU-vs-LFU is a real, caller-visible axis (it changes which key an identical call sequence evicts)
rather than a mechanics-only choice, the same §10.1 discriminator that separates
`MinHeapOrder`/`MaxHeapOrder` from `DisjointSet`'s linking policy.

### 13.8 Whether a trie gets a witness is a Representation question

"Nothing for a witness to parametrize" answers the wrong question about a trie.
`GridTopology`/`BinaryTreeTopology` also have nothing to parametrize (no Min-vs-Max-style variant
choice) and still implement the base interface fine — determinism was never the test for *whether a
type implements `ITreeTopology` at all*; it's only ever been the test for whether a type needs a
*second*, injected witness axis on top of that (§5's own table, §9.1, §13.2).

The real question is Representation, checked directly rather than assumed: every call site of
`IChildren<TNode>.Get(int)` in this repo (`DepthFirstWalk.cs`, `BreadthFirstWalk.cs`,
`TopDownWalk.cs`) drives it in a plain sequential `for i in 0..Count-1` sweep, never random access —
but the interface's own doc comment still states an O(1)-*per-call* obligation, and a node's
children kept in a `HashMap<char,·>` (`HashedChildren`, `SuffixTreeNode.Children`) cannot honestly
promise indexed O(1) `Get` the way a fixed slot array can. That is a §8 Representation/
Complexity-law mismatch — and it is fixable, by bounding the alphabet:
the one trie, `Graph/Engines/Dags/Trees/Tries/Trie<TStore, TValue>`, takes where a node keeps its
children as a type argument, and `AlphabetSlots<TAlphabet, TLayout, TValue>` keeps them in slots over
any `IAlphabet` of at most 64 letters (§19.6). The store keeps a mask with a bit per letter that has a
child, so `TrieChildren.Count` is a popcount and `Get` finds a child by its rank without scanning empty
slots, and a `foreach` over a node's `Children` gives each child with its letter, so a walk that needs
every child's letter visits only the children present. `TrieTopologyTests.cs` proves the witness is
real, not just a satisfied interface, with two facades reusing generic Tree-tier engines unmodified:
`TreeMetrics.Size` counts every node across a shared-prefix chain, and `LowestCommonAncestor.Find`
over a trie topology *is* longest-common-prefix — the node where two keys diverge is exactly the node
reached by walking their shared prefix, with no trie-specific LCA code at all.

Where the slot store keeps its children is its second type argument (owner ruling, 2026-11-02).
`ITrieLayout` answers in integers, a position and a capacity, so a layout is not generic over the
node type and a trie compiles only its own layout's arithmetic. `DenseTrieLayout` gives every letter
a slot, read directly. `PackedTrieLayout` stores only the children present, in letter order, at the
popcount of the mask below the letter, growing from one slot by doubling up to the whole alphabet.
Measured outside the tree on `710a862b` (IMPROVEMENT-WORKLIST), packed nodes made LC 336 2.2 to 5
times faster with 3.4 times less allocation, and every other trie built per call 25-68% faster, while
StreamOfCharacters, which builds its trie once and then only reads it, ran 52-70% slower. So each
adopter names its layout: LC 1032 dense, the other twelve packed. The general trie's ten users chose
the same way when they moved onto the one trie (0ffda5d2): LC 3093 and LC 472, whose benchmarks
build the trie once and then read it, dense, 32-39% and 9-16% faster than packed; the other eight
packed. On LC 208, whose every run builds the trie, the packed set is 65% faster than the textbook
trie at 10,000 words, and the dense set, laid out as that trie is, 5-15% slower than it. Three
walks that need every child's letter (LC 676, LC 720, LC 2452) first asked each node for all 26
letters, and measured 4-44% slower packed than HEAD's dense trie; walking the node's `Children`
instead made them 18-86% faster than HEAD under either layout, and packed allocates a third to a
quarter as much.

A trie over `HashedChildren<TValue>`, keyed by any `char`, correctly stays witness-less — its whole
value is *not* being bounded to a small alphabet, so narrowing it to gain a witness would defeat its
purpose rather than just cost something. Its store has no order, and `TrieTopology` asks for an
`IOrderedTrieChildStore`, so it is the compiler that withholds the tree engines from it. `BitTrie`
earns one (§13.9). `SuffixTree`,
`SuffixAutomaton` and `AhoCorasick` are not yet reviewed under this test, and each raises a question
of its own: `SuffixTree`'s edge-compression (substring-range labels, not one-char-per-edge) is a
second, independent representation question on top of the alphabet one; `AhoCorasick`'s trie
portion has the same bounded-alphabet shape `BitTrie`/`AlphabetSlots` do, but its runtime matching
walks `Fail`/`OutputLink` edges that aren't tree edges at all, so at most its construction-time trie
would earn a witness, not the automaton itself; `SuffixAutomaton`'s transition graph is a DAG, not
a tree, though its suffix-link parent pointers form a separate, real tree that's a plausible
witness candidate in its own right.

There is one trie, not two (the owner's ruling of 2026-11-02, quoted in §17.5). Until then a
general `Trie<TValue>` and a bounded-alphabet sibling each carried a copy of the key walk behind
`Set`/`HasKey`/`TryGetValue`/`HasPrefix`, and this section called the copy deliberate. What differs
between them is only how a node keeps its children, so that is now the injected axis and the walk is
written once, in `Trie<TStore, TValue> : IPrefixMap<TValue>`. The objections once raised against
sharing it do not hold for this shape. There is no base class: `TrieNode<TStore, TValue>` is one
sealed class that holds its store as a struct field. The store contract, `ITrieChildStore`, is the
trie's own Representation axis, not one contract spanning two domains. And a reference-typed `TValue`
shares code just as the bounded-alphabet trie's node did, since each store is a struct type argument
compiled for its own alphabet and layout. A problem names the store its invariants allow: keys over
a small alphabet `AlphabetSlots`, any other keys `HashedChildren`. A hand-specialized trie is a
strategy behind the contract for exactly the layer it replaces, and is benchmarked against the
template (§17.5): `IPrefixMap` for a map, or `IPrefixSet` for a trie that keeps keys and no values,
as LC 208's textbook trie does. The one trie is a set through `PrefixSet`, a struct view of a map to
`bool` that holds one reference and walks nothing itself, so one contract suite, `PrefixSetTests`,
runs over the textbook trie and the view under each store. Moving the bounded-alphabet trie's 12
problems onto the one trie changed no allocation, and no time beyond what identical code reads
apart, once the slot store's enumerator held a reference to the store rather than a copy: a copy is
two values a recursive walk keeps alive across its calls, and it cost LC 676 12-30%. Moving the
general trie's ten problems onto it made every one faster, 13-94%, since each keeps its children in
slots over the small alphabet LeetCode bounds its keys to rather than in a hash map, and the general
trie was deleted.

### 13.9 The two-part witness test, and `BitTrie`

When a structure has a Topology-witness question, don't ask "is there a variant to inject" (§9.1's
row-1/row-2 test still answers that, but answers a *different* question). Ask instead: (1) does the
structure have a node identity distinct from its storage (§4's Heap test), and (2) does its actual
Representation satisfy every law the base interface's consumers actually rely on (§8) — checked
against real call sites, not assumed from the interface's stated contract alone. Both true → a real
witness is earned, and the type's own distinguishing feature (bounded alphabet, edge compression, a
balance invariant, ...) is what to name and isolate, the same "inject the one new axis" idiom
`IPathHeuristic`/`IHeapOrder`/`TOperation` already use elsewhere in this repo. Either false → the
structure stays domain-local, correctly, for a stated reason — not a default.

`BitTrie` passes both. `BitTrieNode` has real identity (a `class`, not an index into shared storage,
unlike `Heap`/`DisjointSet`), and its two named `Zero`/`One` slots already give `IChildren`'s
O(1)-indexed-`Get` obligation honestly, with no alphabet-bounding work needed the way `AlphabetSlots`
required.

`BitTrieChildren` follows `BinaryTreeChildren`'s shape — computed on demand from two named
fields, no backing array at all. `BitTrieNode` has exactly `BinaryTreeNode`'s "two named slots"
shape (deliberately, per its own doc comment, to avoid a `HashMap`'s cost for a fixed 2-key
alphabet); switching it to an actual 2-element array to reuse `SparseArrayChildren` verbatim would
be a real, unforced representation regression (an allocation per node) purely to match another
trie's file shape rather than its reasoning. `BinaryTreeChildren`'s own "compaction loses
positional identity" carve-out (why `InOrderTraversal` bypasses it) doesn't apply here either:
`Insert`/`TryMaxXor` never go through `IChildren` at all, so nothing downstream needs "this was
specifically the 0-branch vs. the 1-branch" preserved — only generic Tree-tier engines consume the
witness, and they only ever ask "how many children, and give me each one."

`Insert`/`TryMaxXor` themselves stay hand-written against `Zero`/`One` directly — the witness
doesn't replace them, it opens the trie to generic Tree-tier engines for free.
`BitTrieTests.Size_ViaTreeMetrics_CountsEveryNodeAcrossASharedBitPrefixChain` proves the witness is
real the same way `TrieTests` does: `TreeMetrics.Size` (`Algorithms/Metrics`, generic over
any `ITreeTopology`) counts every node of a two-value trie with no `BitTrie`-specific code at all.
`CountedBitTrie` fails both halves of the test, and §13.3 records why.

## 14. Worked example: the three Storage/Operations decompositions

Three containers — `ContiguousGroupBuffer`, `HashMap`, `DynamicArray` — split their storage from
their public operations, as `Heap`/`HeapArray` (§4) does. All three follow the same shape:
**Storage owns raw, unchecked, non-throwing access; Operations owns bounds/precondition checks,
throwing, and the one composed Storage field.** Both halves live under `DataStructures/<Structure>/`
(§13.5), since none of the three has a capability interface to be generic over. A decomposition,
unlike a file move, can change behaviour, so each is held by the full test suite, not only its own
folder's tests.
`CircularBuffer` has no Storage half, though it once had one; §20 says why.

### 14.1 `ContiguousGroupBuffer`

| File | Axis | Why |
| --- | --- | --- |
| `DataStructures/Buffers/ContiguousGroupBufferStorage.cs` | Representation | the current group and its key; unchecked `HasCurrentKey`/`CurrentKey`/`CurrentGroupCount`/`AppendToCurrentGroup`/`SnapshotCurrentGroup`/`ClearCurrentGroup`/`SetCurrentKey`/`MarkCurrentKeyConsumed`/`ResetAll` |
| `DataStructures/Buffers/ContiguousGroupBuffer.cs` | Operations | `keyComparer`, `GetCurrentKey`/`ThrowMissingCurrentKey`, composes Storage |

`MarkCurrentKeyConsumed` deliberately clears only `HasCurrentKey`, leaving the stale `CurrentKey`
value in place (only `Reset` clears both) — `Flush`'s behaviour, which has a test of its own.

### 14.2 `HashMap`

| File | Axis | Why |
| --- | --- | --- |
| `DataStructures/HashMap/HashMapStorage.cs` | Representation | bucket array + separate-chaining entries array + free list; owns `Grow`/`RehashEntryInto`/`CreateEmptyBuckets`/`AllocateEntrySlot` (none touch the comparer) and `Insert(hashCode,key,value)` |
| `DataStructures/HashMap/HashMapEntry.cs` | Representation | one chained entry, beside Storage |
| `DataStructures/HashMap/HashMap.cs` | Operations | `_comparer`; `TryUpdateExisting`/`FindEntryIndex`/`TryRemove`'s chain-walk loops, reading each step via raw Storage accessors (`BucketHead`/`Entry`) |

The split point here isn't read/write the way `DynamicArray`'s is (§14.3) — it's "needs the
comparer" vs. "doesn't." `Storage.Insert` absorbs the whole grow-if-needed → recompute-bucket →
allocate-slot → write → link pipeline, since "should this insert grow first" isn't a
comparer-dependent decision, and it always recomputes its own fresh bucket index internally after
any grow rather than accepting one as a parameter — structurally ruling out a stale post-grow
bucket index, the same kind of silent-misrouting risk §8's "performance independence" section warns
about, except here it's a correctness risk, not a Big-O one. A test forcing a `Grow()` mid-sequence
and then a `TryGetValue` on a key inserted immediately before the resize exists specifically to
catch a regression here.

The comparer stays a runtime object (§9.2), but the default one is not called through it. When
`TKey` is a value type and the comparer is `EqualityComparer<TKey>.Default`, `_comparer` is null,
and `IsSameKey` and `KeyHashCode` call `EqualityComparer<TKey>.Default` itself, which the JIT
devirtualizes and inlines: the shape `Dictionary<TKey,TValue>` uses. Through the stored interface,
the `inlining` verb (§19.8) read every probe of a chain in LC 874's arm as an interface call the JIT
could not inline ("target not direct"). `IsSameKey` is always inlined, since without the attribute
the JIT judged it an unprofitable inline in the same loop. Both test `typeof(TKey).IsValueType`
first, which the JIT folds. A reference-type key's code is shared across every reference type, where
`Default` is no more direct, so it keeps its comparer and compiles to the interface call it always
made. With the dead `Default` arm left in, GroupAnagrams' string-keyed arm allocated 31 B more per
call. A custom comparer is still called through the interface.

### 14.3 `DynamicArray`

| File | Axis | Why |
| --- | --- | --- |
| `DataStructures/DynamicArray/DynamicArrayStorage.cs` | Representation | `_items: T[]`; own `Count`; unchecked `Get`/`Set`/`Add`/`InsertAt`/`RemoveAt` (assume the caller already validated the index); private `EnsureCapacity` |
| `DataStructures/DynamicArray/DynamicArray.cs` | Operations | `IsValidIndex`/`ThrowIndexOutOfRange` and the `IndexOutOfRangeMessage` constant; bounds-checked `Get`/`Set`/`RemoveAt`/`Insert` and their `Try` forms delegate to Storage after validation |

The highest fan-out of the three: `Stack`, `Sequence.DynamicArraySequence`, and
`Sequence.DynamicArrayIndexedSequence` all compose it, and none calls anything beyond
`Add`/`Get`/`Set`/`RemoveAt`/`Count`, so the split is invisible to them.

## 15. Worked example: `Algorithms/TopologicalSort`

`Algorithms/TopologicalSort` fills a gap `Folding/CheckedFold.cs` names explicitly in its own
doc comment: "an iterative, stack-safe DAG fold needs a real topological sort." It sits at the
untrusted tier `CheckedFold` occupies — generic over `IGraphTopology<TNode,TChildren>`, not
`IDagTopology`, because Kahn's algorithm's own leftover-in-degree check *is* the cycle check, the
same law `CheckedFold` resolves this way. That tier choice alone would not need a section —
`Connectivity/ConnectedComponents.cs` sits at the same bare `IGraphTopology` tier with only a §13.3
row — but it brings a precondition shape nothing else here has, and a second occurrence of §10.3's
namespace collision.

| File | Axis | Why |
| --- | --- | --- |
| [`Algorithms/TopologicalSort/TopologicalSort.cs`](DSAExperimentation/Algorithms/TopologicalSort/TopologicalSort.cs) | Operations | `TrySort` — Kahn's algorithm; composes `IGraphTopology`/`IChildren`/`IChildOrder` from `DataStructures/Graph/Contracts/**`, owns none of them, needs no new Representation or Topology contract |

### 15.1 A precondition on the input's completeness, and its asymmetric failure

The other unenforced preconditions here constrain a *property* of the input —
`BinarySearch`'s sortedness (§9.1), `ShortestPath`'s non-negative edge weights (§7). `TrySort`'s
precondition is different in kind: `nodes` must enumerate the *complete vertex set* of the graph,
not merely its sources. A root-seeded walk (`Reduce.Graph`, `ConnectedComponents.Count`) only
needs one representative per component, because it discovers descendants transitively through
`GetChildren`. Kahn's algorithm can't work that way — it needs every vertex's true in-degree
before the first node is ever dequeued, and there is no reverse-adjacency contract anywhere in
`DataStructures/Graph/Contracts/**` (no way to ask a node for its parents), so nothing here could
discover a missing vertex even if it wanted to.

The two ways to violate this precondition fail differently. Omitting a *descendant* self-detects:
it's still discovered via its parent's edge during the walk, which inflates the produced ordering
past `vertices.Count`, so `TrySort` correctly returns `false`. Omitting an *ancestor* does not:
nothing forward-reachable from the supplied set points back to it, so `TrySort` returns `true` with
an ordering that silently omits it — a caller cannot tell this happened from the return value
alone. A consequence worth stating plainly: a `false` result therefore doesn't distinguish a true
cycle from a caller-omitted descendant either — both present identically as leftover in-degree.

### 15.2 Free cycle detection, unlike `CheckedFold`'s recursion-path set

`CheckedFold.TryFold` needs an explicit `HashSet<TNode> inProgress` tracking the current
recursion path, because a cycle only reveals itself as "a node still on the path being reached
again" mid-descent. Kahn's algorithm needs no equivalent structure: a cycle *is* the set of nodes
whose in-degree never reaches zero, which the algorithm was already computing for an unrelated
reason (deciding what to dequeue next). This is a case where the untrusted tier's defense comes
free of charge rather than as an added cost over the trusted tier — there is no `DagFold`-style
faster sibling for this utility to skip down to, because there's nothing left to skip.

### 15.3 When an `Algorithms/` folder and its class share a name

`Algorithms/TopologicalSort/TopologicalSort.cs` names its folder and its class identically — the
same shape §10.3 documents for `DataStructures/DisjointSet/DisjointSet.cs`, and the same C# wrinkle
results: `TopologicalSortTests.cs` at namespace `DSAExperimentation.Tests.Algorithms.TopologicalSort`
can't reference a bare `TopologicalSort` without it resolving to the enclosing namespace segment
instead of the type, since namespace-member lookup wins over `using` directives. The fix is §10.3's,
a differently-named alias:
`using TopologicalSortOperations = DSAExperimentation.Algorithms.TopologicalSort.TopologicalSort;`

The rule behind keeping the name: an `Algorithms/` folder is named for *what the algorithm does*
(§5 step 6), which usually reads as a broader category than any one class
(`Connectivity`/`ConnectedComponents`, `Ancestry`/`LowestCommonAncestor`, `Paths`/
`AllRootToLeafPaths`, `Searching`/`BinarySearch`, `Sorting`/`MergeSort`). When the category and the
sole technique's name coincide exactly, keep the descriptive name and alias around the collision.
Renaming the class to something eponymous (`KahnsAlgorithm`) would dodge the collision but breaks
this repo's preference for descriptive over eponymous names for a sole/primary technique
(`MergeSort`, `BinarySearch`); `ShortestPath.Dijkstra`/`.AStar` are eponymous, but as *methods*
distinguishing several techniques inside one shared category class, which doesn't apply when only
one technique exists.

## 16. Worked example: `BellmanFord` and `AllPairsShortestPaths`

`Algorithms/ShortestPaths/` holds two siblings of `ShortestPath.cs` that relax its
non-negative-edge-weight law (§7) rather than assume it, and report a negative cycle rather than
silently mis-answer. Both reuse `ShortestPath.cs`'s `IEdgeTopology`/`IEdges` contracts unchanged,
so neither is here for its Representation/Topology choice — what they settle is §15.3's naming
question, landing on a different answer for each file, and a failure-reporting distinction that
neither `ShortestPath.cs` nor `TopologicalSort` needs on its own.

| File | Axis | Why |
| --- | --- | --- |
| [`BellmanFord.cs`](DSAExperimentation/Algorithms/ShortestPaths/BellmanFord.cs) | Operations | Single-source, negative-weight-tolerant relaxation with cycle detection; composes `IEdgeTopology`/`IEdges`, owns neither |
| [`AllPairsShortestPaths.cs`](DSAExperimentation/Algorithms/ShortestPaths/AllPairsShortestPaths.cs), [`DenseDistanceMatrix.cs`](DSAExperimentation/Algorithms/ShortestPaths/DenseDistanceMatrix.cs) | Operations | All-pairs refinement over a dense matrix; `DenseDistanceMatrix` builds the index map and the seeded starting matrix from the same contracts, and `AllPairsShortestPaths` refines it in place |

### 16.1 Same contracts, no shared engine — why `BellmanFord` isn't a third `ShortestPath` method

§3.3's template for `Dijkstra`/`AStar` sharing one class is "one algorithm, one injected axis, not
two parallel implementations" — they are literally the same `Explore`/`Traverse`/`RelaxNeighbors`
engine closed over a different `THeuristic`. Bellman-Ford shares none of that mechanism: no
priority queue, no settled set, a fundamentally different bounded-rounds-over-every-edge loop
instead. Output shape (a `Dictionary<TNode,TWeight>` of distances from one source) is the same as
`Dijkstra`'s, but §3.3's actual test is shared mechanism, not shared output — so this stays its own
file, flat in `ShortestPaths/` rather than nested (§13.1's nesting rule mirrors topology/strategy-
witness refinement chains like `Grids/`; non-negative-weight is an unenforced doc-comment law, not
a witness, so there is no refinement chain here to mirror).

### 16.2 `BellmanFord`: eponymous where §15.3's principle runs out of a better answer

§15.3 states a preference for descriptive category names over eponyms, with `TopologicalSort`
the case where "the category and the sole technique's name coincide" and no better descriptive name
was available. `BellmanFord.cs` is a second such case, for the same underlying reason: the category
this file's output belongs to — "single-source shortest paths" — is the name `ShortestPath.cs`
already has. There is no second, equally-natural descriptive name to give a
negative-weight-tolerant sibling that doesn't either collide or read as contrived
(`NegativeWeightShortestPath` names the precondition it relaxes, not what it computes). Unlike
`TopologicalSort`, this isn't the *only* naming option this section discusses — see §16.3 — it's
specifically the *class* name that has no descriptive alternative here.

### 16.3 `AllPairsShortestPaths.FloydWarshall`... except the method isn't eponymous either

An eponymous *method* on a descriptively-named class is licensed by an **uncheckable**
precondition. `ShortestPath.Dijkstra`/`.AStar` and `MinimumSpanningTree.Kruskal` all have one —
non-negative weights, a validly-disconnected input — so none of them can ever fail at runtime, and
none of them is `TryX`-shaped. `TopologicalSort.TrySort` is the opposite case: Kahn's algorithm
*can* fail (checkably, via leftover in-degree), and its method is a plain descriptive verb, not
`TryKahn`. `BellmanFord`/`AllPairsShortestPaths` both belong to the checkable-failure bucket (a
reachable negative cycle), so both take `TrySort`'s naming shape — `TryComputeDistances` — not
`Dijkstra`'s, even though `AllPairsShortestPaths` is a natural category name that would otherwise
invite a `FloydWarshall` method. Each technique's proper name appears prominently in its file's own
leading doc comment, the same way "Kahn's algorithm" appears only in `TopologicalSort.cs`'s prose,
never in an identifier.

### 16.4 The dense matrix is Operations-internal scratch state, not a second Representation

`AllPairsShortestPaths` refines a `TWeight[,]` matrix that `DenseDistanceMatrix` seeds once from the
same `IEdgeTopology`/`IEdges` contracts `ShortestPath.cs` and `MinimumSpanningTree.cs` reuse (§5
step 5's same-domain-reuse allowance, unaffected by what happens to the data afterward). The matrix
never leaves the call and never competes for a place beside `IEdgeTopology`/`IEdges` as a second
Representation — the same bucket `ShortestPath`'s own `Heap` frontier and `TopologicalSort`'s
`inDegree` dictionary are in (§12.2): Operations-internal working state, not a caller-visible
contract. `DenseDistanceMatrix` is an internal helper of the walk, split out because building the
starting matrix shares no state with refining it.

### 16.5 The index map stays a plain `Dictionary`, not `KeyedDisjointSet`'s `HashMap`

`KeyedDisjointSet<TKey>` (§10.4) looks like the obvious precedent for translating between `TNode`
and a dense int index — it composes this repo's own `HashMap<TKey,int>` rather than the BCL
`Dictionary`, per §7's self-hosting rule. But that precedent is for a `DataStructures/`-tier
*reusable wrapper type*, meant to be constructed, held, and queried repeatedly by a caller.
`DenseDistanceMatrix.BuildIndex`'s map is `Algorithms/`-tier *local scratch state*, built and
discarded inside one call — the same bucket `ShortestPath.Distances` and `TopologicalSort.inDegree`
occupy, both of which use a plain BCL `Dictionary`. The distinction is lifetime and ownership, not
"is this a `TKey`-to-`int` map" — two structurally identical maps land in different buckets because
one is a public reusable type and the other is scratch state no caller ever sees.

### 16.6 The `MaxValue` sentinel: an overflow guard inside, a reserved weight outside

Every cell of the dense matrix must hold *some* `TWeight` value, so "not yet known" needs an actual
sentinel (`TWeight.MaxValue`) the way `ShortestPath.Distances`'s lazy, absence-means-unreached
dictionary never does (`BellmanFord` inherits that same absence convention for exactly this reason —
see its own doc comment). The sentinel has two consequences, and they are stated in two different
places:

- **Inside the walk, it is mechanics.** Adding two `TWeight.MaxValue` sentinels together would
  silently overflow under generic math, wrapping to a small value that reads as a real, very-wrong
  distance instead of "still unreached." No caller action can cause or prevent that, so the guard is
  an inline comment beside the relaxation in `AllPairsShortestPaths`, not a class-level doc comment.
- **At the boundary, it is a checked precondition.** A caller *can* pass an edge whose weight is
  `TWeight.MaxValue`, which the matrix could not tell from "unreached." `DenseDistanceMatrix` rejects
  it with an `ArgumentOutOfRangeException` while seeding, so the reservation is a law a caller can
  violate and the code checks, unlike `ShortestPath.cs`'s non-negative-edge-weight precondition.

## 17. The tiers above Operations: mechanisms, domain, solutions and harnesses

§1–§16 describe the library's first two tiers inside `DSAExperimentation/`: `DataStructures/`
(Representation + Topology) and `Algorithms/` (Operations). This section adds what is built on them
— the `Mechanisms/` and `Domain/` tiers in the same library, the LeetCode solutions, and the
harnesses that test and measure them — and the rules that keep each tier to its own job.

### 17.1 Why a solution is written once, in a tier of its own

A LeetCode coverage entry is a solution, a test that proves it and a benchmark that measures it.
Each has one home, and the solution's is tier 4, so that:

- **The test and the benchmark run the same code.** Both call the solution class, and neither
  carries its own copy of the algorithm: two copies drift, and the benchmark then measures a
  function the test never proved correct.
- **Every baseline is tested.** A benchmark's `[Benchmark(Baseline = true)]` arm — the textbook
  brute force the composed solution is supposed to beat — is a strategy of the solution class like
  any other (§17.3), so it is asserted against the problem's examples. An unasserted baseline makes
  the comparison meaningless, silently.
- **Domain code lives below the harnesses.** A type a solution needs is written once, in the tier
  §17.6 assigns it, and no harness declares a type that implements a structural contract (§17.7,
  §18.5).
- **A harness borrows nothing from another harness.** A test never imports another test folder's
  fixtures; what two harnesses share is library or solution code.

The axis classification §2 defines does not reach code that lives in a harness, which is why these
are tier rules rather than naming conventions.

### 17.2 The tiers

Bottom to top. The first three are peers, and every tier above them reads only those below it:

| Tier | Location | Holds |
| --- | --- | --- |
| 1. Representation + Topology | `DSAExperimentation/DataStructures/**` | §2's first two axes |
| 2. Operations | `DSAExperimentation/Algorithms/**` | §2's third axis |
| 3. Domain | `DSAExperimentation/Domain/**` | What a problem says about its world, pulled out of the problems that say it: models, rules, constants (§17.6) |
| Mechanisms | `DSAExperimentation/Mechanisms/**` | State run by a rule of its own that the caller observes — dropping what no caller named, or handing out what the caller never stored: the LRU and LFU caches, keys that expire, and an allocator of the smallest free number (§17.6b) |
| 4. LeetCode solutions | `DSAExperimentation.LeetCode.Solutions/**`, and at its root `DSAExperimentation.LeetCode/**` (own projects) | One folder per problem, every strategy for it; at the root, LeetCode's own notation and answer rules (§17.11) |
| 5. Harnesses | `DSAExperimentation.Tests/**` (the library's tiers), `DSAExperimentation.LeetCode.Solutions.Tests/**` and `DSAExperimentation.LeetCode.Tests/**` (tier 4), `DSAExperimentation.Benchmarks/**`, `DSAExperimentation.Benchmarks.Generators/**`, which writes the catalog benchmark classes at build time, and `DSAExperimentation.LeetCode.Harness/**`, what both harnesses share to run and judge the catalog's cases and to compare answers (§17.13) | Assertions and measurement, nothing else |
| LeetCode catalog | `DSAExperimentation.LeetCode.Catalog/**` (own project), written only by `DSAExperimentation.LeetCode.Catalog.Sync` | What LeetCode says about each covered problem, and every case a harness runs on it, as a checked-in snapshot (§17.12). It reads no tier, and only harnesses read it |

Domain is a peer of DataStructures and Algorithms, not a tier above them. The owner ruled it as
fundamental as they are (2026-11-01). A triangle's angles, a die's faces and a keypad's letters
hold whichever structure represents them and whichever algorithm computes with them (§17.6), so
nothing about Domain depends on the tiers below it. The three may therefore read one another, with
two limits:

- DataStructures still reads no algorithm, save the one edge §17.6a sanctions.
- Their folders may form no cycle across tiers. A folder that reads itself back through another
  tier has split one subject across two tiers, and neither half is a peer the other can read.

Mechanisms reads all three, and none of them reads a mechanism.

Mechanisms has a name and no number. It arrived after code across the repo had begun citing tiers
by number, so it sits between tiers 3 and 4 without renumbering either, and "tier 3", "tier 4" and
"tier 5" keep their meaning. ARCHITECTURE-HISTORY.md §17.6 records why tier 3 was retired once and
then restored with a wider remit.

Namespaces continue to mirror the physical path exactly (§13.1): insert `LeetCode.Solutions.` immediately
after `DSAExperimentation.` for the LeetCode project, then follow the rest of the path.

### 17.3 Tier 4: a solution class holds *every* strategy, not just the good one

`DSAExperimentation.LeetCode.Solutions/<Problem>/<Problem>Solution.cs` is a static class whose methods are
the strategies, named `<Operation>By<Strategy>` — `AddByBitStack`, `TryFindIndicesByBruteForce`,
`MinTurnsByReduceGraph`. The naive baseline is a first-class method there, not scaffolding hidden
in a benchmark, because:

- it is the thing the composed solution's whole justification is measured against, and
- putting it in tier 4 is what gets it under test.

The class name takes a `Solution` suffix rather than being bare `TwoSum`, which would collide with
its own enclosing namespace segment — the §10.3/§15.3 wrinkle, avoided by construction here instead
of aliased around once per problem.

A problem folder may hold more than the solution file. A witness that serves *one* problem's
strategies and nothing else belongs there too, not in the library:
`LeetCode/CountWaysToBuildRoomsInAnAntColony/` holds `RoomWaysAlgebra.cs` and
`RoomWaysPrecomputedFactorialAlgebra.cs`, two `IFoldAlgebra` witnesses that compute LC 1916's
multinomial and are meaningless anywhere else. What a problem says about its *world* is Domain code
instead (§17.6): the combination lock LC 752 opens is `Domain/Devices/Locks/`, and only its
configuration stays beside `OpenTheLockSolution`: `LockStart`, the combination its strategies and
its baseline both begin from, and `DeadendFilter`, the combinations it refuses.

**A problem folder reaches into another only along a named edge.** It may use anything in the
library — tiers 1–3 and Mechanisms — and anything at the root of this tier, the
`DSAExperimentation.LeetCode` project (`LeetCodeAnswer`, `LeetCodeIntervals`, `LeetCodeAdjacency`,
`LeetCodeWireFormat`). Another problem's folder is open to it in two cases only, each an entry in
`ProblemImportTests`' allow-list that says which case it is and why:

- **Delegation.** The two problems ask the same question under different constraints: Part I and
  Part II at different bounds, or a general problem on a special input (Permutations is
  Permutations II on distinct values). One of them owns every arm the two share. The other keeps its
  public strategies, because its test and benchmark call them, and each body becomes one call to
  the owner, narrowed by a cast where its answer type is smaller. Its test still asserts its own
  examples and its benchmark still times its own bound.
- **Subroutine.** One problem's answer runs another problem's algorithm as a step: Maximal Rectangle
  runs Largest Rectangle in Histogram on each row's histogram.

An owner imports no problem, so delegation never chains and never runs both ways. The owner is the
problem whose arms already answer the other's question with at most a narrowing cast, usually the
wider bound. Where two problems share only a baseline, the problem it was written for may own it.

Anything else two folders would share is general, and it goes below the problems rather than into
whichever folder needed it first: a structure or an algorithm to tiers 1–2 by §17.6's axes
(Andrew's monotone chain is `Algorithms/Geometry/ConvexHull`, and the time-dependent grid search
LC 2577 and LC 3341 share is `Algorithms/ShortestPaths/Grids/GridEarliestArrival`), a fact about
the world both problems describe to Domain (§17.6), and a LeetCode convention to this tier's root
(`LeetCodeIntervals.SortedByEnd` is the end order LC 435 and LC 452 both sweep). Two kinds of likeness stay where they are. When two problems share an algorithm that
is neither one's own answer, their composed arms share it from tiers 1–2 and their baselines each
restate it (§17.5). And an interface that coincides by LeetCode's design stays per problem: My
Calendar I and II both declare `bool Book`, but as two problems' APIs.

### 17.4 The hoisted-overload rule

A benchmark must charge input construction to `[GlobalSetup]`, not to the measured method; a test
wants to pass LeetCode's own input shape and nothing more. Both are served by giving a strategy two
overloads:

```csharp
public static int MinTurnsByReduceGraph(IEnumerable<string> deadends, string target) =>
    MinTurnsByReduceGraph(BuildLockGraph(deadends), target);   // LeetCode's shape

public static int MinTurnsByReduceGraph(StateGraph<string> graph, string target)       // pre-built
```

The prepared-input overload's parameter must be a type the LeetCode-shaped overload's arguments can
never bind to, so every call says which of the two it means. A domain object or one of this repo's
own containers meets that by construction: `Set<string>` is ideal precisely because it does not
implement `IEnumerable<T>`, which is why `Set<T>` has a bulk-seeding `Set(IEnumerable<Element>)`
constructor. A BCL type meets it too when it cannot collide. `List<int>[] adjacency`
beside `(int[][] edges, int[] coins)`, or a `HashSet<int>` beside an `int[]`, is as unambiguous as a
named type, and more than forty problems' hoisted overloads take one. What is never acceptable is a
prepared type the LeetCode-shaped parameter could also accept: `IEnumerable<string>` beside
`List<string>` resolves by specificity, silently, so a harness can time the overload it did not
mean. Name the prepared input when the BCL shape would collide, or when preparing it establishes
something a bare collection cannot state — `DislikeGraph`'s both-direction wiring, a sorted copy, a
labelled distance.

### 17.5 What a baseline arm may use

A baseline is the problem's specialized strategy: the simple form, written by hand and no more general
than the problem's constraints, the answer an interview asks for. It stays the benchmark's
`Baseline = true` arm, so the ratio column reads what the library's generality costs on that problem.
The owner set the rule this replaces aside on 2026-11-02, with the principle it now serves:

> we dont want duplicate implementations or data structure, the while point of doing the trie as part
> of the trie, is that you can explicitly see what property of the data structure distinguishes a trie
> from other trees and what is the same [...] if we want a pure trie implementation with no fluf and
> have it be hyperoptimized, then it can be used as a different strategy, but it must create a
> interface or contract for exactly the layer it wants to swap in for and what invariantes that layer
> guarantees, so that we can still satisfy that contract with the hyper modular design but also the
> specialized design and compare the type for objective benchmarks to see exactly where and how the
> specialized implementation beats the modularized one, even with static dispatch and monomorphization.

So a baseline may read what the library *states*, and may not use what it *implements*:

- **It may read facts.** Domain's named values, constants and the rules of a problem's world
  (`EnglishLetters.Size`, `DecimalDigits`, `Sudoku<ClassicSudoku>.CanPlace`), any public constant or
  enum of any tier (`AlgorithmConstants.HalvingFactor`), and LeetCode's own conventions at the tier root
  (`LeetCodeAnswer.None`, `LeetCodeModulus.Value`). Every arm reads a fact from its one source.
- **It may implement contracts.** An interface, a witness contract with static abstract members among
  them, states what a layer guarantees. A baseline that writes a layer by hand implements that layer's
  contract, so one contract test suite runs over it and the library's configuration alike, and a
  strategy benchmark compares the two. The suite is an abstract test class in
  `DSAExperimentation.Tests` whose abstract member builds the set or map it judges. The library's
  configurations derive from it there, and a baseline from the LeetCode tests, which reference the
  library's tests for that alone: LC 208's `ArrayNodeTrieTests` derives from `PrefixSetTests`.
- **It may not use an implementation.** A type or member of `DataStructures`, `Algorithms` or
  `Mechanisms` that is neither a contract nor a constant or enum - a container, an engine, a
  mechanism, a layout witness - is the library doing the work, and a baseline that used one would no
  longer be the simple form.
- **Its input is the caller's choice.** The container the harness hands it may be a repo type, and so
  may LeetCode's own I/O shapes, as before.

Laying LeetCode's input out is not part of the simple form either. A tier-4 input-layout helper -
`LeetCodeAdjacency`, which states once how an `(n, edges)` pair becomes one slot per node - may build
a baseline's input, provided every slot it fills is a BCL value: `NeighborIdSlots`' `List<int>`,
`WeightedNeighborSlots`' `List<(int To, TWeight Weight)>`, a `HashSet<int>`. Those shared slot rules
are struct witnesses (`IAdjacencySlots`), so a baseline that builds its lists inside the timed region
pays no delegate call per edge.

Until 2026-11-02 a baseline reached no library type at all, constants and Domain included, and kept a
private copy of every fact it needed. That rule needed two carve-outs (the owner's rulings of
2026-10-04 and 2026-11-01), copied Sudoku's and the lock's rules into their baselines, and kept a
hand-written layer from naming the contract it stood in for. Its 96 private copies in the solutions now read the
facts they copied, and `checks.json` no longer sets them aside from `check-duplicate-constant`.
`SharedValueCopyTests` holds every arm to one source per fact: it reports a constant declared in a
problem's folder that repeats a public constant of the library or of the LeetCode tier's root, unless
`AllowedCopies` says why it means something else. `LibraryValueCopies` decides what counts as a copy:
- A small integer or a single character means many things, so it counts only when its name shares
  a subject word with the shared constant's name or its type's. Role words such as Size, Count and
  Factor don't count as subjects.
- An enum member's number is its encoding, not a value, so enums are skipped on both sides.
- Role words such as No, Smallest and Largest don't count as subjects either: a -1 that marks a
  missing greater element is not a parent array's root.

`BaselineIsolationTests` holds every `Baseline = true` arm of a `ProblemSolutions` benchmark to this.
`BaselineTrace` follows each arm through the compiled code (`RepositoryCompilation`, §18.5), so a
name is read as what it binds to. The trace distinguishes the harness from the baseline:

- **The harness hands the input over, and is never charged.** The harness is the arm and the
  benchmark helpers it calls.
  - Each solution member it reaches is an entry. Overloads resolve as the compiler resolves them, so
    a LeetCode-shaped overload that builds the prepared input is not charged to an arm handed that
    input.
  - A benchmark field it reads is followed through every value the benchmark class assigns it, into
    the solution code and the types that value creates. An arm that calls a method on an object
    `[GlobalSetup]` built, behind an interface or not, starts where that object was made.
  - `Fixtures/` only lays input out, and is not walked.
- **The baseline is the solution code reached from an entry.** It is charged with every library
  implementation it binds to, in the sense above, and with nothing it may read:
  - calls, constructions, method groups, and field and property reads (a constant's read is a fact);
  - each expression's own type, so a `var` local counts like a named one;
  - type arguments, which is how a static witness is chosen;
  - a static field's initializer once the field is read, and a static constructor once its type is
    touched;
  - instance initializers whenever their type is created;
  - virtual and interface calls, resolved against the types created so far;
  - a created type's overrides of BCL members, such as a comparer `Array.Sort` calls back.

  Another problem's solution code is followed like this one's (§17.3). A strategy class gives up
  only what the baseline calls on it, never a sibling strategy's members.

Besides the facts and contracts above, what a baseline may meet without offence is its input:

- every type its entries' parameters name, and whatever a member of that input hands back, since
  the container the harness chose holds it;
- the types that stand for LeetCode's own I/O shapes: `BinaryTreeNode`, `SinglyLinkedListNode`,
  `QuadTreeNode`, and the `ICache` the LRU and LFU design problems are driven through.

A reach through code the baseline never runs goes in `AllowedReaches` with the reason. An entry the
trace no longer needs fails the suite.

Where every strategy of a problem composes the library, the problem gains a textbook arm to be its
baseline rather than losing a composition: `BinarySearchTreeIterator`, `ImplementQueueUsingStacks`,
`EvaluateReversePolishNotation` and `ImplementTrie` keep their repo structure beside a BCL one, `SortList` and `SubsetsII` beside a
BCL sort and a plain recursion, `EscapeALargeMaze` beside a BCL breadth-first walk. A benchmark that
compares library algorithms with one another keeps every one and adds the textbook arm beside them:
`ShortestPathAlgorithmBenchmarks` (LC 743's own priority-queue Dijkstra), LC 1334 (Floyd-Warshall
over a plain matrix) and LC 1916 (the post-order recursion its tree fold stands for), so the
library-against-library comparison each was written for still stands.

### 17.6 Tier 3: what a problem says about its world

**The rule: §2's three axes decide the tier, not how many problems share the type.**

- If a type's identity is a **Representation** or a **Topology** witness — generic over its
  contents, usable by anything with that shape — it is `DataStructures/`.
- If it is an **Operations**/strategy witness, it is `Algorithms/`.
- What a problem statement **fixes about its world** — a model (a lock, a board), a rule, a
  constant, a notation the world itself uses — is `Domain/`, filed by field and subject
  (`Domain/Devices/Locks`, `Domain/Mathematics/Geometry`) and tested once at the mirrored path under
  `DSAExperimentation.Tests/Domain/`. Not "a shape that happens to have been introduced for one
  problem"; a shape is tier 1 or 2 by the first two rules.

How many problems share a type is a *sharing* test, not a classification test: it says the type
must not be duplicated, and nothing about which tier it belongs to. "Edge = differs in exactly one
position" is a shape rule exactly like `GridTopology`'s "edge = 4 orthogonal neighbours", so the
Hamming graph is tier 1 however many problems use it; a weighted grid is the sibling of `Grid` and
sits beside it; a heuristic implements `IPathHeuristic`, an `Algorithms/` contract, and sits beside
`ZeroHeuristic`.

`HammingDistances` is the one piece of the Hamming family that cannot sit in `DataStructures/`: it
composes `Algorithms/Reducing`'s engine, so it would invert the tiers. It nests as a
structure-specific tier under `Algorithms/ShortestPaths/Hamming/`, mirroring
`Algorithms/ShortestPaths/Grids/GridShortestPath.cs` (§13.1's nesting rule). `HammingSearch` stays in
`DataStructures/Graph/Hamming/` because it depends on nothing above tier 1 — the same "Operations
rejoins Representation when it is hardwired to one" precedent (§13.5) that keeps
`InOrderTraversal` and `FindClosest` beside the trees they walk.

**What Domain is for.** It isolates a domain's concepts and logic from any one LeetCode problem:
what a problem says about its world is named, tested once and shared by every problem about the
same thing, the treatment algorithms already get. It is not an orthogonality rule, so a Domain type
may compose data structures, algorithms and mechanisms freely. Inside a problem, Domain holds the
*what* and the solution's strategies the *how*: a witness that serves one problem's strategies stays
in its folder (§17.3). LeetCode's own notation and answer conventions — its array encodings of
trees and lists, `LeetCodeAnswer`'s `-1` — stay at the LeetCode tier's root, because they are facts
about LeetCode rather than about any problem's world, and one problem's own spelling of its answer
(LC 1275's `"Draw"`) stays in that problem's folder.

**What qualifies, and in what form (the owner's rulings, 2026-11-01).**

- **Knowledge, not a puzzle's set-up.** Domain holds real-world or mathematical knowledge that is
  general beyond the problem: worth knowing and filing on its own, or usable by another problem,
  "even if just for intellectual categorization". Examples are a standard (the E.161 keypad), a real
  object (a die), a notation (folder paths), a real game's rule (Conway's Life, Zuma) and a
  mathematical fact. A rule a problem invented for itself stays in its folder, however complete a
  world it makes: LC 818's race car and LC 2241's banknotes are such rules.
- **The general rule, configured per problem.** Domain holds the general form, parameterized the
  way the data structures and algorithms are, with zero-cost static dispatch (§17.6a's witnesses).
  A problem's own variant is a configuration in its folder: LC 752's four wheels of `0`–`9`, LC
  773's 2x3 board. Locks, SlidingPuzzle and Modular predate this rule and are being brought to it
  (§17.10). A variant the real world commonly uses may be named in Domain as a preset,
  because other problems are likely to want it. Holding the general rule shows what the variants
  have in common, and which of their properties belong to only some of them.
- **Mathematics names what it assumes.** A mathematical fact states the geometry or topology it
  holds in. A triangle's angles make 180° only on the flat plane: on a surface of constant
  curvature K, Gauss–Bonnet gives π + K·Area. So the flat case is named as flat, and the general
  form is written only where it is exact. Within that, each subject is built in its most general
  exact form (§17.10).
- **Mathematics against algorithms.** In the owner's words: algorithms "are swappable and are
  chosen based on the input data shape and usage constrains while the math facts are the things
  those algorithms exist to compute or solve or use, and the algorithms may be useful in cases
  outside any particular math problem or fact". An algorithm serves many mathematical problems, and
  problems outside mathematics, so it is not confined to one mathematical category. A mathematical
  problem can be solved by many algorithms, and "the truth or invariants still holds among all". So
  a procedure with interchangeable implementations of different cost is `Algorithms/`, and what
  every such implementation must agree on is `Domain/`.

**Where the domain models are today:**

| Folder | Holds | What it fixes |
| --- | --- | --- |
| `Domain/Computing/Binary/` | `BitWidths` | The bits in a 32-bit integer, `int` or `uint`: the depth of a bit trie's walk and the places a bit count or a bit reversal runs over. |
| `Domain/Computing/Filesystem/` | `FolderPaths`, `WindowsPaths` | The folder-path notation, `/` between folder names, written before each, the root first; and Windows', `\` between names with the drive first. What lying inside a folder means is `NamePath`'s rule, configured by the notation. |
| `Domain/Computing/Networking/` | `DomainNames` | Domain names: labels joined by `.`, the root written last, so `mail.example.com` lies inside `example.com` and `notexample.com` does not. |
| `Domain/Computing/Paths/` | `IPathNotation`, `NamePath` | A path through a tree of names, in a notation that fixes the separator and which end the root is written at: a filesystem writes it first ("/a/b/c"), a domain name last ("c.b.a"). One structure written in opposite directions, so one rule says what lies inside what: the path, then the separator, then more, so `/a/bc` is not inside `/a/b`. That is a name's descendants in the tree a trie of names holds. The test reads the separator in place rather than building "<folder>/", and the path is its own type, so the two paths cannot be handed over the wrong way round. Folder paths, Windows paths and domain names ship as presets (§17.10's most general exact form). |
| `Domain/Devices/Compass/` | `CompassHeading`, `CompassStep`, `CompassPoint`, `CompassRules` | Four headings in clockwise order from North, a quarter turn either way, and a unit step along the heading: a concrete instance, like the lock, of a cyclic group no problem needs in general. A step is given as how far it goes east and north, because the problems disagree on axes (LC 1041 calls north its row, LC 885 grows rows southward); each maps the two onto its own coordinates. Each step is named (`Northward` is (0, 1)), each heading and its step are a named `CompassPoint`, and the step table is built by placing each point's step at its own heading, so the order the points are listed in cannot misplace one (the owner's ruling, 2026-11-02). The headings are the square lattice's turning cycle, `GridDirections.Clockwise`, read with north up: each named step is `CompassStep.From` the lattice's own (`Northward` is `GridStep.Up`, since the grid's rows grow downward), a heading's number is its step's index in the cycle, and a turn is the lattice's turn, so the compass and the grid hold the four unit steps once between them (the owner's ruling, 2026-11-03). |
| `Domain/Devices/Locks/` | `ILockSpec<TWheel>`, `CombinationLock<TSpec, TWheel>`, `LockTurns<TSpec, TWheel>`, `FourDigitLock` | A rotary lock is fixed by its wheel count and the alphabet each wheel carries: a bike lock's digits, a word lock's letters. A wheel clicks one slot of its alphabet up or down and wraps, and a combination is numbered as a number written in base the alphabet's size with the wheel's symbols as its digits, first wheel first. `LockTurns` presents the lock as a state space, and the graph of it is tier 1's `StateGraph`; the earlier `LockGraph` was that graph built for one lock. `FourDigitLock` (4 wheels of `DecimalDigits`) is the common preset LC 752 opens, and its deadends are LC 752's filter, not the lock's. A word lock's letter wheels are proven by a test-only build. |
| `Domain/Devices/Telephone/` | `PhoneKey`, `KeyPress`, `IKeypadLayout`, `Keypad<TLayout>`, `E161Keypad`, `ClassicNorthAmericanKeypad` | A keypad layout is its ten keys, each a named value carrying its digit and the letters printed on it in order, so 'e' is key 3's second letter. ITU-T E.161 puts abc on 2 through wxyz on 9 and nothing on 0 and 1; the North American keypad before it left out Q and Z. `Keypad<TLayout>` reads a layout both ways: the key a digit names, and the key and press count that type a letter by multi-tap, built once from the keys into a table over the 26 English letters, so a letter is found in either case. A key's letters are also spelled once as a string, which LC 17's search reads at every node without allocating. |
| `Domain/Games/Chess/` | `ChessRules`, `ChessPiece`, `PieceMove`, `MoveConditions`, `ChessSide`, `BetzaNotation` | The 8x8 board, and each piece as a named value carrying its moves, each a `PieceMove` as Betza notation writes one: its steps, its reach (1 for a leaper, a number for a limited rider, none for a rider) and when it may be made (to an empty square, as a capture, only as the first move). A rook rides its row and column, a bishop its diagonals, a queen both as one move, a king and a knight leap once, and a pawn moves forward one, captures diagonally forward and rides two forward as its first move, facing its `ChessSide`. `BetzaNotation` reads the notation for building and checking pieces, never on a search's path; each preset is a table fixed in code, which the tests hold to its Betza string. The steps are `GridDirections`' tables, facts of the square lattice (`Domain/Mathematics/Lattices`) named for their geometry, not for chess (`Octilinear`, `LShaped`), in their order, since LC 1222's arms are compared answer for answer. The knight problems are played on boards other than chess's, so they read the `LShaped` table rather than the piece. |
| `Domain/Games/Dice/` | `IDie`, `DieFace`, `Die<TDie>` and presets | A die in its most general form is a list of faces, each a number and a weight: a common die's faces are a fair run (1 to 6, 1 to 20), a ten-sided die often runs 0 to 9, a percentile die 00 to 90 in tens, a Fudge die shows -1, 0 and +1 twice each, a doubling cube the powers of two to 64, and a loaded die weighs its faces unequally. Faces may repeat a value, so a die is its faces, not its values; Sicherman's pair sums exactly like two six-sided dice. A face's probability is its weight over the total, which is the fact; drawing a roll is a sampler's, `Algorithms/Sampling` (§17.6, mathematics against algorithms). Which roll may follow which is LC 2318's game. |
| `Domain/Games/Life/` | `ILifeLikeRule`, `LifeLike<TRule>`, `ConwayLife`, `HighLife` | A Life-like automaton's rule is the neighbour counts at which a dead cell is born and a live one survives, written as Life's community writes it: Conway's Life is B3/S23 and HighLife B36/S23, named as presets. `LifeLike` reads a rulestring once into two masks over 0 to 8, constants once the rule is initialized, so a rule's test is a shift of constants, one predicate per cell state, and no caller passes a flag. The 0/1 board and which cells are neighbours are LC 289's. |
| `Domain/Games/SlidingPuzzle/` | `IPuzzleBoard`, `SlidingPuzzle<TBoard>`, `PuzzleSlides<TBoard>`, `EightPuzzle`, `FifteenPuzzle` | A puzzle is fixed by its board's rows and columns. A slide trades the empty cell with a neighbour; solved is the tiles in order with the empty cell last; and the parity law says which arrangements can be solved. On an odd width the inversions must be even; on an even width the inversions plus the empty cell's row from the bottom must be odd. `PuzzleSlides` presents the puzzle as a state space for tier 1's `StateGraph`, enumerable for boards of up to 12 cells. `EightPuzzle` and `FifteenPuzzle` are the common presets; LC 773's 2x3 `TwoByThreeBoard` sits in its own folder. The test oracle: slides from solved reach exactly the 360 of the 720 2x3 arrangements the law calls solvable. |
| `Domain/Games/Sudoku/` | `ISudokuGrid`, `Sudoku<TGrid>`, `ClassicSudoku`, `SudokuDigits` | Sudoku of order n: boxes of n x n cells, a board n² a side holding n² symbols, and the rule that no symbol repeats in a row, column or box — what LC 36 and LC 37 both say about the board for n = 3. The board's side is derived from the box's rather than stated beside it. `ClassicSudoku` is the 9x9 preset, with '.' for an empty cell, and `SudokuDigits` its symbols '1'–'9' as an alphabet; the 4x4 children's grid is proven by a test-only grid of order 2. Which cell a solver fills next and how a validator tracks what it has seen stay with each strategy. |
| `Domain/Games/TicTacToe/` | `TicTacToeRules`, `TicTacToePlayer`, `TicTacToeOutcome` | The 3x3 board, X moving first and O second, the game's own names, the two diagonals and how a game stands: a completed line wins even on the last square, otherwise a full board is a draw and a short one pending. LC 1275's spelling of that answer (`"A"` for X, `"B"` for O, `"Draw"`, …) and the strategies' ±1 marks stay in its folder. |
| `Domain/Games/Zuma/` | `IZumaRule`, `ZumaRow<TRule>`, `ClassicZuma` | A group of touching balls of one colour at least the rule's length vanishes, cascading; Zuma's own length is three. Each pass removes every such group at once, which is the game's one-at-a-time cascade while the row holds at most one, as after one ball goes into a settled row. The colour letters are LeetCode's, and nothing reads them. |
| `Domain/Language/` | `CaseFolded`, `Multigraph`, `IMultigraphSet`, `MultigraphSet<TSet>`, `NumberWord`, `TimeUnitWord` | A cased writing system's letters with case set aside: a letter and its capital take one slot, and a letter's lowercase form stands for it. That holds only where case pairs letter for letter, as English's and Russian's do; German's ß capitalizes to two letters and Turkish pairs i with İ and ı with I, so the type says so rather than claiming every script. A `Multigraph` is letters that together spell one sound, a digraph or a trigraph, which many alphabetic languages have (English's sh, German's sch); a language's set of them is a preset, and `MultigraphSet` reads it once into a frozen set, compared with case set aside and looked up by span, as `LetterSet` reads a letter set into a mask. A test-only German set proves the table is not English's alone. A `NumberWord` is a number and the word a language names it with, so a table of words places each word at its own number. A `TimeUnitWord` is a unit of time, given by its length, and the word a language names it with, so a table of words places each word at its own unit. |
| `Domain/Language/English/` | `EnglishLowercase`, `EnglishUppercase`, `EnglishLetters`, `EnglishNumberWords`, `EnglishTimeUnitWords` | The 26 letters in each case as presets of tier 1's `IAlphabet` (§19.6), each a run of character codes whose two ends are constants, and the two folded into `EnglishLetters`. English numbers its letters from one, A as 1 through Z as 26: a notation people use, and English's rather than every alphabet's, since Greek and Hebrew numerals give kappa and kaf the value 20, so `NumberOf` is `EnglishLetters`' and not the contract's. The written vowels and consonants are English's spelling, in `Orthography` beside its digraphs; the letters themselves are English's configuration of the Latin script, its basic letters with no mark, so they stay here rather than under `Scripts/Latin`. English's number words are named values too: zero to nineteen, each with a word of its own, the tens, a hundred, and the short scale's word for each power of a thousand through quintillion, all lowercase as written within a sentence. Only the words are stated. How they compose follows how a number's digits are grouped, which the short scale, the long scale and Indian English's lakh and crore do differently, so composing them is the caller's for now, and LC 273's capitals are its own notation. English's words for the units of time, the microsecond to the week, are named values too, each carrying its unit's length from `TimeUnit`. The unit and its length are `Science/Time`'s, and English only names it, so `TimeUnit` carries no name; the owner's TODO of 2026-11-04 asked for `EnglishNumberWords`' pattern. LC 1348's frequency names are its own notation and stay in its folder. |
| `Domain/Language/English/Orthography/` | `EnglishVowels`, `EnglishConsonants`, `EnglishMultigraphs` | English's spelling. The five written vowels are a letter set over the folded letters, so 'A' is a vowel as 'a' is. The consonants are the alphabet's other letters, read from the vowels, so each letter is in exactly one of the two sets, and y is a consonant: that it spells a vowel sound in some words is a fact of English's sounds (`Phonology`). How a strategy counts or finds them stays with it. The digraphs ch, sh, th, ph, ng and ck and the trigraph tch are named values, each spelled with consonant letters; which sound each spells is English's phonology. |
| `Domain/Language/English/Phonology/` | `ReceivedPronunciation`, `LetterY` | English's sounds, as a configuration of the IPA (`Phonetics`). Received Pronunciation, the accent British dictionaries transcribe, has 44 phonemes in their traditional transcription: 12 single vowels, 8 diphthongs and 24 consonants. Each vowel is a named value called by its lexical set (Wells), the keyword that names it in every accent, so KIT is ɪ here; another accent, such as General American with its r-coloured vowels, would be another configuration. Two conventions are the IPA's rather than the dictionaries': r is ɹ, since the IPA's r is a trill, and the affricates carry the tie bar, t͡ʃ and d͡ʒ. `LetterY` states y's two roles: a consonant letter that spells the consonant j ("yes"), the KIT vowel ("gym") and the PRICE diphthong ("my"). |
| `Domain/Language/Phonetics/` | `IPhonemeInventory`, `PhonemicTranscription<TInventory>`; `Vowels/`: `Vowel`, `VowelHeight`, `VowelBackness`, `Rounding`, `VowelLength`, `Monophthong`, `Diphthong`, `IpaVowels`; `Consonants/`: `Consonant`, `Voicing`, `Place`, `Manner`, `IpaConsonants` | The International Phonetic Alphabet, which no language owns. A vowel is a cell of the 2020 chart, its height, backness and rounding, and the 28 symbols are named values named as the IPA names them; ə and ɐ leave rounding unspecified, as the chart does. A consonant is its voicing, place and manner: the pulmonic chart's 59, the chart's other symbols (w, ʍ, ɥ, the epiglottals, ɕ, ʑ, ɺ), and the affricates t͡s, d͡z, t͡ʃ and d͡ʒ, each built from its stop and fricative so its symbol cannot disagree with theirs. The voiced velar plosive is the IPA's ɡ, U+0261, not a keyboard's g. A language's single vowel is a quality and a length (ː long, ˑ half-long), and a diphthong one sound gliding between two qualities. An inventory is what a language configures: its monophthongs, diphthongs and consonants. A phonemic transcription reads back into an inventory's phonemes by taking the longest at each place, so /maɪ/ is m and aɪ; a symbol the inventory lacks is refused. The non-pulmonic consonants need an airstream axis, and ɧ has no single place on the chart, so neither is stated. |
| `Domain/Language/Scripts/Latin/` | `Diacritic`, `DecomposedLetter`, `LatinScript`, `IAccentedAlphabet`, `AccentedLetters<TAlphabet>`, `SpanishAlphabet`, `FrenchAlphabet`, `GermanAlphabet` | The Latin script's letters as basic letters with diacritics: accented letters are another alphabet's, not English's (the owner's ruling, 2026-11-05). The 17 marks, grave to ogonek, are named values held as their Unicode combining characters; the diaeresis and German's umlaut are one mark, U+0308. `LatinScript` takes a letter apart through Unicode's canonical decomposition (`string.Normalize` with `NormalizationForm.FormD`), so é is e and the acute, and ế e, the circumflex and the acute, in canonical order. ø and ł (a stroke), ß, æ and œ are letters of their own that do not decompose, and neither do ǿ or Greek ά, whose bases are no basic Latin letter. An accented alphabet lists its accented letters and its other letters: Spanish á é í ó ú ü ñ, French fourteen with æ and œ, German ä ö ü with ß. `AccentedLetters` decomposes each and its capital once into a table, so folding "café" to "cafe" is a lookup; it is the only path to Unicode's decomposition, and English's alphabets, runs of character codes, never take it. Whether a language counts ñ as a letter of its own or á as a marked a is its collation, and is not stated. |
| `Domain/Mathematics/Algebra/` | `IMonoid`, `IAbelianGroup`, `IIntegerModule`, `Addition`, `Minimum`, `Maximum`, `BitwiseXor` | The laws a range structure needs of its elements, as the mathematics names them: a monoid, an associative combine with an identity; an abelian group, a monoid whose combine commutes and can be undone; and an integer module, an abelian group whose integer multiple is a member of its own, held to an O(1) cost. Addition, minimum, maximum and bitwise exclusive or are presets, each at the deepest link its arithmetic reaches: minimum and maximum have no inverse, so a Fenwick tree over either does not compile. Each is a struct witness with static members, so a structure generic over one compiles its own arithmetic (§11.4). |
| `Domain/Mathematics/Geometry/` | `EuclideanPolygons`, `EuclideanPlane`, `ConstantCurvatureSurface`, `AngleUnits` | Mathematical facts no data structure or algorithm represents, which the owner ruled are Domain's, each named for the geometry it holds in. On the flat plane, sides close into a polygon when the longest is shorter than the rest together, which for three sides is the triangle inequality, and a polygon has at least three sides; a sphere has two-sided lunes. A triangle's angles make 180 degrees on the flat plane, a constant a caller pays nothing for, and on a surface of constant curvature K, Gauss–Bonnet's π + K·Area, which is exact. A turn is 360 degrees by the degree's definition. Sorting, scanning and the law of cosines stay with the strategies, and `Algorithms/Geometry` keeps operations such as the convex hull. |
| `Domain/Mathematics/Lattices/` | `IPlanarLatticeStep<TSelf>`, `LatticeSymmetry`, `GridStep`, `GridDirections` | The square lattice a grid's cells sit on: a step as how far it goes down the rows and across the columns, the steps named (`Up` is (-1, 0), `UpUpLeft` an L-shaped jump), and the neighbour tables made of them, each in a fixed order that is part of the contract. A planar lattice's steps form a group under addition, and its symmetries are turns by its smallest rotation, which can only be of order 2, 3, 4 or 6, and a mirror. `IPlanarLatticeStep` puts that algebra on the step type, so a generic method infers the lattice from the step it is handed. A set of steps agrees with the lattice when every symmetry maps it onto itself, which makes it a union of the orbits `LatticeSymmetry` lists. Each table is one: `Orthogonal` is `Right`'s orbit and `LShaped` any jump's. The tables keep their own literal order, and the orbits check them as sets. `Clockwise` (up, right, down, left) is the turning cycle a spiral or a heading moves along, numbered as `CompassHeading` is, and `TurnRight`, `TurnLeft` and `OppositeOrthogonal` are the index rules a hot loop turns by. `WithinManhattan` and `WithinChebyshev` are the von Neumann and Moore neighbourhoods of any range. A test-only hexagonal step proves the contract is not the square's alone: a hexagonal grid's cells sit on the triangular lattice A2, which turns in sixths. A triangular grid's cells are not a lattice, since their up and down triangles are two orbits and a cell's steps depend on which it is, so it is not offered. The owner ruled on 2026-11-03 that these are "more general than any one game or domain", "all useful values that agree with the topology of the grid", and "generalizable to satisfy a wide range of topologies", so they left tier 1, where a baseline could not read them, and chess, the compass and the grid engines all read them here. |
| `Domain/Mathematics/Surfaces/` | `ISurface`, `GluedRectangle<TRows, TCols>`, `MirroredAxes`, `BoundedRectangle`, `Cylinder`, `Torus`, `MobiusStrip`, `KleinBottle`; `Gluings/`: `IEdgeGluing`, `Unglued`, `Straight`, `Twisted` | The board's topology, as the lattice is its local structure: a rectangle of cells with each pair of opposite edges left open, a boundary no step crosses, or glued, so a step off one edge comes back in at the other, the same way round (straight) or the opposite way (twisted). The choice is per pair, so `GluedRectangle` composes two gluings and the presets name the common ones: nothing glued is the rectangle LeetCode's grids are, a disk; one pair straight is the cylinder, both the torus; one pair twisted is the Mobius strip, and with the other straight, the Klein bottle. A glued edge is crossed as many times as a step needs and the gluing counts the crossings, so a knight on a board two cells wide lands right. Crossing a twisted pair an odd number of times mirrors the other axis, and `TryLand` reports it (`MirroredAxes`): a set of steps closed under the mirror, such as the orthogonal ones, is unchanged, while an ordered one, such as a heading or `Clockwise`, has its left and right swapped. `GridSize.HasCell` is the bounded rectangle's rule, one unsigned comparison per axis, and the JIT inlines the whole chain. A grid without defects is flat, so by Gauss-Bonnet its closed surfaces have Euler characteristic 0, the torus and the Klein bottle. Two twisted pairs would make the projective plane, which needs cone points, and a diagonal step out of a corner comes back to its own cell, which a test shows; the sphere needs them too, as a cube's corners are. Neither is offered. The tests read each preset back as a cell complex built from where its steps land, and check its Euler characteristic and boundary circles: 1 and one for the rectangle, 0 and two for the cylinder, 0 and one for the Mobius strip, 0 and none for the torus and the Klein bottle, which orientation tells apart, since once round the Klein bottle a walker comes back mirrored. |
| `Domain/Mathematics/Modular/` | `IModulus`, `IPrimeModulus` | What a modulus is, as mathematics rather than LeetCode's convention: the residues 0 to m - 1, the bound that keeps the product of two residues inside a long, and for a prime, Fermat's inverse a^(p - 2). An algorithm over them is swappable, so `ModularInverse.ByFermat` and `FactorialTable<TModulus>` are `Algorithms/NumberTheory`'s, beside `ModularPower`. A modulus that is a compile-time constant is a struct witness the JIT folds. LeetCode's own prime, 10^9 + 7, is `LeetCodeModulus` at the LeetCode tier's root, since it is LeetCode's choice; its tests check it is prime and within the bound. |
| `Domain/Mathematics/Numerals/` | `DecimalDigits`, `IPartsPerNotation`, `Percent`, `Permille`, `Permyriad`, `PartsPerMillion` | The ten decimal digits, '0' to '9', as an alphabet (§19.6): a digit's slot is its value, since the digits are one run of character codes. A lock's wheels carry them and a numeral is written in them.`Zero` and `Nine` name the two digits a wheel turns between. A proportion written as parts of a fixed whole is a notation too: per cent counts hundredths, per mille thousandths, per myriad (finance's basis point) ten-thousandths and parts per million millionths, and the whole is each one's constant. Taking a share of an amount is not stated, because in floating point dividing by the whole last and multiplying by a rate worked out once round differently, and which a problem does is its arms' choice. |
| `Domain/Mathematics/NumberTheory/` | `Primes` | The smallest prime, 2, and the only even one: where a search for primes begins and a sieve's first crossing-out. |
| `Domain/Science/Dna/` | `DnaBase`, `DnaBases` | The four bases, adenine A, cytosine C, guanine G and thymine T, each a named value carrying its name and letter. `DnaBases.All` lists them once, and its letters, names, spelled-out string and alphabet slots are all read from that one list. As an alphabet they are A to T with gaps, so their slots come from a `LetterTable` (§19.6). They were tier 1's `StandardAlphabets.Dna`, a fact about DNA in a graph's folder. A number for each base is not DNA's: LC 187's 2-bit order is its own strategy. |
| `Domain/Science/Time/` | `ITimeUnit`, `TimeUnit` | A unit of time as a witness carrying its length: the microsecond, millisecond, second, minute, hour, day and week, each a struct in `TimeUnit`. The lengths are .NET's `TimeSpan` constants, exact to its 100-nanosecond tick, so the platform stays their one source, and a week's seven days is the one count stated here. The day is the 86,400 seconds the SI accepts, not a civil day, which daylight saving makes 23 or 25 hours and a leap second 86,401 seconds. A month and a year are a calendar's units, not fixed lengths, so `Calendars/` states them. A nanosecond is a hundredth of a tick, so it is not offered. A unit of any other fixed length is a struct of the caller's that implements `ITimeUnit`. Each length is a constant, so a caller that knows its units divides one by another and the compiler writes the count, and `TimeUnit.Count<TUnit, TIn>` gives the same count to code generic over its units. A unit's name is a language's, not the unit's, so English's words for the units are `Language/English`'s `EnglishTimeUnitWords`. LC 1348's frequency names are its own notation, mapped onto the units in its folder. |
| `Domain/Science/Time/Calendars/` | `ICalendar`, `Calendar<TCalendar>`, `GregorianCalendar`, `JulianCalendar`, `CalendarMonth`, `CalendarMonths`, `CalendarDate`, `RataDie` | The Julian family of calendars: the twelve Roman months, a leap day added to February, and one leap rule of three cycles. A year is a leap year when the leap-year cycle divides it, unless the century cycle does and the leap-century cycle does not. The Gregorian calendar is 4, 100 and 400. The Julian calendar is the same rule with every century year kept, 4, 100 and 100, and the Gregorian reads its 4 and 100 from the Julian's, as the reform kept them. Each cycle is a constant and a static abstract member of a struct witness, so a caller's calendar folds to constants. What follows from them is derived, never restated. The common year's 365 days are the twelve months' constant lengths added up. A cycle's days, 146,097 in the Gregorian calendar's 400 years, are the rule's, and `DaysPerCycle` compiles to that number. The Julian calendar's equal cycles are tested first, so its leap test compiles to one test of the year's low two bits. `Calendar<TCalendar>` gives leap years, month and year lengths, a date's day of the year, its day on the count every calendar shares, the days between two dates and the weekday. The count is Reingold and Dershowitz's rata die, whose day 1 is the Gregorian 1 January of year 1, a Monday, so a weekday is read from that day; the Julian calendar's year 1 begins on day -1. Calendars whose months or leap rule differ, such as the Hebrew, the Islamic, or the Revised Julian with its 900-year rule, are not of this family and are not offered. The tests count the 146,097 days year by year from the rule, check the reform's Thursday 4 October 1582 (Julian) followed by Friday 15 October (Gregorian), and read every day of a whole cycle of each calendar against .NET's `DateOnly` and `System.Globalization.JulianCalendar`. No solution reads them yet: LC 1154, 1185, 1360, 1507 and 2409 are not covered. |

Their tests mirror them in `DSAExperimentation.Tests/Domain/{Computing/Binary,Computing/Filesystem,Computing/Paths,Devices/Compass,Devices/Locks,Devices/Telephone,Games/Chess,Games/Dice,Games/Life,Games/SlidingPuzzle,Games/Sudoku,Games/TicTacToe,Games/Zuma,Language,Language/English,Language/English/Orthography,Language/English/Phonology,Language/Phonetics,Language/Phonetics/Consonants,Language/Phonetics/Vowels,Language/Scripts/Latin,Mathematics/Algebra,Mathematics/Geometry,Mathematics/Lattices,Mathematics/NumberTheory,Mathematics/Numerals,Mathematics/Surfaces,Mathematics/Surfaces/Gluings,Science/Dna,Science/Time,Science/Time/Calendars}/`. The
domain knowledge still written inside individual problem folders moves here problem by problem
(§17.10).

### 17.6a How the tier order is enforced

A constant or check that more than one tier reads lives at the lowest tier that reads it, whatever
its name: `AlgorithmConstants` sits in `DataStructures/` because data structures read it. Among
the three peers none is lowest, so such a value lives where its meaning does. A fact about the world
lives in Domain, whichever peer reads it.

Enforcement comes from the following, chosen deliberately over a project-per-tier split:

- **`LeetCode` is its own project** (`DSAExperimentation.LeetCode.Solutions`, referencing
  `DSAExperimentation`). Nothing below it can depend on a solution, because it cannot see one.
  This seam is worth a project because it is strictly one-way and keeps 1,105 problem classes out
  of the framework assembly.
- **Solutions see only the library's public types.** `DSAExperimentation` grants
  `InternalsVisibleTo` to its three test projects and nothing else, so the compiler holds every
  solution and benchmark to the library's API. What is public is what a caller composes:
  structures, algorithms, contracts, witnesses, algebras and result values. What stays `internal`
  is another type's own mechanics: each structure's storage (`HeapArray`, `HashMapStorage`,
  `DisjointSetForest`, `FenwickArray`, …), shared bounds checks and tuning constants, and the walk
  and fold engines with their visit guards. A solution that wants one of those gets a library
  feature instead (`DisjointSet.MergeInto` replaced a reach into `DisjointSetForest`), or a type
  made public on purpose with the reason in its doc comment (`HeapArrayIndex`).
  `Tests/Architecture/InternalsVisibilityTests.cs` fails if the library grants its internals to
  anything but a test assembly.
- **The test projects split the same way.** `DSAExperimentation.Tests` tests the library's tiers
  and does not reference `DSAExperimentation.LeetCode.Solutions`; every solution-facing test — the per-problem
  `<Problem>SolutionTests` at `DSAExperimentation.LeetCode.Solutions.Tests/<Problem>/`, the seam tests beside
  them, the LeetCode catalog — lives in `DSAExperimentation.LeetCode.Solutions.Tests`. A core test that
  reached for a solution would not compile.
- **`Tests/Architecture/LayeringTests.cs` polices the rest.** DataStructures, Algorithms,
  Mechanisms and Domain stay in one assembly and one project, and the test fails on any file of that
  project outside their four folders, so nothing can grow inside the library as a folder no rule
  judges.

Splitting those four apart was considered and rejected for two reasons. First, several designs
depend on single-assembly encapsulation: the walk and fold engines, their visit guards and each
structure's storage stay `internal` behind public entry points. `IVisitGuard`'s own doc comment
argues that *"nothing outside this assembly (its test projects aside) can reach one…
TrackedVisitGuard's constructor stays internal too, so outside code cannot construct a working
one"*, which stops being true the moment its consumers are a different assembly. Splitting would
force that surface public or thread a dozen `InternalsVisibleTo` attributes through the tree.

Second, one upward edge is **intended**: `IntervalSet` (a Representation) composes
`BinarySearch.LowerBound` through a private `IRandomAccessSequence` view rather than carrying a
second bisection loop, and its doc comment defends that as sanctioned reuse. A `ProjectReference`
cannot express "this one edge, for this reason"; the test's allow-list can. An entry names the file
*and* the one namespace it may reach — `IntervalSet.cs` → `DSAExperimentation.Algorithms.Searching`
— so any other upward reference from that file still fails. A companion test fails once the
file's code no longer names that namespace, so an entry cannot outlive its edge.

The test reads each file's code, with comments and string literals blanked, not only its `using`
lines. A fully qualified `DSAExperimentation.Algorithms.X`, a `using static` or alias, and a
partially qualified `Algorithms.X` all count as references. The partial form needs no `using`
because it resolves the same way inside any `DSAExperimentation` namespace. Global usings are
judged more strictly, because they put a namespace in scope for every tier at once. That covers a
`global using` directive in any of the project's files and a `<Using>` item in any build file
MSBuild reads for the project: its project file, a `Directory.Build` file, or
`Directory.Packages.props`, which NuGet imports into every project whether or not central package
management is on. A global using may name no tier, and no allow-list entry can sanction one. It
would put that tier in scope for every file with nothing in any file's code to show the read, so
neither the tier rule nor the cycle check below could see it. All of this reads a tier off the
namespace a name is written in, so every file must also declare its namespace inside its own tier:
a type an `Algorithms/` file declared in a `DataStructures` namespace, in the root namespace or in
none would reach `DataStructures` files by its simple name, with no tier written anywhere for the
test to read.

The peers' freedom to read one another is checked as a graph of folders. `FolderGraph` builds it
from the same references:

- Each name resolves to the deepest folder holding a file that its path reaches.
- Each folder also counts as reading every enclosing folder that holds a file. C# lets a file name
  an enclosing namespace's types with no `using`, so no text would show that read.

`FolderCycles` then fails each read across tiers whose folder reaches its reader again, and
states the cycle. A cycle inside one tier is left alone. Algorithms' walks, traversals and
reductions form one today, and it splits no subject across tiers.

### 17.6b Mechanisms: a rule of their own that the caller observes

**The line.** A *data structure* holds exactly what its caller put in and has not taken out. Every
element enters through a store the caller makes, and leaves only through a caller operation that
names it, by key or by position: a pop, a removal, a truncation after a cursor. Constraints and
invariants injected into a general structure make specialized ones, the way Graph narrows to DAG and
then to Tree (§3). They may refuse or clamp an operation, or fix which held element an access
reaches, but they never add an element or drop one. A *mechanism* applies a rule of its own whose
effect the caller observes: it drops something no caller operation named — evicted at a capacity,
lapsed past a time-to-live — or it hands out a value the caller never put in.

`LruCache` and `LfuCache` are mechanisms: `Set(k3)` names k3 and drops k1. With `ICache`, the
contract LeetCode's two cache problems are driven through, they make up `Mechanisms/Cache`.
`ExpiringSet` is one too: a key lapses when its time-to-live runs out, and no caller operation
names that. Time is passed in rather than read (§17.6c), and with a fixed time-to-live the order
lives start in is the order they end in, so a queue in that order lets lapsed keys go in amortized
O(1). `SmallestFreeAllocator` is the line's other side: `Allocate` hands out the smallest number not
currently held, a value the caller never stored, and `TryRelease` takes back only a number it holds.
A counter stands for every number never issued, so only released numbers take space: a min-heap
below the counter, and a flag per number up to the largest released, which turns away a number not
held. LC 1845's seats and LC 2336's popped numbers are both this allocator from 1.

Deciding is not enough. A heap's order decides which value a pop returns, and `Heap` is a data
structure. Nor is growth: a `DynamicArray`, `HashMap` or `Deque` that grows changes only what an
operation costs. A deque that refuses a push once it is full (`BoundedDeque`, §20) is a data
structure whose capacity is an injected invariant; one that overwrote its oldest element
instead would be evicting, and so a mechanism. `Memoizer` is not one either — its cache is
discarded when the call returns — and nor is `MonotonicDeque`, whose dominance drop is its window
order deciding what the front can reach, with the caller naming the window through `EvictBefore`.

Mechanisms sits above the three peers (§17.2). A mechanism may compose any structure or algorithm
and read any of Domain's facts. A domain type holds no mechanism: Domain states what is true of a
world whatever machinery runs it, and a mechanism is machinery. No domain type held one when Domain
became a peer (2026-11-02). Design problems that bury a mechanism under their rules
contribute it here when the problem is split (§17.6, §17.10), as LC 1797 gave up its expiry and LC
1845 and LC 2336 their allocator. There is no tier for code that holds part of the machine itself;
§17.6c keeps the tiers below the harness free of it.

### 17.6c Nothing below the harness touches the machine

No library tier and no solution touches the machine: no file, socket, clock read, thread, console,
ambient randomness or process-wide setting. Every answer is then a function of its inputs, and every
test of one repeats. Randomness enters as a seeded `Random` the caller passes in: each of the seven
problems that draw at random, from `ShuffleAnArray` to `RandomPickWithWeight`, takes one from its
caller, as a constructor or method argument, and its tests and benchmarks seed it.

The two places that do touch the machine are harness tools, and each keeps its I/O apart from its
logic: the benchmark baseline tool (`BaselineCommand`'s files, clock and exit code around a pure
comparison) and the LeetCode catalog (`LeetCodeApiClient`'s network and `LeetCodeQuestionCache`'s
disk around pure records, mapping and validation). `Tests/Architecture/MachineAccessTests.cs` scans
`DSAExperimentation` and `DSAExperimentation.LeetCode.Solutions`, with comments and strings blanked, and fails
on any of those accesses.

### 17.7 Tier 5: what stays in a harness

Two things, and only two:

- **Assertions.** A test file — `DSAExperimentation.LeetCode.Solutions.Tests/<Problem>/<Problem>SolutionTests.cs`,
  named for the solution class it tests, at the path that mirrors it — states its cases once as a
  `public static TheoryData<...>`, LeetCode's published examples written inline. It then carries one
  `[Theory] [MemberData(...)]` method *per strategy*, so a failure names the strategy
  that broke rather than reporting a disagreement between two anonymous arms, and at least one test
  named for each input-preparation helper §17.4 makes public, asserting what that helper builds.
  A solution that names its LeetCode question is the exception. `CatalogCaseTests` runs every
  strategy it has over every catalog case, one theory row per strategy, so a failure still names the
  strategy (§17.13). Its own test file keeps only what the catalog cannot state, such as an input
  outside the question's promise.
- **Workload sizing.** How *large* an input to measure is a measurement decision, so it stays in
  `Benchmarks/Fixtures/`: `LockWorkloads.BuildDeadends(count, seed)`, `HammingWorkloads.BuildChain`,
  `WeightedGridWorkloads.OpenGrid(size)`. What each of those builds *from* is domain code.

The line is: deterministic construction of the problem's own structure is tier 4; choosing a random
seed and an input size is tier 5.

`Benchmarks/Fixtures/` therefore holds workload generators only, never a domain type, and a
problem's test folder has no `Fixtures/` subfolder at all.

Most of those generators serve one benchmark — 131 of 152 when this was measured — and that is the
intended shape, not a failed attempt at sharing: a `<Problem>Workloads` generator is where one
problem's workload, and the property its comment promises about it, are written down and tested.
What *is* shared is what they are built from. A plain array of seeded values is
`SeededDraws.Values(count, low, high, random)`, and an index permutation is `SeededSequences`;
neither a benchmark nor a generator restates that pipeline inline.

Sizing is per arm where the arms' costs differ in kind. `[Params]` applies to every arm of a class,
so an exponential baseline would cap a polynomial arm at a size where its growth could not show;
a class like `BurstBalloonsBenchmarks` instead gives each arm its own sizes through
`[ArgumentsSource]`, shares the smallest ones so the arms are still compared, and builds every
size's workload in `[GlobalSetup]`.

Every size stays inside the problem's stated constraints. The benchmarks exist to make the
solutions as fast as possible on the inputs LeetCode actually poses without giving up the
abstractions they are built from, so a harness that times an input past those bounds is measuring
a problem nobody asked, and is a defect to fix rather than a choice to record. That covers everything a harness controls: lengths, node and grid sizes, query and call
counts, drawn value ranges, and guarantees such as "the answer fits in a 32-bit integer" - past
that one, both arms overflow to the same wrong number and still agree. A fast arm that cannot
show its growth inside the bound stops there anyway; its class comment says so.

### 17.8 Every arm returns the whole answer

An arm returns LeetCode's whole answer, never a proxy for it. A proxy — `nums[0]` after an in-place
rearrangement, the sum of reported name lengths over a design replay, `.Length`, a checksum, a count
of sequences instead of the sequences — lets two arms "agree" while answering differently, and puts
the proxy's own walk inside the timed region. This holds even where the answer is large:
`WordLadderIIBenchmarks`' arms build and return every shortest sequence, so the generic arm check
compares the sequences themselves, and on its chain-shaped workload the shortest-path DAG stays
narrow enough that the comparison is still about search cost.

A design problem's arm returns the sequence of outputs its operation script observed, written into a
buffer sized in `[GlobalSetup]`; a script of operations that return nothing ends with the reads
LeetCode would use to observe it. A problem whose answer is a mutation the repo's types cannot hold -
LC 116/117's next pointers, which `BinaryTreeNode` has no field for - answers with what LeetCode's
judge reads back from it: there, each level along its next pointers with `#` between levels.

### 17.9 Gate-compliance notes for the new tiers

§6 records the repo's standing gate notes; these are the ones §17's own conventions create.

- **`check-responsibility-extraction` is waived tier-wide over `DSAExperimentation.LeetCode.Solutions/*/*Solution.cs`.**
  The check reads three or more public declarations sharing a name prefix as a hidden module
  boundary. §17.3's `<Operation>By<Strategy>` naming produces that prefix wherever a problem has
  enough strategies or overloads — `MinTurnsByMutationQueue`/`MinTurnsByReduceGraph`, each with a
  LeetCode-shaped and a hoisted overload, is four declarations sharing `Min` — so the check fires on
  196 of the 1,105 solution files (measured 2026-10-03), on the convention working as designed. The
  boundary it infers is already the file: the shared prefix is the one operation the problem asks
  for, and splitting the strategies apart is exactly the drift §17.1 exists to undo. This is a
  waiver on the *convention*, not on any one file's shortcomings.
- **`check-constant-placement` was waived on the types whose subject IS their constants**,
  `ModularArithmetic` and LC 752's `LockWheels`. The check's own prescribed remedy is "move them to
  a type of its own, in its own file whose subject IS these values", and each already was that type,
  yet it still fired. Both waivers went with their types, when the general lock and the modulus
  witnesses replaced them (§17.10).
- **A search reports "no path" as `null`, not as a shared sentinel.** `HammingSearch` returns
  `int?`: a public `Unreachable = -1` on a search type is the smell `check-constant-placement`
  names, and with a nullable result each caller maps "no path" onto its own problem's convention
  (LC 127 reports `0`, LC 433 reports `-1`) instead of agreeing on a magic value in between.
- **LeetCode's reporting conventions are declared once, at the root of the tier.** `-1` for "no
  valid answer" is LeetCode's convention, not any one problem's value, so it is
  `LeetCode/LeetCodeAnswer.cs`'s, and `check-duplicate-constant` flags a solution that declares its
  own. `1_000_000_007` is the same kind of convention, and lives beside it as
  `LeetCode/LeetCodeModulus.cs`. What a modulus is belongs to `Domain/Mathematics/Modular`, and the
  arithmetic over one to `Algorithms/NumberTheory` (§17.6); which prime LeetCode reports in is
  LeetCode's.
- **`check-test-coverage` asks for a corresponding file and member-named methods, nothing more.**
  It wants a test file whose **name corresponds to the source file** and whose **method names
  mention the members**: `HeapTests`' ordinary `Push_Pop_MinHeap_ReturnsElementsInAscendingOrder` /
  `TryPeek_EmptyHeap_ReturnsFalse` satisfy it. Its findings are cleared by writing those tests,
  never by a waiver (§18.2).
- **A benchmark class's harness coverage is one generic check, plus a companion only where it pins
  something.** A benchmark's arms are entry points the runner invokes; whether each arm answers
  *correctly* is owed by the solution's own tests (§17.7), and what the harness owes is narrower:
  that its arms time the same question on a reproducible input. `Benchmarks.Tests/BenchmarkArmsTests`
  asserts that once for every class with a `[Benchmark]` method, found by reflection, so a new
  benchmark is covered the moment it exists. At each `[Params]` member's smallest value, on a fresh
  harness per arm: rebuilding the workload reproduces it, every arm answers the same on a rebuilt
  harness, every arm answers what the baseline answers, and every arm answers the same when called
  again on the harness it already ran on, with only its `[IterationSetup]` re-run in between, as
  BenchmarkDotNet calls it across iterations. Answers are compared by walking their
  fields (`AnswerGraphText`), so node-returning arms and void arms that rewrite the harness are
  compared by shape and value. The per-problem knowledge it cannot learn by reflection lives in
  `ArmAgreement`, each entry with its reason: answers LeetCode leaves unordered, random draws, and
  arms whose valid answers differ by design. An entry is for a difference the *problem* allows, not
  for a harness defect — an arm that drops what it builds, a solution that seeds itself, an arm that
  consumes the workload it was handed, a workload outside the problem's contract are fixed instead. A `<Benchmark>Tests` companion exists only to
  assert what that check cannot — an expected value derived independently of the arms, a property
  of the workload's shape, a size other than the smallest. A companion that compares arms only with
  each other restates the generic check under a different name, and is not written.
- **Every library tier owes direct unit tests, and the LeetCode tier is a layer on top, not a
  substitute.** A data structure, algorithm, mechanism or domain model is not covered because some
  LeetCode solution happens to compose it: a coverage tree shaped that way can only tell you a
  problem's answer changed, not which piece broke. `DSAExperimentation.Tests/` mirrors the source
  tree for exactly this reason: `Domain/Devices/Locks/CombinationLockTests.cs` tests
  `Domain/Devices/Locks/CombinationLock.cs`.
- **A type's rationale goes in prose here, never in `<remarks>`, and `check-doc-schema` is why.**
  `<remarks>` is C#'s catch-all bucket: the check projects it as free prose rather than a section,
  because a type's doc is otherwise only its summary plus the sections its signature implies
  (`<param>`/`<returns>`/`<exception>`/`<example>`), and an essay is none of those.
- **Two advisories are accepted rather than fixed.** `check-transposable-parameters` on signatures
  like `LadderLengthByMutationQueue(string beginWord, string endWord, ...)` is reporting LeetCode's
  own problem signature, which the solution tier deliberately mirrors; and `check-type-nature` on
  `AddBinarySolution.DigitWalk`. Both are advisory and neither fails the gate.
- **`check-abbreviations` is waived on `EnglishMultigraphs`, whose names are spellings.** Its
  named values are the digraphs and the trigraph themselves, `Ch`, `Sh` and `Tch`, which the
  check reads as abbreviations; a multigraph has no other name, and one called by an example
  word (`Church` for ch) would name the word, not the letters. `Language/Phonetics` went the
  other way: check-folder-organization flagged fifteen flat files, and they were grouped into
  `Vowels/` and `Consonants/`, the IPA chart's own division, rather than waived.

### 17.10 Migration status

Complete: every covered problem has its `<Problem>Solution` in
`DSAExperimentation.LeetCode.Solutions/<Problem>/`, and its test and benchmark are harnesses over it. Count
this by solution-file existence, not by the coverage manifest's tier tag, which under-reports.

In progress: moving the domain knowledge problem folders still hold into `Domain/` (§17.6). A pilot
of seven subjects, chosen to be unlike each other, has settled where the line falls:

- **Sudoku and tic-tac-toe moved.** `SudokuRules` (LC 36, LC 37) and `TicTacToeRules` (LC 1275)
  hold the board, its notation and its rule. Each problem folder keeps LeetCode's encoding of
  them, such as LC 1275's answer strings and its move replay, and each baseline keeps its own
  copy (§17.5).
- **Calendars was a shape.** That a booking is half-open, `[start, end)`, is said by five
  problems, so it is a shape, and shapes are tier 1 (§17.6). `IntervalSet` gained the half-open
  overlap question, and My Calendar I and II ask it instead of storing `end - 1`.
- **Accounts had no code to move.** With LC 721's strategy and LeetCode's row encoding taken
  away, what remained was a sentence.
- **Three were not domain at all.** Token expiry and the smallest free number apply a rule of
  their own that the caller sees, so they are mechanisms (§17.6b): `ExpiringSet` (LC 1797) and
  `SmallestFreeAllocator` (LC 1845, LC 2336). The history cursor drops only what an operation
  names by its position, so it is a data structure, `CursorSequence`. LC 2296 adopted it, and
  LC 1472 declined it on measurement (§13.3).
- **A bounded deque is a data structure.** Its bound refuses a push and drops nothing, so it is
  one configuration of `CircularBuffer`, with the deque, the queue and the bounded queue (§20).

The pilot's test for the rest: take away every strategy and LeetCode's encoding, and see what
stays. A rule of the problem's world, in that world's terms, is `Domain/`. A shape other problems
share is tier 1. A rule the structure applies on its own is a mechanism. If nothing stays, there
is nothing to move. The rest goes subject by subject, with composed arms using `Domain/` and
baselines keeping their copies.

Two rules govern each move:

- **An adoption is done when breaking it breaks a test of the problem itself.** For every Domain
  member a problem adopts, a mutation of that member, made in a mirror of HEAD, must fail one of
  the problem's own tests, not only the Domain type's tests. LeetCode's published examples are
  often too few for this. LC 1041's never told a right turn from a left, LC 17's never named keys 4
  to 9, and LC 289's never crowded a cell. A mutation that survives gets the example that kills it,
  in the same commit.
- **The most general exact form.** In the owner's words (2026-11-02): "do the most general form for
  a die, and do the same for other things too. that is part of the goal of this entire
  dsaexperimentation, it applies not just to modularity of data structure and algorithms but also
  domain logic and mechanisms." A subject is built in the most general form that is still exact, its
  well-known real-world variants shipped as presets and its parameters measured to cost nothing.
  Where the general form would stop being exact, it stops and names what it assumes. This replaces
  the earlier rule, which generalized only once a second variant existed in the repo.

A second wave took the four subjects that more than one folder restated, or that sat in tier 1
by mistake:

- **Chess.** `ChessRules` (LC 999, LC 1222, LC 2056) is the 8x8 board and which lines each
  sliding piece moves along. The lines themselves stay `GridDirections`' tables, because they are
  grid shapes that problems other than chess walk. The knight problems are played on other boards
  and keep `GridDirections.LShaped`. On 2026-11-03 the owner ruled the steps and their tables more
  general than any one game or domain, so they moved from tier 1 to `Domain/Mathematics/Lattices`,
  where a baseline may read them too.
- **The compass.** `CompassHeading` and `CompassRules` (LC 874, LC 1041) are four headings,
  quarter turns and a unit step. The step is given as east and north, because the problems
  disagree on axes.
- **English's vowels.** `EnglishVowels` serves LC 1641, LC 2063 and LC 3227.
- **DNA's bases.** `DnaBases` left tier 1's `StandardAlphabets`, a graph's folder. The `Alphabet`
  record they filled has since folded into the alphabet contract (§19.6).

Measuring the wave found two costs that moving a rule into `Domain/` can carry. Both were fixed
before the step landed:

- A foreach over an `ImmutableArray` inside an iterator keeps the enumerator in the iterator's
  object, so it allocates more. LC 2056 walks its lines by index.
- A rule written as a switch can be too big for the JIT to inline into a hot loop. `StepOf` began
  as a four-way switch, and LC 874 made a call per step and ran 10% slower. It reads a table now.

It also found one trap in reading the measurements. Some arms drift more between two builds of the
same code than the change under test moves them. LC 1222's set arm read ±20% against itself, so
its result needed a pass of HEAD against HEAD to interpret.

A third wave took the subjects that only one folder stated, and the owner's rulings settled the
ones the second wave left open:

- **Moved, each as the general rule with LeetCode's variant as its configuration.** These are:
  - the phone keypad (E.161, LC 17);
  - Life-like rules, with Conway's B3/S23 as a preset (LC 289);
  - Zuma's collapse, with the classic game's shortest vanishing group as a preset (LC 488);
  - a die of any face count, with the six-sided die as a preset (LC 2318);
  - letters numbered from 1 (LC 91, LC 2156);
  - folder-path notation (LC 1233).
- **Geometry names its plane.** `EuclideanPolygons` and `EuclideanPlane` state the flat-plane facts
  that LC 976, LC 2971, LC 3024, LC 3899 and LC 1610 use. `ConstantCurvatureSurface` gives the
  angle sum on a sphere or a hyperbolic plane, so the triangle's 180° reads as the flat case of a
  general rule.
- **Invented rules stayed.** The race car's motion (LC 818) and the ATM's banknotes (LC 2241) are
  rules those problems invent, so they stay in their folders (§17.6).

`Domain/` was then grouped by field: Mathematics, Games, Devices, Language, Science and Computing.
It is a peer of `DataStructures/` and `Algorithms/`. Any of the three may read another, and
`LayeringTests` refuses a cycle between their folders (§17.6a).

Three subjects were then generalized, each on the evidence the rule above asks for:

- **The combination lock** (`Domain/Devices/Locks`) takes any wheel count and symbol range. LC
  752's four decimal wheels are the `FourDigitLock` preset.
- **The sliding puzzle** (`Domain/Games/SlidingPuzzle`) takes any board of at least 2x2, and
  decides which arrangements can be solved by the parity law. `EightPuzzle` and `FifteenPuzzle`
  are presets, and LC 773 configures its own 2x3 board.
- **The modulus** (`Domain/Mathematics/Modular`) is `IModulus` and `IPrimeModulus`. The
  arithmetic over one, Fermat inverses and a factorial table, is in `Algorithms/NumberTheory`.
  LeetCode's 10^9 + 7 is `LeetCodeModulus`, at the root of the LeetCode tier. The least modulus
  is 1, the zero ring, which the owner ruled in (2026-11-02) so that LC 3525's moduli k = 1 to 5
  are `IModulus` witnesses too. Reading the `long` modulus as an `int` where k sizes an array
  measured no different from the problem's own `int` witnesses.

The lock and the puzzle present themselves as state spaces, and tier 1's `StateGraph` builds
their graphs. LC 818 builds its graph the same way, over its own state space.

The grid's steps (2026-11-03). The owner ruled that `GridStep` and `GridDirections` "should be
more general than any one game or domain", "all useful values that agree with the topology of the
grid", and "generalizable to satisfy a wide range of topologies". A plan in nine phases followed:

- **The steps are the square lattice's** (`Domain/Mathematics/Lattices`, 1a9ffc87), so a baseline
  reads them. Their algebra is on the step (3ca6577a): steps add and negate, a quarter turn and a
  mirror generate the square's symmetries, and a table agrees with the lattice when it is a union of
  orbits, which the tests check for each one. `Clockwise` is the turning cycle.
- **Every solution reads them** (a8914353, 25156000). The chess baselines read `ChessPiece`'s moves,
  about thirty solutions read `GridDirections` instead of their own tables, the spirals and the
  compass turn along `Clockwise`, and `LatticeTableCopies` refuses a new literal copy.
  - LC 3001's baseline walks a span of the piece's moves: passed the `ImmutableArray` itself, the
    walk read 12 to 13% slower at 100,000 queries, and as a span it reads within the spread of
    identical code.
  - Measuring it found that LC 3001's and LC 1222's benchmarks drew their squares into a
    `HashSet<(int, int)>` and took them in its order, which follows hashes .NET seeds afresh in
    every process. Each process ran a different workload, and an A/A pass read 23% apart. Both now
    keep the order drawn.
- **`GridChildren` reads a mask**, one bit per step that leads somewhere open, so `Count` is a
  population count and `Get(i)` the i-th set bit. Engines ask `Count` on every turn of their loop.
- **The board's surface** (`Domain/Mathematics/Surfaces`): the rectangle, cylinder, torus, Mobius
  strip and Klein bottle, read back by their Euler characteristic and boundary circles.
  `GridSize.HasCell` is the bounded rectangle's rule.
- **A board carries its steps** (`Grid.Steps`), so a search moves along any table, and LC 1091 has
  an 8-connected arm on the engine.

The element algebra (2026-11-05). The laws the range structures need of their elements moved from
`DataStructures/ElementAlgebra` to `Domain/Mathematics/Algebra`, since they are mathematics, and
took the mathematics' names at the owner's ruling: `IMonoid`, `IAbelianGroup` and
`IIntegerModule`, with `Addition`, `Minimum`, `Maximum` and `BitwiseXor` as presets. Every user
names them so; the move changed what each compiles to in nothing but the names.

The time units, English's number words and the per cent (2026-11-05). Another session began them
on 2026-11-03, and the owner asked for them to be finished. Each states facts a problem's arms
read, and leaves how they are combined to the arms:

- **Time units** (`Domain/Science/Time`). Each unit, the microsecond to the week, is a struct
  carrying its length, a constant read from .NET's `TimeSpan` (the name it carried then moved to
  `EnglishTimeUnitWords` in the TODO plan's B7), so the platform stays
  their one source and the week's seven days is the one count stated here. LC 1348 reads the
  seconds in a minute, an hour and a day as `TimeUnit.Minute.Ticks / TimeUnit.Second.Ticks`, a
  constant expression, so its switch on a frequency's name is three literals, as when it held its
  own numbers. Code generic over its units has `TimeUnit.Count<TUnit, TIn>`. Two first forms cost
  LC 1348 time. As named values whose length was a `TimeSpan`, the units had LC 1348 count seconds
  on every query, 4-13% slower in both passes of an A/B (a value fixed for a configuration is a
  constant: the owner's ruling of 2026-11-04). Calling `Count` from the switch folded each arm to
  its number, but the switch's IL grew past what the JIT would inline into the query, which then
  paid a call on every query and read 7-21% slower. A day example in LC 1348's tests, tweets at
  86,399 and 86,400 seconds, holds only for a day of exactly 86,400 seconds.
- **English's number words** (`Domain/Language/English`). `EnglishNumberWords` gives every word
  with the number it names (`NumberWord`): zero to nineteen, the tens, a hundred and the short
  scale's powers of a thousand. LC 273 builds its capitalized tables from them once, and reads the
  thousand it groups by, the hundred, and the twenty numbers below the tens from the words
  themselves. How the words compose is not stated here; the owner's TODOs below take it up. LC 273
  has tests of its own beside the catalog's three cases, a number for every word it spells.
- **The per cent** (`Domain/Mathematics/Numerals`). `IPartsPerNotation` writes a proportion as
  parts of a fixed whole, with per cent, per mille, per myriad (finance's basis point) and parts per
  million as presets. LC 1357 and LC 2303 divide by `Percent.PartsPerWhole`, a constant. Taking a
  share of an amount is not stated: in floating point, dividing by the whole last and multiplying by
  a rate worked out once round differently, and which a problem does is its arms' choice.

Every value a problem adopted was mutated in turn, and one of that problem's own tests failed for
each: LC 1348's for the second, minute, hour and day, LC 1357's and LC 2303's for the hundred, and
LC 273's for each of the 32 words an int needs and for the hundred and the thousand. Measured as
landed against the commit before them, in two passes, no arm moved one way beyond what unchanged
code reads apart: LC 1348's query compiles to 1,711 bytes against 1,705 and makes no call, and
LC 273's divides and multiplies by the same constants as before.

The owner's TODOs (2026-11-04). The owner left 47 TODOs in 14 Domain files and ruled to keep every
subject they raise, each organized on its own terms: "not everything needs a problem that uses it,
the problems are just the inspiration." A value fixed for a configuration is a constant, or a static
member of a struct witness that the JIT folds, never derived again at run time. Each batch removes
the TODOs it answers:

- **Small fixes.** `EnglishUppercase` has `LastLetter`, derived from `FirstLetter` and the count as
  the lowercase run's is, and `EnglishConsonants` is the alphabet's other 21 letters, read from the
  vowels. The 15-puzzle's TODO asked whether it needs a type of its own: a board type is how
  `SlidingPuzzle<TBoard>` gets its rows and columns as constants, which its comment now says. The
  degree's TODO asked whether a turn is 360 degrees off a flat surface, which `AngleUnits` already
  answered: the unit is a definition, and only the angles around a point need flatness to make a
  turn.
- **Chess in Betza notation.** A piece is a name and a list of moves (the owner's choice of shape),
  each a `PieceMove`: its steps, a reach in place of `IsSliding`, and the conditions under which it
  may be made. A rider's reach is no number rather than `int.MaxValue`, which a solution's own
  sentinels share. The pawn is three moves facing its side, and `BetzaNotation` reads the notation
  the tests hold every preset to. LC 999, 1222, 2056 and 3001 read the steps of a piece's one move.
- **Language.** Accented letters are another alphabet's, not English's (the owner, 2026-11-05),
  and spelling, sound and script are different subjects, so each got a place. English's written
  vowels and consonants moved to `Language/English/Orthography`, beside its digraphs (ch, sh, th,
  ph, ng, ck) and trigraph (tch); a multigraph is the languages' general type in `Language/`.
  `Language/Phonetics` holds the IPA's vowel and consonant charts as named values, the inventory a
  language configures, and reading a transcription back into it. `Language/English/Phonology` is
  Received Pronunciation's 44 phonemes as that configuration, each vowel named by its lexical set,
  with y's two roles: a consonant letter that spells j, KIT and PRICE. `Language/Scripts/Latin`
  takes a letter apart into its basic letter and diacritics through Unicode's canonical
  decomposition, which only the accented alphabets (Spanish, French and German as presets) reach.
  The alphabet types stay in `Language/English`, English's configuration of the script. Only the
  vowels' and consonants' namespace moved for their three problems and LC 567's benchmark, so no
  arm's code changed.
- **Calendars, and the units' names.** The Gregorian calendar's rule is three constants: a leap
  year every 4 years, except every 100, except every 400. The Julian calendar is the same rule with
  every century year kept (4, 100, 100). Each calendar is a struct witness whose static abstract
  members fold in a caller (`Domain/Science/Time/Calendars`). The common year's 365 days are the
  sum of the twelve months' constant lengths. The 146,097 days of a Gregorian cycle are derived
  from the rule, and the tests count them year by year. A month and a year are a calendar's units
  rather than `TimeUnit` lengths, which answers the TODO asking for them; a custom unit of a fixed
  length is a struct implementing `ITimeUnit`. A unit's English name moved from `ITimeUnit.Name` to
  `EnglishTimeUnitWords`, named values carrying their unit's length as `EnglishNumberWords` carries
  numbers, which answers the TODO asking `TimeUnit` to follow that pattern. Only the units' own
  tests read the names. The tier-1 code showed that the JIT kept the Julian calendar's century
  test, a remainder by 100 compared with itself, until the calendar's equal cycles were tested
  first; now its leap test is one test of the year's low two bits. No solution states a calendar
  fact, since the five calendar problems (LC 1154, 1185, 1360, 1507 and 2409) are not covered, so
  no arm changed.

All four subjects the owner opted into on 2026-11-02 are done: the time units, English's number
words, the per cent, and the element algebra above.

Deferred by the owner: generalizing beyond what is exact, such as geometry on surfaces of varying
curvature. This waits until the architecture is stable, and then happens only if it helps.

Open items:

- **LC 773 reads 1 to 3% slower** since its graph became a `StateGraph` (7408f14b). A variant
  with the problem's own non-generic topology recovered part of the gap. What remains is no larger
  than the spread on identical code, and its cause is not settled.

### 17.11 What every harness shares: LeetCode's conventions, a project of their own

A problem's harness is a hand-written test and a hand-written benchmark over its solution class, with
one exception: its benchmark of the catalog's cases is generated from its solution's
`LeetCodeQuestionAttribute` (§17.13). Sizing a workload stays in tier 5 (§17.7), and that every
strategy is asserted is `LeetCodeStrategyCoverageTests`' to enforce (§18.5); a harness generated from
a per-problem registration would move the first into tier 4 and duplicate the second.

What the solutions and their harnesses share is LeetCode's own conventions, the root of this tier.
**Since 2026-11-03, at the owner's request, they are a project of their own,
`DSAExperimentation.LeetCode`,** below the solutions, which compile against it. It holds what
LeetCode fixes for every problem rather than any problem's answer:

- Its notation: `LeetCodeWireFormat` (its array notation for trees and lists, both directions),
  `LeetCodeAdjacency` with the slot shapes it fills (`IAdjacencySlots`, `NeighborIdSlots`,
  `WeightedNeighborSlots`), and `LeetCodeIntervals`.
- Its answer rules: `LeetCodeAnswers` (exact, sequence, any-order, rows-in-any-order, within
  tolerance), `LeetCodeAnswer`'s `-1`, and `LeetCodeModulus`'s 10^9 + 7.
- `LeetCodeQuestionAttribute` and `LeetCodeAnswerOrder`, with which a solution enrolls in the catalog
  harness (§17.13).

A test or benchmark that needs one of these calls it rather than writing its own `BuildList` or
order-insensitive comparison. Their namespace is `DSAExperimentation.LeetCode`, which encloses every
solution's, so a solution names them with no using. Every type stays internal, granted to the
projects that read it, as the rest of the tier's are.

What the project boundary gives over the folder it replaced:

- The harness library compiles against the conventions and the catalog, not the solutions. It finds
  the solutions it runs by reflection, so changing a solution does not rebuild it.
- The conventions cannot name a problem, since their project does not reference the solutions. That
  half of §17.3's import rule is now the compiler's.

`AnswerGraphText`, which renders an answer of any shape so two arms can be compared, is not a
convention of LeetCode's. It is a harness's way of comparing answers, so it moved to the harness
library (§17.13).

### 17.12 The LeetCode catalog: what LeetCode says, and the cases a harness runs

`DSAExperimentation.LeetCode.Catalog` holds, for every covered problem, what LeetCode publishes about
it and the cases its harnesses run, as JSON under `Snapshot/`. `LeetCodeCatalog.Problem(slug)` gives a
problem's question, statistics and contest rating, and `LeetCodeCatalog.Cases(slug)` its cases.
Nothing in the project calls the network. The snapshot's one writer is
`DSAExperimentation.LeetCode.Catalog.Sync`, run by hand and weekly by
`.github/workflows/leetcode-catalog-sync.yml`. That job runs the whole suite against what it wrote
and opens a pull request, so `dotnet test` stays offline and every refresh is reviewed.

**What a problem carries, and from where.** The files are split by how often their contents change,
so a refresh's diff shows only what LeetCode changed:

- `Questions/<slug>.json` is rewritten only when LeetCode edits the question. It holds the
  question's identity and link, its Easy/Medium/Hard difficulty, its topics, similar questions and
  hints, the statement, and the C# starter code. It also holds the constraint lines as text, from
  which `LeetCodeBounds` reads the numeric ranges when asked rather than storing them.
- The question file also holds the signature, metaData typed. Its parameters are the *judge's*
  inputs, which can include one the function never sees, such as Linked List Cycle's `pos`. For an
  in-place question, it names the input the judge reads the answer back from.
- `Statistics.json` holds every question's acceptance rate, submission totals and votes, under one
  date.
- `ContestRatings.json` holds zerotrac's Elo-style rating, which exists only for a question that
  appeared in a contest. LeetCode itself publishes no numeric difficulty.

**The cases, and who vouches for them.** No public API exposes LeetCode's judge suite, so no case
here is one of its hidden tests. Each case carries its source:

- **Examples** are the published examples. Their outputs are read from the statement's prose and
  paired with inputs only when the statement states exactly one output per example, and each stored
  example's inputs are the ones the statement shows at its position. For 14 questions LeetCode's
  example list holds inputs the statement never shows, such as Check if There is a Valid Path in a
  Grid's three grids, and pairing by position had recorded other inputs' answers for them until
  2026-11-03. An output the statement writes in prose, such as "tail connects to node index 1", is
  not recorded. Reading the statement's text folds each run of whitespace into one space, except
  inside a quoted string, whose spaces are its value: Print Words Vertically's `"   T"` had been
  recorded as `" T"`, and Regions Cut by Slashes' second grid, `[" /","  "]`, had not matched its
  stored input, which left all three of its examples unpaired, until 2026-11-04.
- **Dataset** cases come from LeetCodeDataset (Apache-2.0). Their inputs were generated and their
  outputs printed by a reference Python solution. The importer drops a case:
  - whose reference failed;
  - that it cannot translate;
  - where the reference returned None for an answer that cannot be empty;
  - with an input not of its declared type, such as a 1.5 in an integer tree (80 such cases, in 33
    questions, were in the snapshot until 2026-11-03);
  - whose answer is not of the type the question returns, such as an integer past 2^31 - 1. The
    reference computed with Python's unbounded integers an answer its statement promises will fit,
    so the input broke that promise (179 such cases, in 10 questions, were in the snapshot until
    2026-11-04). A whole-number answer the reference printed as a float, The Number of Good Subsets'
    `31.0`, is written as LeetCode writes it, `31`;
  - that breaks a parsed bound;
  - that repeats another case.
- **LeetCodeRun** cases, outputs from LeetCode's own Run button, have a folder reserved and no
  importer yet. That endpoint needs a signed-in session.

A generated input can still break a promise written as prose, which no parser reads. Two Sum's
"exactly one valid answer" is one. The harness states such a promise, and a type in the problem's
folder holds it so both harnesses share one statement, as `TwoSumPromise` does.

**How the harnesses use it.**

- A solution that names its question has its strategies run over `Cases(slug)` by `CatalogCaseTests`
  (§17.13). Any other harness can still build its `TheoryData` from `Cases(slug)`, reading each input
  with `Input<T>` in LeetCode's notation and converting trees and lists through `LeetCodeWireFormat`
  (§17.11).
- A benchmark of the catalog's cases is generated rather than written: a solution that names its
  question gets a `<Problem>CatalogBenchmarks` class at build time (§17.13). Each of its arms runs a
  strategy over every case per invocation, as LeetCode times a submission across its suite, and
  writes the answers into a buffer sized in `[GlobalSetup]` (§17.8).
- That workload sits beside the sized workloads, not in place of them. Most catalog cases are small,
  so it shows what a strategy costs on the inputs LeetCode poses, and the sized workloads (§17.7)
  still show how it grows.

The solutions enrolled as §17.13 describes have moved onto the catalog, and §17.13 records how far
that has gone. The other harnesses still write their examples inline.

### 17.13 The catalog harness: generated benchmarks and one test, not a class per problem

**Decided 2026-11-02, at the owner's request.** A problem's catalog workload is timed by a benchmark
class written at build time, not by a hand-written `<Problem>SuiteBenchmarks`, and its strategies are
held to the catalog's cases by one test for every problem, not by a theory in each
`<Problem>SolutionTests`. Rolled out by hand, each would have been 1,100 more files of the same code
with the names changed. What a problem must supply is the knowledge the catalog cannot state, and only
that.

**What a problem states.** One attribute on its solution class, beside the strategies it describes:

```csharp
[LeetCodeQuestion("two-sum", LeetCodeAnswerOrder.AnyOrder, Promise = typeof(TwoSumPromise))]
internal static class TwoSumSolution
```

- The question's slug. A folder's name and LeetCode's title disagree for 35 of 1,105 problems, mostly
  in acronym case (`Xor`, `XOR`), so the slug is stated rather than derived.
- Whether LeetCode accepts the answer's elements in any order. This is the explicit choice
  `LeetCodeAnswers` asks for (§17.11), and it is never a default. A nested answer can free either
  depth: `AnyOrder` frees the rows, `EachInAnyOrder` each row's own elements (LC 1489's two edge
  lists), and `AnyOrderThroughout` both (LC 49's anagram groups).
- The type that states a prose promise a generated case can break (§17.12), when there is one.
- Whether the strategies may rewrite the inputs they are handed (`RewritesInput`), as relinking a
  list does, so the generated benchmark hands each call inputs of its own (below).
- Whether the question is answered in place (`AnswersInPlace`), as Move Zeroes moves the zeros
  within `nums`: each strategy returns nothing, and its answer is its first input as the call leaves
  it. Its inputs are copied for every call, as for `RewritesInput`.
- How far a real-valued answer may stand from LeetCode's (`Tolerance`), when the statement says so:
  "Answers within 10^-5 of the actual answer will be accepted". `CatalogJudge` then accepts each
  number that close, and holds arms to one another with each number written as the nearest multiple
  of the tolerance, so two arms LeetCode would each accept for the other agree. Twelve significant
  digits, as `AnswerGraphText` holds a double, split Pow(x, n)'s two arms, whose million
  multiplications and squarings part in the eleventh digit, whenever a rounding boundary fell between
  them.
- The type that judges an answer where the statement lets several stand (`AnswerCheck`), as "return
  any of them" does. Its `IsAccepted` takes the strategy's inputs, the answer and the recorded
  answer, which `CatalogAnswerCheck` reads from the case into its parameter types. The recorded
  answer is one of those that stand, so it says what any accepted answer must achieve - its length,
  its size, whether there is one at all - rather than what it must be: Longest Palindromic
  Substring's check takes any palindrome in the string as long as the recorded one. `CatalogJudge`
  asks the check instead of comparing.
- For a design question, the type standing for the calls the strategies' instance does not answer by a
  method of the call's name (`Calls`), as `LRUCacheCalls.Get` turns `ICache`'s `TryGetValue` into
  LeetCode's -1 for a missing key. Each of its methods is named for its call and takes the instance,
  then the call's arguments (below).
- The type that states how far the strategies reach (`Reach`), when one of them cannot finish every
  case the catalog holds: a brute force that runs for seconds on the larger inputs, a recursion that
  overflows the stack. Its `IsWithin` takes the strategy's inputs, as a promise's `IsKeptBy` does, and
  `CatalogColumns` leaves out every case beyond it for every arm, so all of them answer the same cases
  and the ratio to the Baseline keeps its meaning. It bounds the question's own measure - a length, a
  value - and chooses no input of its own.

The first eight are facts about the problem; the reach is one about its strategies. The attribute
names no workload: the catalog's cases are the inputs LeetCode poses, and choosing sizes stays a
tier-5 decision (§17.7). A reach is the one size it names, by the owner's ruling of 2026-11-05: it
says which of LeetCode's own cases the strategies can finish, where a workload would choose inputs. Being data,
the attribute keeps tier 4 off the machine (§17.6c). Nothing in the solutions project reads the
catalog.

**What is generated.** `DSAExperimentation.Benchmarks.Generators` is a source generator the
benchmarks project runs as an analyzer. It writes one `<Problem>CatalogBenchmarks` class per
enrolled solution. They are filed in `ProblemSolutions` beside the hand-written problem benchmarks, so a
run of that category times both, and `--filter '*CatalogBenchmarks*'` selects the generated ones alone.

- Every strategy (§17.3) becomes an arm, and the first declared is the baseline.
- `[GlobalSetup]` reads the cases into typed column arrays through the harness library's
  `CatalogColumns`. That conversion out of LeetCode's notation is paid once, never inside the timed
  loop. `NotationReader` holds the rules: scalars (a `uint` too, from a non-negative integer, as
  LeetCode's bit questions take one), arrays, tuples from `[start, end]` rows, trees
  and lists through `LeetCodeWireFormat`, and a positional record of value type - one public
  constructor that keeps each value as the property of the same name - as its values: a role type of
  one value, such as `SourceText(string Text)`, as that value, and a record of several as an array of
  them, as a tuple is read. For a case that lists more inputs than the strategy takes, a tuple or
  record of several values stands for as many consecutive inputs, when the widths add up exactly: Rectangle
  Area's eight coordinates are two `Rectangle(X1, Y1, X2, Y2)`. The generator's `NotationTypes` states
  the same rules over compile-time types.
- Each arm copies its columns into locals and calls the strategy directly in a `for` loop, writing
  each answer into a buffer (§17.8).
- A solution whose strategies may rewrite their inputs, as relinking a list does, says so with
  `RewritesInput = true`. Its class fixes every iteration at 16 calls with `[InvocationCount]`.
  Before each iteration, untimed, an `[IterationSetup]` per arm reads 16 fresh copies of the cases,
  and each call spends the next copy. A 17th call would index past them and fail rather than time an
  input an earlier call consumed. The benchmarks' tests call such an arm twice within one iteration,
  as BenchmarkDotNet does, so an arm that reused its copy would answer differently the second time.
- A `Try` strategy's answer is its return value gathered with its out parameters, as Two Sum's
  `(found, first, second)`. One output is written as itself, as Find Substring With Given Hash Value's
  substring is, and several as LeetCode's array of them.
- A strategy that answers in place, for a question that says so, is called on the iteration's next
  copy of its inputs, and the arm stores that first input as the call left it.
- A strategy may have two overloads a case can feed, when its hoisted overload takes a type the
  notation reads too, as §17.4 allows: `(int Start, int End)[]` beside `int[][]`, `string[]` beside
  `string`. The arm is the overload LeetCode's signature shapes - the one with as many inputs as the
  question declares, then the one matching most of the declared types - and a tie is refused. The
  benchmarks project hands the generator each question's snapshot file for its signature, and
  `CatalogStrategy` reads the same signature from the catalog (`LeetCodeShape` in each).
- A design question's cases are call scripts: the class's name, then each method LeetCode calls on
  one instance, beside each call's arguments. Its strategies are factories, `CreateBy<Strategy>`, that
  take the constructor's inputs and make the instance. The generator reads the question's constructor
  and methods from its snapshot file (`DesignShape`) and binds each method by name, ignoring case and
  LeetCode's underscores: to a static method of the question's `Calls` type, which takes the instance
  first, or else to the instance's own method (`ScriptShape`). `[GlobalSetup]` lays the scripts out
  once (`CatalogScripts`): which method each call makes, and each call's arguments read into the types
  of the method that answers it. Each arm makes its instance with its factory, case by case, and makes
  every call directly through a switch on the method, writing each answer into a buffer of that
  method's return type. It returns the buffers together as a `CatalogCallAnswers`, which the tests
  read back as LeetCode writes a script's answer: null for the constructor and for each call that
  returns nothing. `CatalogStrategy` replays the same bindings by reflection (`CatalogCalls`).
  LeetCodeDataset holds no design question, so each is judged on its published examples alone.
- A strategy no arm can call fails the build with `DSACAT001`, naming why. It may be generic, answer
  in place where its question does not say so, or take a type the notation cannot be read into. Leaving it out instead would show an
  enrolled solution as measured when one of its strategies was not.

**Why generated, and not one class driven by parameters.** A single class whose arm is a
`[ParamsSource]` value would call each strategy through a delegate or reflection. That breaks the
static-dispatch rule this repo measures by, and it would lose everything the runner keys on a class:

- `LibraryCategoryDiscoverer` reads categories from an arm's IL, which a reflective call hides;
- `--filter` selects by class and method name;
- the ratio column needs a `Baseline` arm within a class;
- `BenchmarkArmsTests` and `ArmAgreement` are per class.

A generated class is ordinary C#, so all of them apply unchanged.

**What is tested, and where.** The two harnesses share `DSAExperimentation.LeetCode.Harness`, a project
above the solutions. It compiles against LeetCode's conventions (§17.11) and the catalog, and it
finds the solutions themselves by reflection. It reads the catalog from disk, which §17.6c keeps out
of tier 4. It holds:

- `NotationReader` and `CatalogColumns`, the reading described above;
- `CatalogStrategy`, which finds an enrolled solution's strategies by the generator's own rule and
  calls one on one case;
- `CatalogCalls`, `CatalogScripts` and `CatalogCallAnswers`, which bind a design question's calls,
  lay its scripts out for a generated arm, and read the arm's answers back;
- `AnswerNotation` and `CatalogJudge`, which write an answer back in LeetCode's notation and judge it
  against the recorded output by the question's order rule. A missing tree or list in a sequence of
  them is LeetCode's `[]`, as a whole answer that is null is, the way LeetCode prints LC 725's empty
  parts;
- `AnswerGraphText`, which renders an answer of any shape, so the benchmarks' tests can compare a
  hand-written class's arms.

The tests split by whose correctness each one is about:

- `CatalogCaseTests`, in the solutions' tests, runs every strategy of every enrolled solution over
  every catalog case. It is the one test those strategies need for LeetCode's cases.
  `LeetCodeStrategyCoverageTests` counts a strategy it runs as named (§17.7), taking the set of
  strategies from the harness itself, so a strategy the harness would not run still needs a test of its
  own.
- A solution's own `<Problem>SolutionTests` keeps only what the catalog cannot state. Two Sum's keeps
  a target no pair reaches, which LC 1's promise rules out. Climbing Stairs and Merge Intervals had
  nothing else, and theirs are gone. Of the 607 enrolled after the pilot, the trim below removed what
  `CatalogCaseTests` restates where it could do so whole.
- `CatalogBenchmarksTests` checks what belongs to the generated classes:
  - each baseline's buffer lines up with its cases and answers them as recorded, the pinning §17.9
    asks of a harness test;
  - the classes are exactly the enrolled solutions';
  - each class's arms are exactly the strategies the harness runs.

**How it answers the registry harness's retirement** (ARCHITECTURE-HISTORY §17.11):

- *Nobody adopted it:* only ten registrations ever existed, and each problem kept its hand-written
  classes as well. Here, enrolling is one line, and the hand-written suite classes were deleted, not
  kept beside it.
- *It sized workloads in tier 4:* the attribute carries no size, as above.
- *It duplicated a check that already held everywhere:* this replaces the checks it covers rather
  than standing beside them. `CatalogCaseTests` takes the place of each enrolled problem's catalog
  theories, and `LeetCodeStrategyCoverageTests` reads which strategies it runs. `CatalogBenchmarksTests`
  pins each generated baseline, which a `<Problem>BenchmarksTests` did one class at a time.
  `BenchmarkArmsTests` holds the generated classes, like any class, to reproducing their workload and
  answers.

**Checked.**

- Every strategy of every enrolled solution answers every catalog case as recorded.
- Mutations, each run on an isolated copy, fail the check that owns them, naming the strategy, the
  input and both answers:
  - a Climbing Stairs strategy off by one fails `CatalogCaseTests`, and only it;
  - Two Sum enrolled without its promise failed the judge on the cases with several pairs, checked
    in the pilot, when `CatalogBenchmarksTests` still judged every arm;
  - Climbing Stairs un-enrolled fails `LeetCodeStrategyCoverageTests`, since nothing else names its
    strategies any longer;
  - an Add Digits strategy off by one fails its `CatalogCaseTests` row and its generated class's arm
    agreement in `BenchmarkArmsTests`, which reads both arms in LeetCode's notation.
- Enrolling the 610 added about 1,200 `CatalogCaseTests` rows and 3,600 benchmark-test rows, in a
  Debug build:
  - the solutions' tests went from 4 s to 8 to 12 s;
  - the benchmarks' tests went from 14 s to 36 s, most of it the generated arms' repeat-call
    checks in `BenchmarkArmsTests`.
- Measured against the hand-written suite classes it replaced, in one BenchmarkDotNet run of both, two
  passes (2026-11-02):
  - every arm allocated the same bytes;
  - generated time over hand-written ranged 0.86 to 1.17, with the sign flipping between passes for
    four of the six arms;
  - the hand-written arms alone moved 9 to 14% between the two passes on identical code.

  No difference showed beyond that spread. The hand-written classes were deleted.

**Reach, measured.** Every strategy of every solution was run over every catalog case before any
enrolled (2026-11-03, a Release build, 15 s per solution). The 1,105 solutions came out this way:

| Outcome | Solutions |
| --- | --- |
| Every strategy answers every case as recorded | 630 |
| The same, with an answer's elements in another order | 21 |
| Some answer differs from the recorded one | 110 |
| A strategy throws on some case | 38 |
| Runs past 15 s over the catalog | 81 |
| Overflows the stack on the catalog's largest case (Coin Change, Super Egg Drop, Nim Game) | 3 |
| Design questions (a call script, not one function) | 92 |
| No overload a case can feed, or several: a domain-typed parameter, an in-place answer | 85 |
| A judge-only input, such as Linked List Cycle's `pos` | 13 |
| A case the reader cannot read | 6 |
| A folder whose name is not the title's (an explicit slug resolves it) | 15 |
| A recorded output in prose (the importer no longer records one) | 10 |
| No strategy at all | 1 |

The differing answers are mostly not wrong strategies:

- Most are generated cases that break a constraint the importer cannot parse: an odd-length array
  where the statement says `2n`, a tree that is no binary search tree, repeated values where they
  must be distinct, an answer past the 32 bits LeetCode promises.
- Some are judges LeetCode runs that the catalog's comparison does not: answers within a
  tolerance (Pow(x, n)), any valid answer (Convert Sorted Array to Binary Search Tree, Cracking the
  Safe), and any order at every depth (Group Anagrams).

A solution is enrolled only when every strategy answers every case. Its answer order is read from
its statement, never from which order passed. A sequence answer the statement says may come back
in any order is `AnyOrder`, and every other answer is `AsWritten`, unless the statement frees an
inner depth too. Two statements are read differently from their wording:

- Reveal Cards In Increasing Order's "any order" is about the deck it is given, so its answer is
  `AsWritten`.
- Matrix Cells in Distance Order frees the order only among cells at one distance, which an
  any-order comparison cannot see, so it is not enrolled.

A strategy that rewrites its input kept 27 solutions out, nearly all relinking a linked list, until
the generator copied inputs for them (above). A generated arm read its columns once in
`[GlobalSetup]`, and BenchmarkDotNet called it again and again on them, so from the second call on it
would have timed a different workload. Merge k Sorted Lists' second call merged lists its first had
already spliced together, and never returned. The probe renders every input before and after each
call to find these, and each of the 27 now says `RewritesInput = true`.

A solution that passes every case still stays out when its question has no recorded output to judge.
Kth Smallest Path XOR Sum's two examples state none, and the Populating Next Right Pointers questions
have only the empty tree. A check over those would pass whatever the strategy did.

**Enrolled.** 1,038 solutions: the 3 of the pilot, 593 of the 619 that every case passes in under
250 ms each, and 442 more:

- 14 were found once the importer's first two fixes landed or their folder was matched to its slug
  by hand (`3sum`, `01-matrix`, `132-pattern`, ...).
- 5 were found once examples stopped being paired with inputs their statement never shows, among
  them Check if There is a Valid Path in a Grid, whose strategies had been right all along.
- Minimum Operations to Equalize Subarrays and Number of Possible Sets of Closing Branches took
  their inputs in another order than LeetCode's. Their parameters now follow LeetCode's order.
- Car Fleet had a public helper named like a strategy, which the harness ran as one, and a generated
  case with a car already at the target. The helper is `ArrivalTimesNearestTargetFirst` now, and
  `CarFleetPromise` states the promise that case breaks.
- The 27 whose strategies rewrite their input, once the generator copied inputs for them.
- GCD Sort of an Array, whose recorded outputs had been prose until the importer stopped recording
  those. The first probe ran before that fix, and a second probe of every unenrolled solution, on
  2026-11-04, found it passing.
- All Elements in Two Binary Search Trees, whose collect-and-sort strategy parted from the catalog on
  46 generated cases holding trees that are no binary search trees. Its promise drops them.
- 20 whose failing cases each broke a constraint the statement states in prose, now stated by a
  promise type each: an even count (Array Partition), no zero-sized asteroid, only `+`, `-` and
  balanced parentheses (Basic Calculator), letters only (Camelcase Matching), a string as long as the
  tree (Check if DFS Strings Are Palindromes), a search tree (Closest Nodes Queries) or a search
  tree's preorder, at most 30 bits, circles inside the first quadrant, heights up to 100, unique
  ratings, a rotated increasing array, one repeated value from 1 to n, edges to nodes of the graph,
  a bit inside the string, k up to the value count, a square image, no self-loop or repeated edge
  (Count the Number of Complete Components), one valid postfix expression, and numbers without
  leading zeros (Add Two Numbers). Every promise keeps judged cases, at least 14 of them. Basic
  Calculator's two strategies had parted on expressions with a parenthesis left open, where the
  catalog records one strategy's answer. Flipping an Image flips in place, as LeetCode lets it, so
  it also says `RewritesInput = true`.
- 35 more on the same footing, among them a self-loop (Longest Cycle in a Graph, Maximum Employees to
  Be Invited to a Meeting), repeated points (Max Points on a Line, Minimum Area Rectangle), piles not
  in threes, an unreachable end (Jump Game II), unsorted arrays (Median of Two Sorted Arrays) and
  inconsistent observations (Loud and Rich). K Closest Points to Origin answers in any order, and
  its promise rules out a tie at the kth distance. Three promises read the constant their solution
  already states (Maximum Number of Coins' `GroupSize`, Minimum Moves to Spread Stones' `GridSize`,
  Perfect Rectangle's corner indices) rather than state it again.
- Longest Substring with At Least K Repeating Characters, once its brute force stopped answering 0
  for k = 1. A character's first occurrence put it below k and its kth lifted it out, in an
  `else if`, so with k = 1, where the two are one occurrence, only the first happened. The catalog's
  16 cases with k = 1 found it, where the published examples have none.
- Split Linked List in Parts, once a missing part was written `[]` as LeetCode writes it. It rewires
  its list in place, so it says `RewritesInput = true`.
- 30 more promises, among them search trees of unique values (Range Sum of BST, Search in a Binary
  Search Tree), square grids, balanced parentheses, a special binary string, unique answers (Top K
  Frequent Elements, in any order; Two Sum II, counting pairs in one pass), and answers that fit 32
  bits, checked with exact arithmetic (Product of Array Except Self, Unique Paths). Remove
  Duplicates from Sorted List II relinks its list, so it also says `RewritesInput = true`.
- 5 whose statements accept an answer within 10^-5, once the judge honoured that: Angles of a
  Triangle, Calculate Amount Paid in Taxes, Minimum Area Rectangle II, Statistics from a Large Sample
  and Car Fleet II. Car Fleet II's promise also drops a case listing its cars backwards.
- 3 whose answers free an inner order too, once the judge had rules for that: Group Anagrams and
  Critical Connections in a Network (`AnyOrderThroughout`), and Find Critical and Pseudo-Critical
  Edges in Minimum Spanning Tree (`EachInAnyOrder`). The last two promise a connected network, which
  the generated cases did not always keep.
- Accounts Merge, whose generated cases gave one email to accounts of different names. The statement
  makes one person of accounts that share an email, and its promise drops the rest.
- Print Words Vertically, once its recorded example kept the spaces of `"   T"` (§17.12).
- 5 whose failing cases each recorded an answer past 32 bits, once the importer dropped those
  (§17.12): Coin Change II, Combination Sum IV, Decode Ways, Baseball Game and Replace Non-Coprime
  Numbers in Array.
- 14 more promises, among them an acyclic graph (All Ancestors of a Node in a Directed Acyclic
  Graph, checked by the library's topological sort), no move after a win (Find Winner on a Tic Tac
  Toe Game, on Domain's tic-tac-toe rules), a rooted tree with one edge added (Redundant Connection
  II), roads that never cross (Shortest Distance After Road Addition Queries II), a search tree (Find
  Mode in Binary Search Tree, in any order), unique positions (Erect the Fence, in any order), and a
  whole x where there is one (Solve the Equation, read term by term rather than through the
  strategies' parse). Gas Station and 3Sum Closest promise a unique answer, which generated cases
  broke with several stations that could start or two sums tied for closest.
- 12 whose statements accept any of several answers, once an answer check could judge them: Longest
  Palindromic Substring, Longest Duplicate Substring, Sort Characters By Frequency, Subarray With
  Elements Greater Than Varying Threshold, Convert Sorted Array (and Sorted List) to Binary Search
  Tree, Cracking the Safe, Create Grid with Exactly K Paths I, Build a Matrix With Conditions, Valid
  Arrangement of Pairs, Smallest Sufficient Team, and Maximum Nesting Depth of Two Valid Parentheses
  Strings. The last two also promise what their generated cases broke: a valid parentheses string,
  and pairs that make one path.
- 3 whose strategies call a static local function, once neither the harness nor the generator took
  the compiled function for a strategy. It is an internal method of the class named for the strategy
  around it (`<CountByLoop>g__Step|0_0`), so it carries that strategy's `By`; read from the solutions
  assembly's metadata, as the generator reads it, no code can name it. Combination Sum (in any
  order), Convert BST to Greater Tree (which rewrites its tree, so `RewritesInput = true`) and
  Maximum Sum BST in Binary Tree.
- 24 whose strategies take a role type for one LeetCode input, such as Find the Index of the First
  Occurrence in a String's `Haystack` and `Needle`, once the notation reader and the generator read
  one as the value it holds. Four also promise what generated cases broke: a lock mask as long as its
  string (Check if a Parentheses String Can Be Valid), one index per query character (Longest
  Substring of One Repeating Character), exactly one `*` (Substring Matching Pattern) and unique words
  (Word Ladder). Stamping the Sequence's answer check takes any stamp sequence that spells the target.
- Number of 1 Bits and Reverse Bits, once the reader and the generator read a `uint`. A rectangular
  array would have let four grid solutions in too, but Number of Possible Sets of Closing Branches
  hoists a `long[,]` overload beside its LeetCode-shaped one, which the generator could not then tell
  apart (§17.4), so grids stayed unread. It reads each question's signature now, but the four grid
  solutions' baselines are unmemoized recursions that would need a case limit first.
- 12 with a hoisted overload the notation reads too, once the harness and the generator bind the one
  LeetCode's signature shapes: Linked List in Binary Tree, Minimize Manhattan Distances,
  Non-overlapping Intervals, Partition Equal Subset Sum, Path Existence Queries in a Graph II, Queue
  Reconstruction by Height, Russian Doll Envelopes, Sliding Puzzle, Sorting the Sentence, Step-By-Step
  Directions From a Binary Tree Node to Another, Two City Scheduling and Valid Square. Three also
  promise what generated cases broke: a queue that can be reconstructed, words numbered from 1, and
  two different values of a tree whose values are unique.
- Collect Coins in a Tree, once its strategies took `(coins, edges)` in LeetCode's order and promised
  a tree of one node per coin; Words Within Two Edits of Dictionary, once the trie work beside it
  landed, with a promise that every word has one length; Power Grid Maintenance, once its grouping
  helper stopped being named like a strategy (`ConnectedGridsOf`, as Car Fleet's was renamed); and
  Increasing Order Search Tree, which relinks its tree, so `RewritesInput = true`.
- 7 whose strategies take several of LeetCode's inputs as one tuple or record, once consecutive inputs
  could be read into one: Cheapest Flights Within K Stops, Minimum Time to Reach Target with Limited
  Power, Minimum Moves to Capture the Queen, Minimum Weighted Subgraph With the Required Paths, Maximum
  Total Beauty of the Gardens, Path with Maximum Probability (within 10^-5, promising edges and
  endpoints that are nodes of the graph) and Find Substring With Given Hash Value (promising lowercase
  letters and a window that has the hash, which generated cases broke). Rectangle Area and Circle and
  Rectangle Overlapping read now too, but their unit-grid arms take seconds over the catalog.
- 10 answered in place, once the attribute could say so: Move Zeroes, Sort Colors, Rotate Image, Set
  Matrix Zeroes, Game of Life, Surrounded Regions, Flatten Binary Tree to Linked List, Recover Binary
  Search Tree, Reorder List and Wiggle Sort II, which also takes any valid wiggle through an answer
  check. The dataset records no changed input for such a question, so each is judged on its 2 or 3
  examples; every generated case is still timed, and holds the arms to one another.
- 72 design questions, once call scripts could be replayed (2026-11-05). 61 bind as they are, their
  instances answering every call by its name. 11 state a `Calls` type: LRU Cache and LFU Cache (get's -1),
  Implement Queue using Stacks (`IsEmpty`), Implement Trie (`IPrefixSet`'s `Add`, `HasKey`,
  `HasPrefix`), Design SQL (ins, rmv, sel, exp), Subrectangle Queries and Detect Squares (corners and
  points as LeetCode lists them), Design Bitset (`ToBitString`), Walking Robot Simulation II, Design
  Memory Allocator and Design Graph With Shortest Path Calculator. Longest Uploaded Prefix and Seat
  Reservation Manager each have a strategy that needs no count, whose factory takes LeetCode's count
  all the same. 37 of the 72 first gained factories, their strategies having been nested classes
  only.
- Complete Binary Tree Inserter, whose constructor takes the tree its insert calls grow, once a design
  class could copy its inputs (`RewritesInput = true`, as for a function question). The tests replay
  each script from inputs read fresh, but a benchmark that read its columns once would start every
  iteration after the first from the tree the last one grew. Its class fixes an iteration at 16 calls,
  and an `[IterationSetup]` per arm lays the scripts out 16 times, untimed, so each call spends a copy
  of the constructor's and the calls' arguments of its own. Its `Calls` type answers get_root with the
  inserter's `Root`.
- Minimum Number of Moves to Make Palindrome, promising a string that can become a palindrome: at most
  one letter appears an odd number of times. Both strategies nudge an unmatched letter inward and
  retry, so on the 38 generated cases that break it they never finished.
- 2 whose strategies leave out an input LeetCode lists, once each gained LeetCode's shape beside its own
  (§17.4): Longest Common Subpath (the city count) and Delivering Boxes from Storage to Ports (the port
  count, with a promise that every box goes to a port and fits one trip, which a generated case broke).
- 92 whose slowest strategy runs past the time a question's cases are allowed, once a reach could leave
  out the cases it cannot finish (2026-11-05). Every strategy ran every catalog case, smallest inputs
  first, each on a thread of its own with a 1.5 s deadline (Release build, 1 MB stacks as a test host
  gives). A question's reach is the bound - on one input's measure, a length, a nested array's element
  count or a value, or on two together for 9 of them - that keeps the most cases while every kept case
  finished in every strategy and each strategy's kept cases add up to 250 ms. A generator wrote each
  `<Problem>Reach`, its tests and the attribute from that bound; the order rule and a tolerance it read
  from the statement, as for the rest. The median question keeps 49% of its cases: from 3 of Manhattan
  Distances of All Arrangements of Pieces' 89 and 2 of Cat and Mouse II's 57 to all of Count Almost
  Equal Pairs II's and Permutation Sequence's; 34 keep under a third and 13 over nine in ten. Coin
  Change, Super Egg Drop and Nim Game, whose recursions overflowed the stack, reach amounts up to 6,
  150 floors and 1,000 stones.

  The reached cases showed what the slow ones had hidden. Permutation Sequence (a rank past n!),
  Probability of a Two Boxes Having The Same Number of Distinct Balls (an odd number of balls) and
  Count Increasing Quadruplets (a value outside the permutation) gained promises; Beautiful Array an
  answer check, since any beautiful array stands; and Select Cells in Grid With Maximum Score's helper
  stopped being named like a strategy (`RowsHoldingEachValue`). Best Time to Buy and Sell Stock IV
  gained LeetCode's shape, the budget k before the prices, beside its own.

  Pow(x, n) took its reach by hand, since its cost follows |n| and a bound on n itself would keep the
  most negative powers, the slowest of all: |n| up to 1,000,001 keeps 72 of its 78 cases. Its promise,
  that xⁿ lies within 10^4 and is defined, leaves out the generated cases that run to 10^72, where two
  correct strategies part in their fourteenth digit; within it, answers stand within 10^-5, the five
  decimals LeetCode prints.

  20 more followed the same day, from a second probe of the unenrolled solutions. 17 took the reach the
  generator states, Number of Valid Move Combinations On Chessboard among them once the chess rework
  beside it landed. 3 slow down with their numbers' size, not their count, so a bound on length kept
  few of their cases, and their reach is written by hand on a measure the generator cannot state:
  Check If It Is a Good Array's element sum up to 1,937,500,000 (113 of its 122 cases, against 11 by
  length), Matchsticks to Square's sticks summing to at most 114 (48 of 74, against 16), and Minimum
  Operations to Make Array Equal to Target's elements each within 999,999 of their target (97 of 105,
  against 1). The sums are measured cuts over the catalog's cases rather than the arms' costs: two
  numbers far apart in size cost a subtraction gcd more than their sum says.

  The reached cases cost the tests time: in a Debug build the solutions' tests went from 20 s to 38 s
  and the benchmarks' from a minute and a half to three, most of it brute-force arms whose kept cases
  `BenchmarkArmsTests` runs again for its repeat-call checks. A smaller budget than 250 ms would trade
  cases for that time.
- 9 from the same second probe whose answers parted from the catalog's for a reason their statement
  gives (2026-11-05). Different Ways to Add Parentheses and Frequencies of Shortest Supersequences say
  any order stands, the second with a reach of up to 15 words. Course Schedule II, Sort Items by Groups
  Respecting Dependencies and Matrix Cells in Distance Order let any valid order stand, so each gained
  an answer check: every course after its prerequisites, every group in one stretch, the distance never
  falling. Four promise what generated cases broke: lowercase words (Check If a Word Occurs As a Prefix
  of Any Word in a Sentence, whose trie arm threw on "c++ c#"), a position within the row (K-th Symbol
  in Grammar, which also takes a reach of rows up to 22, its row expansion doubling each row), and a
  legal expression, checked by grammar without evaluating: Parse Lisp Expression (15 generated cases
  leave a parenthesis open, 8 put an expression where a variable must stand) and Parsing A Boolean
  Expression (22 give `!` several operands, read by the generator as the negated conjunction).
- 3 grid questions, once the reader and the generator read a rectangular array (2026-11-05): Cherry
  Pickup with a reach of grids up to 6 rows (60 of its 77 cases; an 8-row grid takes its unmemoized arm
  452 ms), Longest Increasing Path in a Matrix up to 50 cells (80 of 84), and Number of Increasing Paths
  in a Grid, every case of which its naive arm finishes. A `T[,]` is read from LeetCode's rows when they
  are of one length; rows of different lengths fill no rectangle and stop `[GlobalSetup]`, as any value
  that does not fit its type does. A grid parameter never matches a declared `integer[][]`, so a solution
  that hoists one beside its LeetCode-shaped overload, as Number of Possible Sets of Closing Branches does
  a `long[,]`, still binds the jagged one. The generator writes an array of a grid as `int[][,]`, since C#
  reads rank specifiers outermost first and `int[,][]` is a grid of arrays.

Open items:

- **The sized workloads.** It is not yet decided whether the 1,026 hand-written `[Params]` classes
  stay as they are or become per-problem workload declarations the generator also consumes.
  Measured 2026-11-03 over the 615 enrolled solutions, 509 of which also have a sized class: for
  the 510 whose constraints state a length bound the measure could read, the catalog's largest input
  is under 1% of that bound for 327 and at least half of it for 57, with a median of 0.15%. The
  catalog's cases are LeetCode's examples and the dataset's generated inputs, mostly small, so the
  sized classes remain the only timing at the sizes the constraints allow.
- **Copying inputs, measured** (2026-11-04). An A/A of the 28 classes that copy their inputs, beside
  four that read theirs once, on identical code: a copying arm's mean moved a median 3.2% between the
  two runs, against 5.2% for the four, so the means reproduce as well. Within one run, though,
  BenchmarkDotNet's error interval was a median 5.9% of the mean, against 1.9%: 16 calls an iteration
  average over fewer calls than the pilot gives a class that reads its inputs once. The widest were
  arms that allocate, Delete Nodes from Linked List Present in Array's array scan at 59% and two more
  past 25%. A difference under about 10% in one of these classes needs a second run before it means
  anything. More copies per iteration would narrow the interval, at the cost of memory and setup time.
- **Enrolling the rest.** 67 solutions are left (2026-11-05), each blocked for a reason the table and
  the exclusions above name. Each needs one of these, or stays out:
  - A solution whose answers differ needs its statement read, then a promise type for each
    constraint its failing cases break (§17.12), or a judge rule below.
  - A slow strategy whose reach the generator could not state: Rectangle Area, Circle and Rectangle Overlapping, Rectangle Overlap and
    Find the Number of Possible Ways for an Event finish no case on every strategy in time; Kth Smallest
    Instructions was not measured; and Find the Shortest Superstring lets
    any shortest superstring stand, so it needs an answer check first. Maximum Number of Moves to Kill
    All Pawns' graph arm spends about 35 ms on every case, two pawns or fifteen, so no bound on its
    input keeps more than 4 of its 96 cases.
  - A design question that lets several answers stand needs an answer check over a whole script: All
    One Data Structure's getMaxKey returns "one of the keys" with the largest count.
  - A design question whose answers are random (Shuffle an Array, Random Pick with Weight, Insert
    Delete GetRandom O(1) and seven more) has no recorded answer to compare.
  - Ten design solutions are not shaped as an instance and its calls: Number of Recent Calls, Range
    Sum Query - Immutable and Range Frequency Queries answer one query, or one batch, per call; Fancy
    Sequence, Kth Largest Element in a Stream, Online Election, Online Stock Span, Prefix and Suffix
    Search, Range Sum Query 2D - Immutable and Kth Ancestor of a Tree Node take a constructor other
    than LeetCode's.
  - A statement that leaves the order free without saying so (Word Search II, Majority Element
    II, Expression Add Operators, Concatenated Words, Palindrome Pairs) needs the order rule
    LeetCode's judge applies, checked against its own submission rather than read from the wording.
    Every strategy of the five answers every case in some order. Concatenated Words and Palindrome
    Pairs would also promise unique words, which generated cases with a repeated word break.
  - Maximum Product of the Length of Two Palindromic Substrings: the dataset records a wrong output on
    3 cases. "babadabababa" holds "babadabab" and "aba", for 27, where it records 25; a brute force
    over every pair agrees with both strategies on all three. The catalog has no way yet to correct an
    output it imports.
  - Sudoku Solver's strategies return whether they solved the board, where LeetCode's answers in place
    and returns nothing, as `AnswersInPlace` requires; its backtracking-engine arm also runs past 3 s
    on one generated puzzle.
  - Check If There Is a Valid Parentheses String Path reads its grid as a `char[,]`, which no row of
    another length fits, and 15 generated grids have one. It needs LeetCode's jagged shape beside its
    own, with a promise of rows of one length, before the reader ever meets them.
- **Trimming the enrolled solutions' own tests.** A theory row restates a catalog case when
  a case has the same inputs, read into the strategy's types, and the catalog's judge accepts the
  row's answer for it. That row is the check `CatalogCaseTests` already makes. Measured over the 608
  test classes of enrolled solutions (2026-11-03):

  | Rows | Count |
  | --- | --- |
  | Restated by a catalog case | 1,671 |
  | Inputs no catalog case has, such as Contains Duplicate's empty array | 1,091 |
  | A shape the measure cannot read | 23 |
  | An answer the catalog does not accept | 6 |

  The six are trees the test writes in another form than LeetCode's array: All Possible Full Binary
  Trees, Maximum Binary Tree, and Construct Binary Tree from Preorder and Inorder Traversal. Their
  strategies pass `CatalogCaseTests`.

  Where every row of a data property was restated, the property and the theories reading it are
  gone: 223 test files whole, and the example theories of 10 more whose other tests stay. Each
  comment that named a deleted test class now names `CatalogCaseTests`, or the solution that holds
  the approach.

  The rest followed on 2026-11-05, measured again over the 781 test classes of the 1,005 enrolled: 490
  data properties where only some rows were restated, and 74 more whose every row was. The restated
  rows went - 1,434 from the 490, keeping 1,427, and 224 with the whole properties - along with each
  comment that belonged to a dropped row (one naming an example, or over that row alone), and 64 test
  files were left with no test and deleted. What stays is renamed for what it holds: the property
  `Examples` is `Cases`, each theory's `_LeetCodeExamples_` is `_Cases_`, and a row record named for an
  example is named for a case. The comments were rewritten file by file, by eight agents working in
  parallel on 245 files: each now says what its remaining rows cover, and a header that pinned the
  tests to LeetCode's examples ends "LeetCode's published examples run in CatalogCaseTests." The rewrite
  found several comments that were never true - Find Winner on a Tic Tac Toe Game's claim that no
  example wins on the anti-diagonal, Remove Max Number of Edges' account of its example 3, and others
  - and corrected them. A few rows that are LeetCode's examples stay where the catalog lacks them or
  states them otherwise: an example of an earlier statement (Reverse Bits, Search Suggestions System),
  one the catalog records with no output (Isomorphic Strings), or an answer written in another form
  (Statistics from a Large Sample's mean as 24/11, a tree in preorder). The solutions' tests went from
  14,746 to 11,391.
- **Shapes the generator refuses.** Design problems and judge-only inputs each need a rule before
  their problems can enroll.

**How arms agree.** A generated class's arms are held to one another in LeetCode's notation, by the
attribute's order rule, as `CatalogJudge` holds a baseline to the catalog (`CatalogArmAnswers`). So
`ArmAgreement` needs no entry for one. An any-order answer may come back in another order. A tree
is its wire-format array, so All Possible Full Binary Trees' memoized arm, which shares subtrees
between the trees it returns, agrees with the arm that builds each tree afresh, as LeetCode reads
them. Its hand-written class is still listed as incomparable, because there `AnswerGraphText`
tells a shared subtree from a copy. Where an answer check judges, two arms may give different answers
it accepts, so they are held to one another by its verdict on each case instead.

## 18. Testing policy: every tier tests itself

**Every data structure and every algorithm carries its own direct unit tests. The LeetCode tier
(§17) is a layer *on top of* that requirement, never a substitute for it.**

A primitive is not covered because some LeetCode solution happens to compose it. Coverage shaped
that way can only tell you that a problem's answer changed — not which primitive broke, and not
that the primitive is right at its own boundaries. `Reduce.Graph` being exercised by forty coverage
problems says nothing about what it does with an empty frontier.

### 18.1 What "direct unit test" means here

- **One test file per source file**, at the mirrored path: `DataStructures/Heap/Heap.cs` is tested
  by `DSAExperimentation.Tests/DataStructures/Heap/HeapTests.cs`, and a Domain model the same way:
  `Domain/Devices/Locks/CombinationLock.cs` by
  `DSAExperimentation.Tests/Domain/Devices/Locks/CombinationLockTests.cs`.
- **Test method names begin with the member under test**, in this repo's existing
  `Member_Scenario_Expectation` form: `TryPeek_EmptyHeap_ReturnsFalse`,
  `Count_ReflectsPushesAndPops`, `OneCharacterMutations_RestoresItsBufferBetweenPositions`.
- **Witness types get tested too.** `NaturalChildOrder`, `ListChildren`, `BinaryTreeTopology`,
  `SizeAlgebra`, `ByPriorityOrder` and friends are one-method structs, but they are the pieces every
  algorithm is generic over, and their behaviour is exactly what a caller relies on. They are cheap
  to test.

### 18.2 Why the naming rule is load-bearing, not cosmetic

The Nomos `check-test-coverage` gate reads both halves — file correspondence and member-naming —
and reports anything else as uncovered. That is a real property, not a formality: a member with no
test naming it is a member whose failure will surface under a test name that does not mention it.

So a `check-test-coverage` finding in the library's tiers is answered with a test, never a waiver. A waiver
arguing that a member is exercised transitively, or that the check mis-attributes, records exactly
the gap this section exists to close, and then stands in for the missing test. The check's only
waivers are over the solution tier's problem folders, which `LeetCodeStrategyCoverageTests` covers
by construction (§18.5), and over benchmark size lists.

### 18.3 What is covered, and how a nested type is named

Every source file under `DataStructures/` and `Algorithms/` that declares behaviour of its own has
its test file at the mirrored path, and no `check-test-coverage` waiver covers either tier. The
files without one are contracts, node and record types, and constant holders, whose behaviour is
their implementers' and users' to test. A test file is named after the one type it tests, never
after a family of them (`TreeFoldTests`, not `FoldTests`).

A nested type is its own unit, so its members are named from a test class nested the same way —
`BreadthFirstTraversal.HooksStep.Enter` is named by `BreadthFirstTraversalTests.HooksStepTests`,
which drives it through the traversal's own entry points.

### 18.4 The rule for new work

A new type under `DataStructures/` or `Algorithms/` ships with its own `<Name>Tests.cs` at the
mirrored path in the same change. A LeetCode coverage entry (§17) does not discharge that
obligation for any primitive it composes — and if a problem needs a primitive that does not exist,
§17's own rule still applies: mark it blocked, do not build one ad hoc and untested.

### 18.5 The layering rules are tests, not prose

The rules below are asserted by `DSAExperimentation.Tests/Architecture/`, so they hold whether or
not whoever is editing remembers them. The tests read the repository as *text* rather than as types
— the benchmark project is not on the test project's reference list, and a reflection-only rule
would be blind in precisely the place duplication happens. A rule that needs to know what a name
binds to reads `RepositoryCompilation` instead. It compiles the framework, solution and benchmark
sources with Roslyn, each handed what MSBuild hands the compiler: the shared framework, the
implicit and declared usings, and the project's `InternalsVisibleTo` grants. The benchmark project
is compiled only in `ProblemSolutions/` and `Fixtures/`, which bind to nothing of BenchmarkDotNet
but its attributes. `RepositoryCompilationTests` fails if any of the three has an error, since an
unbound name answers with an error symbol and hides the reference a rule asked about.

- **§17.2's tier order** — `LayeringTests`: a file in `DSAExperimentation` may reference its own
  tier or one its tier may read, whether in a `using` directive or inline in its code. The peers,
  DataStructures, Algorithms and Domain, may read one another, and their folders may form no cycle
  across tiers (`FolderGraph`, `FolderCycles`). The one deliberate inversion is allow-listed by
  file and target namespace, and a global using may name no tier (§17.6a). Every file of the project must sit in `DataStructures/`, `Algorithms/`,
  `Mechanisms/` or `Domain/`, so a folder no rule judges fails instead of passing unread. Each
  file must also declare its namespace inside its own tier, since every reference is read by the
  tier its namespace names. The LeetCode tier
  has no row: it is the top tier in a project of its own, so nothing it references can sit above
  it, and a row for it could never fail.
- **§17.7's "a harness holds assertions and workload sizing"** — `Tier5WitnessTests`. A type that
  implements a *structural* contract (`IGraphTopology`, `ITreeTopology`, `IFoldAlgebra`,
  `IRecurrence`, `IRandomAccessSequence`, `IHeapOrder`, …) is domain code wherever the file sits, so
  it may not be declared in a test folder, a benchmark class or `Benchmarks/Fixtures/`. Traversal
  hooks are excluded in a test, where a recording `IInOrderHooks` that collects a traversal so the
  test can assert its order is an assertion device, and included in a benchmark, which asserts
  nothing and so can only be using a hook as the algorithm. Base lists are read across lines, with
  comments and strings blanked, because a contract on the line after the type's name is the usual
  layout for a long generic one.
- **§17.3's "the solution class holds every strategy" and §17.7's "one test method per strategy"**
  — `LeetCodeStrategyCoverageTests`. Every non-private `<Operation>By<Strategy>` member of every
  `<Problem>Solution` needs a test method in the problem's test namespace whose first
  underscore-separated word is the member's name, and so does every other non-private static
  method - the input-preparation helpers §17.4 makes public so a benchmark can hoist them, each
  asserted on what it builds. This is the check the `check-test-coverage` waiver over
  `DSAExperimentation.LeetCode.Solutions/**` rests on.
- **§12.4's "per-call state belongs to the call"** — `AmbientStateTests`. No file in
  `DSAExperimentation` or `DSAExperimentation.LeetCode.Solutions` may declare
  an `AsyncLocal`, `ThreadLocal` or `[ThreadStatic]` slot; a hook or algebra that needs a runtime
  value carries it as a field. Comments and strings are blanked through the same
  `RepositoryFiles.CodeOf` the witness rule reads base lists with.
- **§17.6c's "nothing below the harness touches the machine"** — `MachineAccessTests`. No file in
  `DSAExperimentation` or `DSAExperimentation.LeetCode.Solutions` may reach the file system, the network, the
  clock, a thread, the console, ambient randomness or the process; a seeded `Random` a caller passes
  in is the only randomness. Each kind of access has its own pattern and finding, and theory cases
  pin both what each pattern catches and the look-alikes it leaves alone.
- **§17.3's "a problem folder reaches into another only along a named edge"** —
  `ProblemImportTests`. Inside `DSAExperimentation.LeetCode.Solutions`, a folder may name another problem's
  folder only along an allow-listed edge whose reason opens `Delegation:` or `Subroutine:`. An
  imported folder imports none. The tier's root, the `DSAExperimentation.LeetCode` project, cannot
  name a problem, since it does not reference the solutions. Every
  entry fails once its edge disappears. The scan binds every name in the solution project's
  compilation. A symbol declared in another problem's namespace is a reference to that problem,
  whether the file reached it by a using, by a partly qualified name, which needs no using, or by a
  bare call a static using brought in.
- **§17.5's simple form** — `BaselineIsolationTests`. Every
  `Baseline = true` arm of every `ProblemSolutions` benchmark is traced through the compiled code,
  as §17.5 describes. The test fails on any library implementation the baseline binds to outside its
  input: a container, an engine or a mechanism, but not a Domain fact, a contract, a constant or an
  enum. It also fails if fewer than 1,000 arms are
  found, or if any arm reaches no solution member. `BaselineTraceTests` compiles small probes
  against the real library and pins each thing the trace follows and each input it excuses.
- **§17.5's one source per fact** — `SharedValueCopyTests`. A constant in a problem's folder that
  repeats a public constant of the library or of the LeetCode tier's root fails the test unless
  `AllowedCopies` says why it means something else. `LibraryValueCopiesTests`
  compiles small probes and pins what a copy is.
- **§17.6a's "solutions see only the library's public types"** — `InternalsVisibilityTests`:
  `DSAExperimentation` grants its internals to its test assemblies and to nothing else.
- **§1's "section numbers are stable"** — `SectionCitationTests`. Every section citation in the six
  projects' code, the root's documents, `suppressions.json` and the agent runbooks and workflows
  under `.claude/` must name a number a heading of this document carries. It reads "§17.3",
  "ARCHITECTURE 17.9", "ARCHITECTURE.md #17.3/#17.6" and "section 17.2", including a number that a
  comment wraps onto its next line. A step inside a section is cited as "§5 step 5".

Every harness file is in scope of the witness rule.

## 19. The pieces the solutions kept writing

A survey of the solution tier found the same mechanisms hand-written dozens of times over, each
copy a little different: the long spelling of `MergeSort`/`BinarySearch` over an array (§9.5), a
sorted copy of an input the solution may not reorder (§11.3), binary search on the answer (§9.6), a one-way union-find link (§10.1), and the six below. Each was
added to tiers 1–2 with its own tests (§18.4), and then the *composed* arms were moved onto it.
Baselines were left alone, along with any helper or constant a baseline reaches, because a baseline
is what you would write without this repo (§17.5). The classification calls, each made before the
code and reviewed against this document, follow.

### 19.1 `MonotonicDeque`: a window order, not a heap order or an element algebra

Fifteen solutions kept a `Deque<int>` of indices, popping from the back every entry the new value
dominated and from the front every index the window had passed. `DataStructures/MonotonicDeque/`
does it once, storing each key beside its position so a key computed on the fly (`dp[i]`, `y - x`)
needs no array to be read back from. Positions must be pushed in non-decreasing order, a
precondition law. Like Queue and Stack, it sits in a folder of its own beside the structure it
composes (§13.5).

Its one axis is `IWindowOrder.IsDominatedBy(resident, arriving)`: does the arriving key make the
resident unable ever to be the window's extremum again? That is a Topology witness under §5's first
row, because it is re-derived on every push and max versus min changes what the front reports.
Neither ready-made contract fits. `IHeapOrder` is `Heap`'s own Topology contract (§5 step 5).
`Maximum`/`Minimum` are monoids, and a monoid does not promise that `Combine` returns one
of its arguments, so a deque over `Addition` would compile and mean nothing. Ties are dominated,
because every hand-written copy evicted them; a "keep ties" order would be a third witness.

`MinMaxWindow<Key>` pairs the two orders. Six solutions bound a window by its range rather than its
width (LC 1438, 2762, 3578, 3589, 3835 and 3845). Each pushed every value into a max deque and a min
deque, stepped the left edge forward while max − min, or a cost built on it, was too large, and
evicted from both at every step. It adds no axis. Both orders are fixed, because a range is both
extrema, and a caller that wants one extremum wants `MonotonicDeque`. It reports the range as keys
alone, because none of the six read where an extremum sat. Every push and eviction reaches both
deques, so they are empty together. The shrink condition stays with each caller, since that is
where the six differ. It is a readonly struct, and its callers hold it in a local and pass it by
value, for the reasons §19.8 measured.

### 19.2 `PrefixSums`: a consumer of the group chain, with a view rather than an interface

`DataStructures/PrefixSums/` is the static sibling of `FenwickTree`: running totals under an
`IAbelianGroup` (§11.4), answering an inclusive range by one `Invert`. It joins the element-algebra
chain as a consumer and adds no contract to it. It is a sealed class that *exposes* an
`ArraySequence` view, `Totals`, for `BinarySearch`, rather than implementing
`IRandomAccessSequence` itself. That is §9.4's shape: the Sequence contracts belong to the algorithms
that consume them. The exclusive read is named `TotalBefore(end)`, not `PrefixQuery`, because
`FenwickTree.PrefixQuery` includes its index, and an exclusive member under the same name would be
the off-by-one this type exists to remove. Widening `int` input to `long` totals stays the caller's
choice, as choosing `Addition<long>` already is.

`MatrixPrefixSums` is the same totals over a matrix, which LC 304's region sums were building by
hand. LC 1738's coordinate xors build one too, but read every total in a loop, where `TotalBefore`'s
bounds checks measured about 10% slower at 300 x 300, so it keeps its own table. `TotalBefore(rowEnd, columnEnd)` combines the top-left
rectangle before both ends, and `Query(top, left, bottom, right)` answers an inclusive rectangle from
four of those totals: the bottom-right one and the corner, less the strip above and the strip to the
left. Regrouping the four is sound only because `IAbelianGroup`'s operations commute, a law a
one-dimensional range never needed. Rows arrive jagged, the shape the problems hand over, and a
ragged matrix is refused. It has no `Totals` view, because no caller binary-searches a 2-D table.

### 19.3 `CompressedCoordinates`: a data structure by shape, filed by what it composes

The sorted distinct values of an input, ranked so a Fenwick or segment tree can be sized by `Count`
and indexed by rank. By §5 step 7 it would be a data structure, hardwired to its one array. It
builds with `MergeSort` and answers with `BinarySearch`, though, so it lives in `Algorithms/Searching/`
for the reason `HammingDistances` lives in `Algorithms/` (§17.6): in `DataStructures/` it would
invert the tiers. `RankOf` throws for a value the set was not built from. A caller ranking a derived
value finds out at once instead of reading a neighbour's slot, and `LowerBound`/`UpperBound` stay
for thresholds that need not be members.

### 19.4 `BreadthFirstDistances`: naming an engine call, not a new mechanism

`Reduce.Graph` in breadth-first order with `DistanceMapReduceAlgebra` was written out with all six
type arguments at a dozen call sites. `Algorithms/ShortestPaths/BreadthFirstDistances.From` names
it once with the three that vary. `GridShortestPath` and `HammingDistances` now forward to it and
stay, because each still fixes those three types for its own family. It is its own file rather than
a `ShortestPath` method for §16.1's reason: same output shape, no shared mechanism.

### 19.5 `PascalTriangle`: the general form of `FactorialTable`

`FactorialTable<TModulus>` divides by factorials through modular inverses, which only a prime
modulus guarantees, so it takes an `IPrimeModulus` witness (§17.6). `PascalTriangle`
builds by addition, so any modulus works — 10 and 5 are the ones LeetCode asks for — and an exact
mode holds coefficients through row 66, the last whose middle entry fits in `long`. The modulus is
the caller's, so it is an `Algorithms/NumberTheory` type beside `ModularPower`, not a LeetCode one.
Two composed arms use it today, fewer than the eight that moved `FactorialTable`, after a third went
back to a rolling row (§19.8). It was built anyway, on the owner's call that an exact and
composite-modulus binomial table belongs in the library before more solutions hand-roll one.

### 19.6 The alphabet: one contract, and its presets by language

`LowercaseAlphabet` was declared beside `LowercaseTrieNode`, while twenty-two solutions declared their
own `AlphabetSize = 26`. It moved to the root of `DataStructures/` and gained `IndexOf`/`LetterAt`,
which throw outside the alphabet rather than return -1, because a wrong slot silently reads another
letter's count. An uppercase twin followed for LC 424 and LC 621, and `LetterNumbers` and
`EnglishVowels` each restated part of the same alphabet. The owner asked for one general alphabet,
covering both cases and usable for other languages, built by composition (2026-11-02):

- **The contract is tier 1.** `DataStructures/Alphabets/IAlphabet` is an index scheme: `Size`,
  `IndexOf`, `TryIndexOf` and `LetterAt`, each static abstract, so code generic over a preset is
  compiled for that alphabet and a slot folds to arithmetic or a table read. It is a shape any
  structure can be generic over, so §17.6's first rule files it in `DataStructures/`. `LetterRun`
  is the layout of an alphabet that is one run of character codes, and `LetterTable` of one spelled
  out in order, with its slots read from a table: most of the world's alphabets are not one run
  (Russian's ё is coded after я), and a test-only Russian alphabet proves the contract beyond English. `ILetterSet` and `LetterSet` are
  a named subset of an alphabet's letters, read as a mask over its slots: a primitive static
  readonly field the JIT reads as a constant.
- **The presets are Domain's.** Which letters, in what order, is a writing system's fact:
  `EnglishLowercase` and `EnglishUppercase`; `CaseFolded`, two cases sharing slots, exact only
  where case pairs letter for letter; `EnglishLetters`, English's two cases folded, with English's
  numbering of its letters, which is English's and not every alphabet's; and `EnglishVowels`, a
  letter set over it.
- **Composition, not inheritance.** A preset is a struct that forwards to a layout, and folding case
  composes two alphabets. Nothing derives from anything.
- **A count belongs to the alphabet, not to a case.** English has 26 letters, and each case has 26
  slots because English pairs them one for one, so the one constant is `EnglishLetters.Size`, and
  the two case presets answer the interface from it. A count table reads `new int[EnglishLetters.Size]`,
  one slot per English letter, in whichever case the problem writes.
- **One source per fact.** `EnglishLetters.Size` stays `const`, with the interface member
  implemented explicitly, so a caller that needs a constant has one and `LibraryValueCopies` (§17.5)
  still sees a copy of it. A letter set's letters are an array, the one source its count and its
  spelled-out string are read from (the owner's ruling), so the vowels' count is derived rather than
  written beside them; as a non-constant it is no longer one `LibraryValueCopies` compares.

Graph/Hamming's `Alphabet` record, the letters a mutation substitutes from, was kept separate as a
runtime-chosen set (§5 step 5), but only two fixed values ever built it: the English lowercase letters
and DNA's bases. It folded into the contract: `HammingGraph.OneCharacterMutations<TAlphabet>` and
`HammingSearch.MutationDistance<TAlphabet>` take the alphabet as a type argument, which still cannot
be transposed with the value mutated, and `StandardAlphabets` went. A solution adopts an alphabet only
when no baseline reads its constant. The
owner ruled that a baseline may not read even a compile-time constant from a core type, so a
constant shared with a baseline stays local.

### 19.7 `NearestBoundary` dispatches statically

The monotonic-stack sweep chose its relation and direction through two private enums, switched on
every comparison inside the pop loop. Both are now private struct witnesses, so each of the eight
public forms is its own instantiation with the comparison inlined. The public surface is unchanged,
and the sweep was then adopted by the composed arms whose hand-written stacks computed exactly a
boundary array.

The sweep keeps no stack either. Once it has placed a position p, a monotonic stack would hold
exactly the chain p, boundary(p), boundary(boundary(p)), and so on down. The sweep walks that chain
through the boundary array it is filling, so it makes the stack's comparisons in the stack's order
and allocates only its result. The chain must end at something that is not a position, so a
caller's `none` that is a position is swept with -1 and written in afterwards. The first
static-dispatch version kept a `Stack<int>`; measurement removed it (§19.8).

### 19.8 What adoption was measured to cost

§12.4's rule held for every adoption here: measure a new axis before calling it free. Each
adopter's benchmark class ran out of process on the commit before its adoption and on the commit
after it, alternating the two sides class by class. The comparison is the composed arm's ratio to
its own baseline arm. The baseline's code is the same on both sides, so it absorbs whatever load the
machine carried while each side ran. When the baseline itself moved more than the change being
judged, the pair was re-run rather than read.

Three results changed the code instead of being recorded:

- `NearestBoundary` kept its pending positions in a `Stack<int>`, and four adopters regressed at one
  size or both, CountSubmatricesWithAllOnes by up to 51%. Now that it follows the boundary chain
  (§19.7), eight of the twelve benchmark classes that run it are 13–57% faster than before the
  adoption. CountSubmatricesWithAllOnes, NextGreaterNodeInLinkedList and OddEvenJump are no slower.
  NextGreaterElementI is faster at n = 200. At n = 1000 it read 22–30% slower in two passes, but
  on that same arm the chain then measured 16% faster than the stack. That reading is the
  machine's load (below), not the sweep.
- `PascalTriangle` reduced every entry with a 64-bit `%`, even in exact mode, where no entry ever
  reaches the modulus. NumberOfIntegersWithPopcountDepthEqualToKI ran at up to 2.3 times its own
  builder's time. A conditional subtraction replaced the division, and that arm is back inside
  BenchmarkDotNet's error bars.
- CheckIfDigitsAreEqualInStringAfterOperationsI read its one row off a triangle built for it: every
  row up to n − 2, 43 KB where its rolling row allocates 848 bytes, and up to 59% slower even after
  that fix. It has its rolling row back, the case `PascalTriangle`'s own complexity law names.

`CompressedCoordinates` made both of its measured adopters faster, CountNumberOfTeams by 17–20% and
ReversePairs by 5–16%, and cut their allocation by half or more. The deque, the predicate search,
the BFS facade and `PrefixSums` showed no cost on SlidingWindowMaximum, JumpGameVI, KokoEatingBananas,
OpenTheLock and MinimumSizeSubarraySum. The deque's other adopters read within this machine's noise.
LongestContinuousSubarrayWithAbsoluteDiffLessThanOrEqualToLimit, which keeps two deques, measured
anywhere from −1% to +19% across passes. The JIT's own output settled it instead. With everything
inlined, the hot path compiles to 306 instructions against the hand-written version's 298, with no
calls in either and the same twelve divisions, so the spread was the machine. Those divisions were
`CircularBuffer`'s `%` on every index, shared by every deque and queue alike. Its length is now
always a power of two, so an index is masked instead. Across the measured hot paths, the JIT output
went from 68 divisions to none, and the five `MonotonicDeque` adopters measured ran 7–40% faster.

One solution declines the deque, on measurement. MaximumSumOfMNonOverlappingSubarraysII ranks
starts by a two-part key, transition then fewer subarrays, and every key can be read back from its
arrays by position. Stored beside the position, the key made 24-byte entries and a tuple comparison
the JIT does not fold, about 10% slower in two passes. So that solution keeps a deque of bare starts.
Part I, whose key is one `long`, moved onto `MonotonicDeque` at no cost.

`MinMaxWindow` was a class at first, and it cost what any object between a caller and its storage
costs (§20). LC 1438's sweep went from 23 pointer loads to 32, and its composed arm ran 8–18% slower
in two passes, LC 2762's 16–42%. As a readonly struct it is no object of its own. But three adopters
kept it in a field of a nested class, so the JIT reached it by address and reloaded both deques on
every pass of the shrink loop. They now hold it in a local and pass it by value, as the other three
already did. The sweeps compile to fewer instructions than the hand-written pair and at most two more
pointer loads, with no calls: 321 instructions against 334 for LC 1438, 319 against 325 for LC 2762,
and 338 against 342 for LC 3835. The timings that followed showed no consistent cost. One pass read
LC 2762 10–15% slower, and LC 1438 19% slower at one size while its baseline arm moved 44%; the
next pass repeated neither.

Two adopters allocate more for the same time. CountSubmatricesWithAllOnes takes a boundary array per
row where its stack stayed shallow (109 KB to 188 KB at n = 150). ShortestSubarrayWithSumAtLeastK
keeps a (position, key) pair per deque entry where it kept an index, about twice the bytes.

On this machine, with other sessions building, identical code on both sides of one pass read 45%
apart, and NextGreaterElementI's composed arm measured 16 µs in one pass and 28 µs in another. So
no single pass is read alone. Every change that moved code was checked by a later pass. A result
that only one pass showed is reported as noise, not as a finding.

When a timing moves and the code did not change enough to explain it, the JIT's own inlining
decisions usually do. A call the JIT judged an unprofitable inline, or one through an interface it
cannot resolve, stays a call in the hot loop. The benchmark runner reads those decisions:

```bash
dotnet run -c Release --project DSAExperimentation.Benchmarks -- inlining WalkingRobotSimulation SetObstacles 3 MaxDistanceSquaredByObstacleSet
```

It builds the class the way BenchmarkDotNet does, with its first `[Params]` values and its
`[GlobalSetup]`, and calls the arm for the given seconds until it tiers up. It then prints the failed
inlines of the last compile, of the arm itself or of the method named last. It reads them in its own
process from the runtime's event source (the Jit and JitTracing keywords), which an out-of-process
BenchmarkDotNet run cannot report. It needs a Release build, since Debug code is compiled without
the inlining it would report.

### 19.9 Moving composed arms onto the engines

The owner asked for every composed arm that fits an engine to move onto it (2026-11-02). Three
read-only surveys covered about 250 traversal-heavy folders. About thirty fit cleanly. Most of the
rest need something the engines did not have: an in-order position, an early exit or a stop at a
target, a breadth-first search from several sources, state passed down from the root, weighted
edges, or a binary node's null children (`BinaryTreeChildren` compacts them away).

Each conversion was measured before and after, and an arm the engine made slower stayed
hand-written. The measurements sorted the arms by what they replaced:
- **Lambda and iterator traversals win.** Arms on `DepthFirstSearch.Traverse`'s successor lambda,
  or on LINQ over the nodes a traversal returned, got faster on the engines. They lost an
  allocation per node and a delegate call per edge. OperationsOnTree went from 204 to 6.2 us,
  DetonateTheMaximumBombs ran 33 to 50% faster, and NumberOfIslands 22 to 28%.
- **Direct recursion does not.** `DepthFirstTraversal`'s hooks cost 15 to 40% over a hand-written
  recursion on a tree (SerializeAndDeserializeBST and CheckIfDfsStringsArePalindromes, both
  reverted). `TreeFold` cost four to five times the recursion it stands for on HouseRobberIII, and
  the recursive evaluation allocated a child-results array per node. §19.10's span fold removed the
  array (65 KB a run down to 328 bytes), and the fold still costs three to four times the recursion.
  BalancedBinaryTree and BinaryTreeMaximumPathSum, the conversions it held back, were not retried.
- **Unit-weight Dijkstra was a breadth-first search.** JumpGameII and JumpGameIV ran
  `ShortestPath.Dijkstra` over graphs whose every edge weighs 1. `BreadthFirstDistances` answers
  the same distances 30 to 50% faster.
- **Large grid components pay per edge.** `GridChildren` built a new `GridNode` for every neighbour
  it listed, reached or not, and the visited set hashes each one. That outweighed the lambda's cost
  on big components: CountSubIslands at 120 x 120, NumberOfClosedIslands at 100 x 100 and
  MakingALargeIsland at 60 x 60 measured 22 to 98% slower. MaxAreaOfIsland,
  MaximumNumberOfFishInAGrid and CountIslandsWithTotalValueDivisibleByK moved onto
  `ConnectedComponents.ReduceEach` and went back: a pass on a quiet machine read them 7 to 54%
  slower at one size or both, where the first pass had overlapped a peer's unlocked benchmark job.
  All six were retried on `NodePerCell` (below), in two passes against 222694e2 (2026-11-03). They
  allocate 42 to 72% less, but only CountIslandsWithTotalValueDivisibleByK got faster, by 1 to 10%,
  and it now reads `ReduceEach`. The other five stay on `DepthFirstSearch.Traverse`'s lambda over
  value-tuple cells, since both passes read them slower at their larger board: CountSubIslands 36
  to 41%, MakingALargeIsland and NumberOfClosedIslands about 20 to 25%, MaxAreaOfIsland 15 to 20%
  and MaximumNumberOfFishInAGrid 9 to 10%. MinimumNumberOfDaysToDisconnectIsland, which builds a
  `Grid` for each of its n^2 queries, and JumpGameIII, whose hop graph cost far more to build than
  the few nodes its walk needs, stay hand-written.

Three pieces came out of it. `ConnectedComponents.ReduceEach` reduces each component on its own:
the multi-root walk `Count` rides, returning every component's state. `NodeCountReduceAlgebra`
counts the nodes a walk enters, which is a component's size. `Grid.Where` builds a board from an
`IGridCellFilter`, and `Grid.PassableNodes` lists its open cells as the roots a whole-board scan
needs; NumberOfIslands and SurroundedRegions read them. LastDayWhereYouCanStillCross and
CountIslandsWithTotalValueDivisibleByK read `ReduceEach`. The grid takes the choice between one node
per cell and a new one per neighbour as a type argument, an `IGridNodeSource`. `NodePerCell` hands
out the board's one node for each cell, made the first time it is asked for, and `NodePerStep` a
new node each time. The whole-board flood fills
take `NodePerCell`: NumberOfIslands, LastDayWhereYouCanStillCross, MatrixCellsInDistanceOrder,
FindTheSafestPathInAGrid, MaximumNumberOfPointsFromGridQueries and ConnectedComponentsOrder. They
allocate 16 to 44% less, and none runs slower. The largest boards run up to 60% faster
(ConnectedComponentsOrder's depth-first arm at 400 x 400). SurroundedRegions walks only the regions
touching the border, so it takes `NodePerStep` and allocates no table. A reduce algebra is static, so a value it reads, such as a cell's fish or
grid1's land, travels in the state it threads.

### 19.10 Stopping early, several roots, in-order, and a fold without an array per node

The survey in §19.9 named what the engines lacked, and the owner opted into closing three of the
gaps (2026-11-02). The design question was where each one belongs, and each went to the piece that
already knew the answer:

- **Stopping early belongs to the state.** `IReduceAlgebra` gains `IsDone(TState)`, because a
  search that has found its target or a count that has reached k is what knows the walk can end. A
  walk that is told true makes no further `Enter` or `Exit` call. Asking has a price. Every node type
  here is a reference type, so the walks run as shared generic code, and a call to the algebra's
  static members is a runtime lookup the JIT cannot inline. Asking `IsDone` at every node measured 14%
  on OperationsOnTree, whose algebra never stops. So an algebra also says whether it can stop at all,
  through `StopsEarly`, which defaults to false. The order strategy reads it once per walk and picks
  the walk's `IStopCheck` from it: `NoStopCheck` or `AlgebraStopCheck`. That check is a type argument,
  not a flag. A flag passed down the recursion still measured 20% on OperationsOnTree's one-node
  walks, while the `NoStopCheck` instantiation is the walk with no check in it at all. The read itself
  folds to a constant wherever the strategy inlines into a caller with exact types, as
  OperationsOnTree's does. Hooks still observe a whole walk: a `DepthFirstTraversal` or
  `InOrderTraversal` hook has no `IsDone`, and an arm that can stop uses a reduce.
- **Several roots belong to the order.** `Reduce.GraphFromAll` takes several roots, and each order
  strategy says what "at once" means through `EvaluateFrom`. Breadth-first puts every root at depth 0,
  one frontier, so a node's depth is its distance from the nearest root: the multi-source search.
  Depth-first walks the roots in turn over one visited set. `BreadthFirstDistances.FromAll` names the
  multi-source distance map, and `BreadthFirstDistances.To` names the stopping search for one target.
  `GridShortestPath.Distance` and the new `HammingDistances.To` use it. A stopping algebra still pays
  the lookup at every node, so `To` wins only when the target is reached well before the search would
  end. JumpGameII's target is the last index the search reaches. MinimumNumberOfVisitedCellsInAGrid
  and MinimumOperationsToEqualizeBinaryString measured slower at one size in two passes. All three
  keep `From`. `FromAll` has no adopter. RottingOranges and ZeroOneMatrix measured 9 to 10 and 16 to
  18 times slower through it on a `NodePerCell` grid, at 5 to 25 cells a side in two passes, and
  allocated 8 to 13 times as much: their arms index an `int` array, where `FromAll` fills a
  `Dictionary` keyed by `GridNode`. Both keep their hand-written multi-source search.
- **In-order is a third order.** `InOrderReduceOrder<TValue>` walks a `BinaryTreeNode`'s left
  subtree, the node, then the right subtree, and fires `Enter` between them, the way breadth-first
  fires `Enter` alone. It reads `Left` and `Right` directly, since a compacted `IChildren` view cannot
  say which side a child was on. DataStructures' `InOrderTraversal` stays as it is:
  `BinarySearchTree` walks itself with it, and the layering lets DataStructures read no `Reducing`
  type (§17.6a). DesignMovieRentalSystem stops at the fifth key through it, and
  MinimumDepthOfBinaryTree stops at its first leaf in breadth-first order. KthSmallestElementInABST
  measured slower through it at 500 nodes in two passes (614 to 820 ns) and keeps its
  `InOrderTraversal` hooks.
- **A fold's child results live on one stack.** `IFoldAlgebra.Combine` takes `ReadOnlySpan<TResult>`
  rather than `IReadOnlyList<TResult>`. The recursive evaluations keep every open node's child results
  on one `FoldStack`: a node pushes each child's result, hands `Combine` its own span, and pops them.
  The iterative evaluation reuses one buffer. A fold no longer allocates an array per node, and
  `Combine` reads its children with no interface call. The span is valid for the one call, which a
  pure `Combine` never outlives.

Measured with bdn-ab against c7f8f09c (2026-11-02), on the composed arm. In two or three passes
each: OpenTheLock 21–38% faster with half the allocation, WordLadder 14–32%, HouseRobberIII 19–38%
from the span fold, DesignMovieRentalSystem 84–85% at 500 movies, and MinimumDepthOfBinaryTree 8–16%
at 100,000 nodes. At 50 movies and 1,000 nodes the passes were mixed within error. In one pass:
SlidingPuzzle 95–99% and JumpGameIV 91% at 200, both past the 45% this machine's noise has reached
(§19.8); SmallestIntegerDivisibleByK, MinimumGeneticMutation and DiameterOfBinaryTree 6–39%, which
says only that they are no slower, though DiameterOfBinaryTree's allocation fell from 64 KB to 88
bytes at 2,000 nodes. A walk that never stops costs what it did: NumberOfIslands and
MatrixCellsInDistanceOrder read level in every pass, and OperationsOnTree read between 4% slower
within error and 34% faster in two passes.

## 20. Worked example: the queue family, configurations of one buffer

A queue, a deque, a bounded queue and a bounded deque are one structure configured four ways, the
way Graph narrows to DAG and then to Tree (§3): constraints and invariants injected into a general
structure make the specialized ones. The general structure is `CircularBuffer<T, TCapacity>`, a
wraparound array read and written at both ends, holding as many elements as its capacity allows.

| Structure | Capacity | Operations | Adds |
| --- | --- | --- | --- |
| `CircularBuffer<T, TCapacity>` | the witness | push, peek and pop at both ends | `IsFull` |
| `Deque<T>` | `UnboundedCapacity` | all of them | a push never refused, so `PushFront`/`PushBack` return nothing |
| `BoundedDeque<T>` | `BoundedCapacity(k)` | all of them | `TryPushFront`/`TryPushBack`, refused when full |
| `Queue<T>` | `UnboundedCapacity` | push at the back, pop at the front, peek at both | FIFO |
| `BoundedQueue<T>` | `BoundedCapacity(k)` | as `Queue` | `TryEnqueue`, refused when full |
| `MonotonicDeque<K, TOrder>` | `UnboundedCapacity` | its own push, eviction and front | its window order (§19.1) |

**Two kinds of constraint, entered two ways.** A constraint that changes how an operation behaves
is a type argument. One that changes which operations exist is a facade. Capacity is the first
kind: a bounded push is the same push, which may now be refused. Which ends exist is the second: a
queue has no push at the front, and a C# type argument cannot remove a member. A FIFO witness could
only make the front push throw, a refusal at run time of what the type system can refuse at compile
time. The graph tiers draw the same line. A topology witness changes what a walk may assume, while
the binary tree's own node type is what fixes which children a node has.

**A configuration that keeps every operation is the buffer.** `Deque<T>` derives from
`CircularBuffer<T, UnboundedCapacity>` and `BoundedDeque<T>` from `CircularBuffer<T,
BoundedCapacity>`, each with its argument fixed, so a deque is the buffer itself rather than an
object wrapping one. A deque's inherited `TryPushFront` never refuses and its `IsFull` never answers
true, which is what an unbounded capacity means. Only the configurations that remove operations,
the two queues, compose the buffer behind a facade.

**The capacity is an instance witness.** Its set is closed, unbounded or bounded at k, like
`IHeapOrder`'s two orders (§9.2). But a bound carries a number known only at run time, so the
witness is a struct value the buffer holds in a field, not a `static abstract` member. The JIT
still specializes the buffer per witness type, and `UnboundedCapacity.IsFullAt` answers a
constant `false`, so a deque or queue carries no capacity check at all.

**Refusing is all a bound does.** A full `BoundedDeque` refuses a push and keeps everything it
holds. A ring buffer that overwrote its oldest element to admit a new one would be evicting, which
§17.6b makes a mechanism. LC 3508's router keeps such a buffer, and it stays out of this family.

**The array stays a power of two.** The bound is checked against `Count`, separately from the
array's length: a capacity of 3 holds three elements in a four-slot array, and every index is still
masked rather than divided (§19.8). `Stack` stays out of the family. Its representation is the flat
`DynamicArray`, and a configuration varies a constraint over one representation, not the
representation itself.

**What the JIT made of it.** The composition was read in the JIT's output with tiered compilation
off before it was measured, on loops that push, peek and pop a `Deque<int>`, a `Queue<int>` and a
`Queue<string>`, and slide a `MonotonicDeque` window. No loop divides, before or after. One path
regressed. Inside `MonotonicDeque.Push`, the JIT inlined the dominance pops and, out of budget, left
`IsFull` and then the whole push as a call on every element, where the old `Push` had inlined the
push and called out to pop. The pushes now ask the witness directly and are always inlined.

**What measuring it found.** BenchmarkDotNet runs tiered code with dynamic PGO, and that output was
no guide to it: it had put the new code ahead on every loop. Measured, four drafts regressed, none
in the work done: three in how the buffer's memory is reached and one in what the inliner was
charged for. Each was fixed by taking something away:

- **A struct storage.** The buffer first kept its array in a Storage struct held in a field, split
  from its operations as §14's containers are, to save an object. The struct's methods reach its
  fields through `ref this`, and the JIT reads fields reached that way as memory any array store
  might overwrite. A loop dequeuing an `int` re-read `Count`, `_head` and the array on every
  element, and decremented `Count` in memory. LC 933's queue window ran 57% and 106% slower in two
  passes.
- **A storage object.** As a class, the storage put one more object between every configuration
  and its array. The deques, then facades over the buffer, were two objects from it where `Deque`
  had been one, and LC 641's deque at capacity 8 took 1.29 µs where it had taken 1.06 µs. LC 622's
  bounded queue, whose every operation arrives through an interface call, ran 10–18% slower at
  capacity 8 in three passes.
- **A cold `Grow`.** Growth had moved into a `Grow` that was never inlined, so that a push would be
  a compare and a store. Tiered code had inlined the old growth whole, so a breadth-first search
  over a `Queue<int>` made no call in its loop and loaded the queue's objects once, outside it. With
  `Grow` a call inside the loop, the JIT kept no field across it. LC 310's leaf peeling re-read the
  queue, the buffer and the storage on every enqueue and dequeue, and ran 11–24% slower in two
  passes, and LC 542 5–10%.
- **A rounded constant.** The buffer rounded its first length, `ArrayGrowth.InitialCapacity`, up to
  a power of two, and each grown length inside `Grow`, though both already were one. The rounding
  cost nothing at run time, since the JIT folds it, but the inliner charges each method for the IL
  it inlines, against a fixed budget. LC 1797's benchmark compiles into one method that builds an
  `ExpiringSet` and replays the whole script through it. The configurations' constructors pass
  their capacity witness along, a few bytes more IL than the constructors they replaced, and that
  method ran out of budget sooner. The JIT's own inlining events gave the reason as "inline exceeds
  budget": the `(key, expiry)` tuple's constructor and `Queue.Enqueue` became calls on every
  `Generate`, the tuple through a copy on the stack, and the arm ran 8–19% slower in both passes.
  With no rounding, both are inlined again.

So the deques derive from the buffer, the buffer owns its array with no Storage half, `Grow` is
always inlined, as the old growth was, and no length is rounded. A `Deque` reaches its array
through one object where it went through two, and a `Queue` through two where it went through
three.

**What the finished family measured.** Two passes over the 24 benchmark classes whose composed arm
builds a deque, a queue or a monotonic deque compared each arm's ratio to its baseline before and
after the family, and two more went over the two classes those left in doubt. No arm ran slower in
both passes. LC 1797's arm, slower in every pass while the buffer rounded, read −9% and +15% at 200
and +30% and −5% at 999, then −8% to +12% in the two passes after; the +30% came from a run whose
own spread was 11%. LC 542 at 10 read +7% twice, inside the error, and +0% and −1% when run again.
Many arms ran faster in both passes: LC 641's bounded deque at capacity 512 by 23% each time, LC
622's bounded queue at 512 by 9% and 22%, LC 1499's window by 14–21% at both sizes, LC 1425's by
21–23% at 2,000, and LC 239's by 12–18% at 2,000. LC 2073's report reads the other way only because
its queue simulation is the baseline arm: that arm got 8–15% faster, so the closed form's ratio to
it rose.
