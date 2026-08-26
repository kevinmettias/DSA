# Architecture

## 1. Purpose

This document formalizes a classification that already governs `DSAExperimentation/Graph/**`,
even though nothing in the repo says so explicitly until now. It exists so that future work —
starting with `DSAExperimentation/Collections/**` — follows the same discipline deliberately
instead of by accident.

## 2. The three axes

Every data structure in this repo is designed along three independent axes:

- **Representation** — physical storage. Contiguous array, pointer/list-linked, arena+indices,
  sparse slot array, geometry-computed. This is *where things live* and *how they're reached* —
  and, just as importantly, *what reaching them costs*. That cost is a real contract even though
  C#'s type system can't state it: `IChildren<TNode>.Get(int)` is one syntactic contract, but
  `ListChildren.Get` is O(1) (a `List<T>` indexer) while `SparseArrayChildren.Get`/`GridChildren.Get`
  are O(k) bounded scans (trie slots / the 4 grid directions). Harmless today, since nothing in
  `Graph/**` assumes O(1) `Get` — every consumer iterates the whole `Count` regardless — but it's a
  real, currently-unstated assumption. §8 names it explicitly.
- **Topology** — the admissible relation/shape. General graph, DAG (acyclic), tree (acyclic +
  unique ancestry), heap-order, BST-order. A stronger topology promise lets algorithms skip
  runtime defense (cycle checks, visited-tracking, rebalancing) at compile time instead of
  paying for it at run time.
- **Operations** — the algorithm/API surface built on top of a Representation constrained by a
  Topology.

As of the reorg documented in §13, this classification is also the repo's primary *physical* axis:
every file lives under a top-level `DataStructures/` or `Algorithms/` folder according to which of
these three it is (Representation and Topology under `DataStructures/`, Operations under
`Algorithms/`), not under a domain-first folder with the axes mixed together inside it. §3–§12
below describe each domain's *reasoning* — why a given capability is Representation vs. Topology
vs. Operations — using the paths and folder names as they stood when each section was written,
before that physical reorg happened; §13 is where to look for where things live today.

A fourth axis, "Invariants," was considered and folded into Topology: a topology's
admissible-relation-set already *is* its invariants (acyclicity, unique ancestry, heap order,
BST order are all statements about which relations are legal), so tracking it separately would
just be re-describing Topology from a different angle.

**The illustrating example**: a binary heap has tree *topology* (each slot has at most two
children, no cycles, one root) but is conventionally given array *representation* instead of
pointers — `parent(i) = ⌊(i-1)/2⌋`, `left(i) = 2i+1`, `right(i) = 2i+2`. Same logical shape,
completely different physical layout and cost profile. This is exactly why Representation and
Topology have to be tracked as separate axes rather than conflated — and it stops being
hypothetical in §4 below, where it becomes `Collections/Heap`.

## 3. Worked example: `Graph/**`

> Paths below are as they stood before §13's reorg. See §13 for where each file lives today.

### 3.1 Representation

- [`Graph/Contracts/Ordering/IChildren.cs`](DSAExperimentation/DataStructures/Graph/Contracts/Ordering/IChildren.cs) —
  an indexable view over a node's children. Deliberately *not* `IReadOnlyList<TNode>`: `Get` is a
  named method, not an indexer, specifically so implementations stay thin structs that JIT-specialize
  to direct, non-virtual calls with no boxing.
- [`ListChildren.cs`](DSAExperimentation/DataStructures/Graph/Contracts/Ordering/ListChildren.cs) — pointer/`List`-backed.
- [`SparseArrayChildren.cs`](DSAExperimentation/DataStructures/Graph/Contracts/Ordering/SparseArrayChildren.cs) —
  fixed slot-array backed (e.g. a trie's 26-letter alphabet), scans past nulls instead of storing a
  materialized list.
- [`Graph/Algorithms/Grids/GridChildren.cs`](DSAExperimentation/DataStructures/Graph/Grids/GridChildren.cs) —
  a third representation: children aren't stored at all, they're computed on demand from geometry
  (row/col offsets filtered by what the grid says is in bounds and passable). This is the direct
  precedent reused by `Collections/Heap`'s index arithmetic in §4.
- `IEdges.cs`/`IChildOrder.cs` round out the representation contracts: edge-aware children
  (`ListEdges`, `EdgeTargets`) and order-independent-of-topology traversal (`NaturalChildOrder`,
  `ReverseChildOrder`, `ReversedChildren` — allocation-free reversal via index arithmetic over an
  existing `IChildren`).

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
`DagFold.cs` vs. `CheckedFold.cs` — three tiers of the same catamorphism, each paying only the
defense its topology tier doesn't already rule out. `IEdgeTopology.cs`/`EdgeTopologyAsGraphTopology.cs`
show topology-to-topology *composition* (projection, not inheritance); `GridTopology.cs` shows a
topology witness sitting over an arithmetically-computed representation.

### 3.3 Operations

Two layers: generic **engines** (`Graph/Engines/Reducing/Reduce.cs`'s `Tree`/`Graph` entry points,
`Graph/Engines/Traversal/{BreadthFirst,DepthFirst,TopDown}/**`, `Graph/Engines/Walking/**`) and
user-facing **facades** built by closing those generics over concrete node/topology types
(`Graph/Algorithms/{Ancestry,Connectivity,Grids,Metrics,Paths,ShortestPaths}/**`).

[`IPathHeuristic.cs`](DSAExperimentation/Algorithms/ShortestPaths/IPathHeuristic.cs)/`ZeroHeuristic.cs`
is the template for "one algorithm, one injected axis, not two parallel implementations" —
`ShortestPath.Dijkstra` is `ShortestPath.AStar` closed over `ZeroHeuristic`. `Collections/Heap`'s
`IHeapOrder` reuses this exact idiom in §4.

## 4. Worked example: `Collections/Heap`

> Paths below are as they stood before §13's reorg. See §13 for where each file lives today.

`Collections/Heap` is the first non-Graph instance of this framework, and it doesn't transplant
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
| `Collections/Heap/IHeapOrder.cs` | Topology | The ordering invariant (min vs. max), generic over `T` only |
| `Collections/Heap/MinHeapOrder.cs`, `MaxHeapOrder.cs` | Topology | Closed-set witnesses of `IHeapOrder<T>` |
| `Collections/Heap/HeapArrayIndex.cs` | Representation | Pure `parent`/`left`/`right` index arithmetic — the array *is* a complete binary tree; parallels `GridChildren`'s coordinate arithmetic |
| `Collections/Heap/HeapArray.cs` | Representation | The growable backing store |
| `Collections/Heap/Heap.cs` | Operations | `Push`/`Pop`/`Peek` — the sift walk that combines the arithmetic and the comparison; owns neither on its own |

Representation contracts here are **domain-local**, not shared with `Graph.Contracts.Ordering`.
`IChildren`'s own doc comment already explains why: it exists as an interface only because Graph
needed two genuinely different shapes (`ListChildren`, `SparseArrayChildren`) from day one.
`Collections/Heap` has exactly one representation today, so it gets a concrete class
(`HeapArray<T>`), not an interface — see the recipe below for when that should change.

`Graph/Algorithms/ShortestPaths/ShortestPath.cs` is `Heap`'s first real consumer: Dijkstra/A*'s
frontier is `Heap<(TNode Node, TWeight Priority), ByPriorityOrder<TNode,TWeight>>`, where
[`ByPriorityOrder.cs`](DSAExperimentation/DataStructures/Graph/ShortestPaths/ByPriorityOrder.cs) projects
`IHeapOrder<T>` down to "compare `.Priority`, ignore `.Node`" — the same projection move
`EdgeTopologyAsGraphTopology` makes for `IGraphTopology`, just for a different domain's Topology
witness. Note this doesn't violate "domain-scoped contracts": `ByPriorityOrder` *implements*
`Collections.Heap`'s public `IHeapOrder<T>` as a client, the same way any consumer of a library
type would — it isn't Graph reaching in to redefine or reuse Heap's own Representation.

### 4.1 The other five structures

Each follows §5's recipe below rather than repeating Heap's exact shape — none of them has an
independently-pluggable Topology witness, because none needed one:

| Structure | Representation | Operations | Notes |
| --- | --- | --- | --- |
| `Collections/DynamicArray/DynamicArray.cs` | itself (a manually-doubled `T[]`) | `Add`/`Insert`/`RemoveAt`/`Get`/`Set` | No Topology axis at all — the only invariant is "how it's stored," which §5 step 2 classifies as Representation, not Topology |
| `Collections/Stack/Stack.cs` | `DynamicArray<T>` (composed, not duplicated) | `Push`/`Pop`/`Peek` | A stack is a sequence plus a LIFO *access constraint* — an Operations-level restriction over an existing Representation, not a new physical layout |
| `Collections/Deque/CircularBuffer.cs` + `Deque.cs` | a wraparound array (`_head`/`Count` modulo length) | `Push`/`Pop`/`Peek` at both ends | A second, distinct Representation (wraparound, not flat) — earned by needing O(1) at *both* ends, which `DynamicArray`'s flat layout can't give the front |
| `Collections/Queue/Queue.cs` | `Deque<T>` (composed) | `Enqueue`/`Dequeue`/`Peek` | FIFO is Deque restricted to one end each — the same "sequence + constraint" relationship Stack has with `DynamicArray`, just against the wraparound Representation |
| `Collections/HashMap/HashMap.cs` (+ `HashMapEntry.cs`) | bucket array + separate-chaining entries array with a free list | `Get`/`Set`/`Remove`/`ContainsKey` | The associative primitive from §2 in code: no Topology witness, because "at most one value per key" is what a map *is*, not a variable choice the way min-vs-max-heap is |
| `Collections/Set/Set.cs` | `HashMap<T,bool>` (composed) | `Add`/`Contains`/`Remove` | Backed by `HashMap` the way `java.util.HashSet` is backed by `HashMap<E,Object>` — membership is "is this key present," so the value is an unused placeholder |

`Stack`/`Queue` composing `DynamicArray`/`Deque` rather than each managing their own array is itself
the philosophical point from earlier in this conversation made concrete: *"a stack isn't a unique
physical structure — it's a sequence plus a LIFO access constraint."* Representation is shared
because the constraint, not the storage, is what varies.

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
   *representation* (as `DynamicArraySequence` does with `Collections.DynamicArray`, §9.4) is a
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
   This is the one place this reorg changes future behavior: it decides where a *new* file gets
   created, not just where old ones were moved.
7. **Not everything that composes a Representation and adds checks belongs in `Algorithms/`.**
   When an Operations type is hardwired to exactly one concrete Representation — no interface, no
   second implementation possible — it isn't decoupled from a data structure the way
   `ShortestPath`/`MergeSort`/`Reduce`/`Fold` are (each generic over an interface with genuine
   multiple implementations); it *is* the data structure, with its bounds-checking layer split out
   for the reasons in step 4. That pairing co-locates under `DataStructures/<Structure>/`
   (`Heap.cs` beside `HeapArray.cs`, `Stack.cs` beside the `DynamicArray.cs` it hardcodes) — never
   under `Algorithms/`. Reserve `Algorithms/<Utility>/` for the type that's actually generic over a
   capability interface with real substitutability (§13.5).

This is what already happened, in order, for `Searching/BinarySearch` (§9); `Collections/Heap` (§4)
and `Graph/**` (§3) arrived at the same shape but retrofitted, with the Topology witness pulled out
after the fact rather than named first. Both routes land in the same place here because the
domains are small enough to fix in hindsight — but running the checklist forward is what keeps
that true as things grow.

## 6. Gate-compliance notes

This repo's Nomos code-standards gate (`standards.json`, `suppressions.json`, engine at
`kevinmettias/code-standards`) already shapes every file in `Graph/**` toward: one public type per
file, small single-cluster classes, closed-set modeling over loose bool/enum flags, and composition
over inheritance. `Collections/**` follows the same shape, and where the gate still flagged
something, the fix was almost always real (a shared `ArrayGrowth` constant instead of three
independent copies of the same tuning numbers, `HashMap`'s `Set`/`Remove`/`Grow` split into named
helpers, a `HeapArrayIndex.RightChild` expressed as `LeftChild + 1` instead of a bare literal).
The remaining `suppressions.json` entries for `Collections/**` are judgment calls already
precedented elsewhere in this repo: `check-class-size`'s "two disjoint call-graph clusters" fires on
every small ADT wrapper here (`Heap`, `DynamicArray`, `CircularBuffer`, `Stack`) because their
grow-path and remove/access-path methods never call each other, not because either hides a second
responsibility; `check-literals`/`check-function-cohesion`/`check-responsibility-extraction` on test
files follow the exact reasoning already recorded for `ContiguousGroupBufferTests.cs`/
`TraversalTests.cs`; and `check-test-coverage`'s attribution-by-best-name-match gap (already waived
throughout `Graph/**`) recurs for a handful of generically-named members (`Count`, `Get`) exercised
only transitively. `Collections/DisjointSet/DisjointSetForest.cs` (§10) joins the same
`check-class-size` precedent for the same reason (its parent-path and rank-path methods never call
each other); `Sorting/MergeSort.cs` (§11) hit two genuinely real findings instead — a five-parameter
`SortRange` and a six-parameter `Merge`, both fixed by grouping the range into `SortBounds` (§11.3)
rather than waived, and a five-section `Merge` shortened to a single counted loop (§11's
`MergeRunsIntoBuffer`) instead of three separate merge/drain loops.

## 7. Resolved asymmetries

- **Fixed**: `DSAExperimentation.Tests/Engines/**`, `Algorithms/**`, and `Contracts/Ordering/**`
  used to drop the `Graph/` prefix segment the main project uses under `DSAExperimentation/Graph/**`,
  a leftover of the `935c3f2` restructuring that predated a full test-tree migration. They now live
  under `DSAExperimentation.Tests/Graph/**`, mirroring the main project exactly.
  `Collections/Heap`'s tests instead mirror `Buffers/ContiguousGroupBufferTests.cs`'s convention
  (prefix retained), since `Buffers` — not `Graph` — is the correct sibling-domain precedent for a
  small standalone `Collections` structure.
- **Fixed**: `Graph/Algorithms/ShortestPaths/ShortestPath.cs`'s Dijkstra/A* now use
  `Collections/Heap`'s `Heap<T,TOrder>` instead of the BCL `PriorityQueue<TNode,TWeight>`, via a
  `(TNode Node, TWeight Priority)` element ordered by a priority-only `IHeapOrder` witness — no
  change to `Heap`/`HeapArray` themselves was needed.
- **Fixed**: `IPathHeuristic.cs`'s contract used to require only that a heuristic be *admissible*
  (never overestimates). That's weaker than what `ShortestPath.Traverse`'s settle-once design
  actually needs — *consistent* (`h(u) <= cost(u,v) + h(v)` on every edge), which is what guarantees
  a node's first pop already carries its true shortest distance. An admissible-but-inconsistent
  heuristic can still bias the search, but on some graph shapes can make `Traverse` settle a node at
  a non-optimal distance with nothing here to catch it. The contract now says "consistent" and
  explains why; `ShortestPath.cs` also now documents its non-negative-edge-weight precondition,
  which neither `Dijkstra` nor `AStar` checks or ever did.

## 8. Capability contracts: binding algorithms to what they need, not what they're given

An algorithm can be isolated from a *concrete* data structure, but not from the structural
properties it actually depends on. Precisely: an algorithm requires a set of operations `O` and a
set of laws `L` — semantic invariants *and* complexity guarantees — and any representation that
implements `O` while satisfying `L` is a valid model for that algorithm. The goal is binding to the
weakest sufficient requirement, not to a concrete type.

There are three kinds of independence here, in decreasing order of how safely they can be assumed:

- **Semantic independence** — already how this repo works, and the safest kind.
  `IGraphTopology<TNode,TChildren>.GetChildren` doesn't care whether children come from
  `ListChildren`, `SparseArrayChildren`, or `GridChildren`; `IPathHeuristic.cs`'s own doc comment
  (requiring a *consistent* heuristic, not merely an admissible one — see §7's entry on why) is this
  repo's existing example of a stated semantic *law* — `L` already has precedent here, even though
  its complexity half doesn't yet.
- **Implementation independence** — often true, not universally guaranteed. The same `Reduce`/
  `Fold`/`Walk` code already runs unchanged over every `IChildren` implementation in this repo. It's
  not a law, though — a future structure could need a genuinely different optimal algorithm per
  representation (dense vs. sparse matrix multiplication is the classic case elsewhere), so this
  independence is a frequent convenience, not something to assume by default.
- **Performance independence** — the dangerous one, and the one this repo has a live example of.
  `Collections/Heap`'s `Heap<T,TOrder>` push/pop is O(log n) *only because*
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

## 9. Worked example: `Searching/BinarySearch`

> Paths below are as they stood before §13's reorg. See §13 for where each file lives today.

`Searching/BinarySearch` is this repo's first non-Graph, non-Collections domain, and its first
algorithm bound to a Representation contract it defines for itself rather than one it's handed.
Where `Collections/Heap` needed one new Topology witness (`IHeapOrder`) over an already-obvious
Representation (a flat array), binary search needs the opposite: no Topology witness at all
(sortedness isn't a variant to choose between — see §9.1), but a brand-new Representation contract,
because "the sequence being searched" is the one part of this problem genuinely open to more than
one physical layout.

| File | Axis | Why |
| --- | --- | --- |
| [`Searching/IRandomAccessSequence.cs`](DSAExperimentation/DataStructures/Sequence/IRandomAccessSequence.cs) | Representation | An indexable view over an already-sorted sequence, generic over the element type alone — `Find`'s only physical-layout requirement |
| [`ArraySequence.cs`](DSAExperimentation/DataStructures/Sequence/ArraySequence.cs), [`DynamicArraySequence.cs`](DSAExperimentation/DataStructures/Sequence/DynamicArraySequence.cs) | Representation | Two witnesses satisfying the O(1)-`Get` obligation above, for two different physical reasons |
| [`BinarySearch.cs`](DSAExperimentation/Algorithms/Searching/BinarySearch.cs) | Operations | `Find` — the bisection loop; owns neither Representation witness |

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
`IHeapOrder<T>`. The discriminator is not how often the comparer gets called: `HashMap.Get`'s
`_comparer.Equals` already runs on every probe of a bucket's chain, exactly as often as `Find`'s
`comparer.Compare` runs on every bisection step, and `HashMap` is still correctly modeled as an open
runtime object. The discriminator is whether the space of valid choices is closed. `IHeapOrder<T>` is
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
`IRandomAccessSequence<T>` splits Representation into two implementations from day one (see §9.4),
so the assumption is stated three times instead, each from a different angle so none of them repeat:
`IRandomAccessSequence<T>`'s own doc comment states the *obligation* every implementation is expected
to satisfy — new relative to `IChildren<TNode>`, whose doc comment never had to say this, since
nothing in `Graph/**` depends on `Get`'s cost (§2). `ArraySequence<T>`/`DynamicArraySequence<T>`'s
doc comments each state *why their own backing store actually satisfies it*, for two different
physical reasons. `BinarySearch.cs`'s doc comment states the *consequence* for its own complexity
claim if some future implementation doesn't.

### 9.4 A second Representation, earned on day one

Unlike `HeapArray<T>` (§4 — one implementation, so no interface, per §5's recipe),
`IRandomAccessSequence<T>` launches with two implementations already, `ArraySequence<T>` and
`DynamicArraySequence<T>` — the interface's existence is earned from day one rather than deferred
the way `IChildren` waited for `SparseArrayChildren` to justify it.

`DynamicArraySequence<T>` composes `Collections.DynamicArray.DynamicArray<T>` directly, unmodified.
This is a new variant of cross-domain reuse, not a repeat of an existing one. It isn't
`ByPriorityOrder` implementing `Collections.Heap`'s public `IHeapOrder<T>` as a client (§4's
precedent — one domain implementing another's interface without ever touching its Representation).
It isn't `Stack`/`Queue` composing `DynamicArray<T>`/`Deque<T>` either (§4.1's precedent —
composition *within* `Collections/**`, the same domain both sides belong to). It's `Searching`, a
domain outside `Collections/**` altogether, adapting `Collections/DynamicArray`'s own Representation
*type* — not an interface belonging to it — into a Representation contract `Searching` defines for
itself. That's still consistent with §5's "never reuse another domain's Representation… contracts"
line: what's prohibited is reusing another domain's *contract* (forcing `DynamicArray<T>` itself to
implement `IRandomAccessSequence<T>`, coupling it to a domain it doesn't belong to), not consuming
its already-public concrete type as one of several witnesses for a contract the consuming domain
wrote itself. `DynamicArray.cs` stays completely unaware `IRandomAccessSequence<T>` exists;
`DynamicArraySequence<T>` absorbs all of the coupling on `Searching`'s side, one-directionally, with
zero changes to `DynamicArray.cs` — exactly the demonstration §8's "implementation independence"
argument calls for, using a structure this repo already had for an unrelated reason instead of a
contrived second example.

## 10. Worked example: `Collections/DisjointSet`

> Paths below are as they stood before §13's reorg. See §13 for where each file lives today.

`Collections/DisjointSet` is this repo's first equivalence-class structure, and its first
demonstration that a choice which *looks* exactly like Heap's Min-vs-Max axis can still fail to
earn a Topology witness. Where `Collections/Heap` needed a Topology witness over an obvious
Representation, and `Searching/BinarySearch` needed a new Representation over an already-settled
Topology, Union-Find needs neither: the representation is a concrete two-array forest, and the
one axis that looks variable turns out to be a Complexity law instead.

| File | Axis | Why |
| --- | --- | --- |
| [`Collections/DisjointSet/DisjointSetForest.cs`](DSAExperimentation/DataStructures/DisjointSet/DisjointSetForest.cs) | Representation | Parallel `parent`/`rank` `int[]` fields, fixed-size at construction — concrete, not an interface, since exactly one physical layout exists today (§5 step 3) |
| [`Collections/DisjointSet/DisjointSet.cs`](DSAExperimentation/DataStructures/DisjointSet/DisjointSet.cs) | Operations | `Find`/`Union`/`Connected` — path compression and union-by-rank, both hardcoded, not swappable |

### 10.1 Why the linking policy is a Complexity law, not a Topology witness

Union-by-rank vs. union-by-size vs. naive/arbitrary linking looks, on the surface, exactly like
Heap's Min-vs-Max choice: a decision re-made on every call (`Union`, here; `HasPriority`'s
comparison, there) that branches control flow. §9.1 already warns that call-frequency isn't the
discriminator — but the sharper test this example adds is: **does the choice change the
observable output, or only the cost of producing it?** `MinHeapOrder` vs. `MaxHeapOrder` changes
*which value `Pop()` returns* — a real, caller-visible semantic difference. No linking policy ever
changes *which partition `Find`/`Connected` report* for a given sequence of calls — every policy,
including a deliberately bad one, computes the identical equivalence classes. Only tree height,
and therefore amortized cost, differs: O(n) worst case with neither optimization, O(log n)
amortized with only one, O(α(n)) — effectively constant — with both path compression and
rank-guided linking applied together. That is §5's Complexity-law row verbatim ("`Get` assumed
O(1); a slower representation degrades big-O with no compiler error"), so the policy is hardcoded,
unconditionally, inside `Union` rather than exposed as a generic witness. Path compression itself
isn't even a law in this sense — no external caller could violate it, since nothing outside
`Find`'s own walk ever touches `SetParent` — it's pure Operations-internal mechanics, the same
status `Heap`'s `SiftUp`/`SiftDown` already have.

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

### 10.3 A first non-generic multi-file structure, and the C# wrinkle that comes with it

Every other multi-file `Collections/**` structure (`Heap<T,TOrder>`, `Stack<T>`, `Deque<T>`,
`HashMap<TKey,TValue>`) is generic, so a bare reference to its own name is never ambiguous: a
type-argument list (`Heap<int, MinHeapOrder<int>>`) is something no namespace can have, so the
compiler always resolves it as the type. `DisjointSet` is dense-int-indexed by design (§10.4), not
generic over an element type, so a bare `new DisjointSet(3)` reference from
`DSAExperimentation.Tests.Collections.DisjointSet` has no such disambiguator — namespace-member
lookup finds the test file's own enclosing namespace (a nested member of
`DSAExperimentation.Tests.Collections`) before any `using` directive is ever consulted, and fails
with "`DisjointSet` is a namespace but is used like a type." A same-named `using` alias doesn't
help, since that lookup order is unaffected by which names a `using` brings in; only a
*differently*-named alias (`DisjointSetOperations` in `DisjointSetTests.cs`) resolves it. This is
a real, repo-first consequence of keeping the Representation dense-int-indexed rather than generic
— not a naming mistake to fix, just the first time this shape has come up.

### 10.4 Dense-int-indexed today, not generic over `T`

`DisjointSetForest` is fixed-size at construction with no growth path — a real departure from
every other `Collections/**` Representation (`HeapArray`/`DynamicArray`/`HashMap` all start empty
and grow), because a disjoint-set forest's universe of ids is conventionally known upfront, the
same way CLRS's own "disjoint-set forest" is presented. It is not generic over an element type `T`
either: ids are plain `int`s in `[0, Count)`, assigned by the caller before any `Find`/`Union`
call — an unchecked precondition the same shape as `BinarySearch`'s sortedness (§9.1). This is a
deliberate minimal-generality choice, not an oversight: nothing in this repo yet needs a
`Dictionary<T,int>` id-assignment layer, and every other first-cut Representation here
(`HeapArray<T>`, `ArraySequence<T>`) started at the minimal generality its one known consumer
needed rather than the most general shape imaginable. If a generic-`T` consumer appears later (a
Kruskal's-MST algorithm under `Graph/Algorithms` would be the natural first one, mirroring
`ShortestPath` becoming `Heap`'s first real consumer per §4), the right shape is a `DisjointSet<T>`
*wrapper* composing this concrete type plus a `Dictionary<T,int>` — composition over redesign, the
same relationship `Stack`/`Queue` already have with `DynamicArray`/`Deque` (§4.1).

## 11. Worked example: `Sorting/MergeSort`

> Paths below are as they stood before §13's reorg. See §13 for where each file lives today.

`Sorting/MergeSort` is this repo's second checklist-first algorithm after `Searching/BinarySearch`,
and the first built specifically to show that "the same capability shape" is not grounds to reuse
another domain's Representation contract. Both algorithms need indexed access to a sequence; only
one of them needs to write it back.

| File | Axis | Why |
| --- | --- | --- |
| [`Sorting/IIndexedSequence.cs`](DSAExperimentation/DataStructures/Sequence/IIndexedSequence.cs) | Representation | Indexed get/set view, generic over the element type alone — earns the interface on day one via two witnesses, same §5-step-3 exception `IRandomAccessSequence<T>` used |
| [`Sorting/ArrayIndexedSequence.cs`](DSAExperimentation/DataStructures/Sequence/ArrayIndexedSequence.cs), [`Sorting/DynamicArrayIndexedSequence.cs`](DSAExperimentation/DataStructures/Sequence/DynamicArrayIndexedSequence.cs) | Representation | Two witnesses satisfying the doubled O(1) obligation, for the same two physical reasons `ArraySequence`/`DynamicArraySequence` already do |
| [`Sorting/SortBounds.cs`](DSAExperimentation/Algorithms/Sorting/SortBounds.cs) | Representation | Groups the `[Low, High]` range being sorted — an immutable value bundle, not a mutable shared cursor |
| [`Sorting/MergeSort.cs`](DSAExperimentation/Algorithms/Sorting/MergeSort.cs) | Operations | `Sort` — top-down recursive split/merge; owns neither witness |

### 11.1 Domain separation is the reason for a new contract, not read/write alone

`IIndexedSequence<T>` exists because `Sorting` is a distinct domain from `Searching`, per §5's
domain-reuse rule (never reuse another domain's Representation or Topology contracts as your own) —
that is the primary reason, and it holds regardless of operation-set overlap. Stating "it needs
`Set` and `IRandomAccessSequence<T>` doesn't have one" as the *sole* justification would wrongly
imply that a future `Sorting` algorithm needing only `Get` could then reuse `Searching`'s
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
is `MergeSort`'s postcondition, not an assumed input.

## 12. Worked example: `Traversal/DepthFirstSearch`

> Paths below are as they stood before §13's reorg. See §13 for where each file lives today.

`Traversal/DepthFirstSearch` is this repo's first algorithm with no Representation axis at all,
and its first demonstration that a capability which looks exactly like a Graph `Topology` contract
can still resolve to a plain runtime object once it leaves Graph's domain. It is built entirely
outside `Graph/**`, generic over nothing but a bare successor function.

| File | Axis | Why |
| --- | --- | --- |
| [`Traversal/DepthFirstSearch.cs`](DSAExperimentation/Algorithms/Traversal/DepthFirst/DepthFirstSearch.cs) | Operations | `Traverse` — the only file this domain needs; there is no Representation to write |

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
through `IChildren` — not an inherent property of "successor-shaped" algorithms. The essay's own
implicit-graph illustration (a chess position, generated on demand) is naturally value-typed, and
this repo's test suite proves it directly with a `readonly record struct Position`. `Traverse` also
carries a precondition law this repo hasn't needed before: `successors` must produce a *finite*
reachable set from `start`. An infinite one — with no cycle for the visited set to catch — makes
`Traverse` run forever, with no compiler or runtime error to catch it, the same unchecked-but-real
shape as `BinarySearch`'s sortedness (§9.1) or `ShortestPath`'s non-negative-edge-weight assumption
(§7).

## 13. Physical layout: the reorg

Every worked example above (§3–§12) was written against a domain-first tree — `Graph/**`,
`Collections/Heap/**`, `Searching/**`, `Sorting/**`, `Traversal/**` — where a domain's
Representation, Topology, and Operations files all lived in one folder together, and the
three-axis classification was a *convention* the file tree didn't enforce. This section documents
four successive reorgs that made that classification physical.

**First reorg — split by axis.** Every file in both `DSAExperimentation/` and
`DSAExperimentation.Tests/` moved under exactly one of two top-level folders, `DataStructures/` or
`Algorithms/` — Representation and Topology under `DataStructures/`, Operations under
`Algorithms/`. Both sides initially kept the same domain-name subfolder underneath
(`DataStructures/Graph/Engines/Reducing/`, `Algorithms/Graph/Engines/Reducing/`, and so on).

**Second reorg — organize `Algorithms/` by utility, not by domain.** Keeping a domain-name
subfolder under `Algorithms/` was itself still domain-first, just one level down — it filed
`ShortestPath`/`ConnectedComponents`/`Reduce`/the `Fold` family under a top-level `Graph/` folder,
as if they belonged to "Graph" the data structure, when what actually distinguishes them is what
each one *does*. `DataStructures/` needed no equivalent second pass: a data structure's identity
*is* its representation/topology, so organizing it by domain name (`DataStructures/Graph/**`,
`DataStructures/Heap/**`) was already correct (§5 step 6). §13.3 below describes the tree as it
stands after both reorgs.

### 13.1 Namespace mapping rule and the tiering rule

Namespaces mirror the physical path throughout: insert `DataStructures.` or `Algorithms.`
immediately after `DSAExperimentation.`, then follow the rest of the path exactly.

Within `Algorithms/`, a second rule governs how deep a topology/strategy tier gets to nest:
**when two or more tiered implementations of the same utility exist, nest them under folders that
mirror the actual refinement relationship** (`Folding/Dags/Trees/TreeFold.cs` nests under `Dags/`
because `ITreeTopology` refines `IDagTopology`, per §3.2's chain; `ShortestPaths/Grids/
GridShortestPath.cs` nests as a second, Grid-specific tier beside the general edge-weighted one).
**When only one implementation exists today, the file stays flat** — the same "don't build
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
  is real but secondary to axis-purity here, since axis-purity is the entire point of this reorg.
- **`IVisitGuard`/`UnguardedVisit`/`TrackedVisitGuard`** superficially resemble a closed-set
  Topology witness — the same `static abstract`, two-variant shape as `IHeapOrder` — but the actual
  discriminator is *what the closed choice is about*, not its shape (§9.1, §12.1): `IHeapOrder`
  decides a relation on stored data, while `IVisitGuard` decides how the *algorithm executes*
  (whether to track revisits) — the same execution-strategy axis as `IFoldEvaluationStrategy`/
  `IReduceOrderStrategy`, uncontroversially Operations. This tier lives at `Algorithms/Walking/`,
  alongside `BreadthFirstWalk`/`DepthFirstWalk`/`TopDownWalk` — a utility folder, not a topology or
  domain one, exactly where the second reorg's own rule says it belongs.

### 13.3 Where everything lives now

**`DataStructures/`** stays organized by domain, since a data structure's identity is its
representation/topology. This includes every Operations type with no capability interface to be
generic over (§5 step 7, §13.5) — `Buffers`, `Heap`, `HashMap`, `DynamicArray`, `DisjointSet`,
`Deque`, `Stack`, `Queue`, `Set` all co-locate Representation and Operations in the same folder:

| Domain | Files |
| --- | --- |
| Buffers | `Buffers/{ContiguousGroupBufferStorage,ContiguousGroupBuffer}.cs` |
| DisjointSet | `DisjointSet/{DisjointSetForest,DisjointSet}.cs` |
| Deque | `Deque/{CircularBuffer,Deque}.cs` |
| DynamicArray | `DynamicArray/{DynamicArrayStorage,DynamicArray}.cs` |
| HashMap | `HashMap/{HashMapStorage,HashMapEntry,HashMap}.cs`, `ArrayGrowth.cs` |
| Heap | `Heap/{IHeapOrder,MinHeapOrder,MaxHeapOrder,HeapArrayIndex,HeapArray,Heap}.cs` |
| Stack | `Stack/Stack.cs` (composes `DynamicArray`) |
| Queue | `Queue/Queue.cs` (composes `Deque`) |
| Set | `Set/Set.cs` (composes `HashMap<T,bool>`) |
| Sequence | `Sequence/{IRandomAccessSequence,ArraySequence,DynamicArraySequence,IIndexedSequence,ArrayIndexedSequence,DynamicArrayIndexedSequence}.cs` (§13.6) |
| Graph — Contracts/Ordering | `Graph/Contracts/Ordering/**` |
| Graph — Topology chain | `Graph/Contracts/Topologies/**`, `Graph/Engines/Dags/IDagTopology.cs`, `Graph/Engines/Dags/Trees/ITreeTopology.cs` |
| Graph — Grids | `Graph/Grids/{Grid,GridNode,GridChildren,GridTopology}.cs` |
| Graph — ShortestPaths | `Graph/ShortestPaths/ByPriorityOrder.cs` |

**`Algorithms/`** is organized by utility instead — the second reorg's whole point, and after
§13.5's third reorg it holds *only* things generic over a capability interface with real
substitutability. `Searching` and `Sorting` have no topology axis (§4.1/§10.2) but still stay under
`Algorithms/`, since `IRandomAccessSequence`/`IIndexedSequence` each have two real implementations —
the distinguishing question is interface substitutability, not the topology axis specifically:

| Utility | Files | Topology/strategy tier |
| --- | --- | --- |
| `Ancestry/` | `LowestCommonAncestor.cs` | flat — only one tier exists today |
| `Connectivity/` | `ConnectedComponents.cs` | flat — only one tier exists today |
| `Paths/` | `AllRootToLeafPaths.cs` | flat — only one tier exists today |
| `Metrics/` | `{DiameterAlgebra,HeightAlgebra,HeightDiameterState,SizeAlgebra,TreeMetrics}.cs` | flat — Tree is the only tier that exists today |
| `Folding/` | `{CheckedFold,IFoldAlgebra,IFoldEvaluationStrategy,IterativeFoldEvaluation,RecursiveFoldEvaluation,ZipFoldAlgebra}.cs` (general tier) + `Dags/DagFold.cs` (DAG tier) + `Dags/Trees/TreeFold.cs` (tree tier) | three nested tiers, mirroring §3.2's refinement chain |
| `Reducing/` | `{BreadthFirstReduceOrder,DepthFirstReduceOrder,DistanceMapReduceAlgebra,IReduceAlgebra,IReduceOrderStrategy,Reduce,ZipReduceAlgebra}.cs` | flat — order strategy (BFS/DFS) is a separate axis from topology tier |
| `Traversal/` | `BreadthFirst/**`, `DepthFirst/{DepthFirstSearch,DepthFirstTraversal,IDepthFirstHooks}.cs`, `TopDown/**` | `DepthFirstSearch` is the weakest tier (no topology witness at all, a bare `Func`), nested beside `DepthFirstTraversal` |
| `Walking/` | `{BreadthFirstWalk,DepthFirstWalk,TopDownWalk,IVisitGuard,TrackedVisitGuard,UnguardedVisit,Unit}.cs` | flat — the guard tier (tree vs. graph) is a constructor parameter, not a file split |
| `ShortestPaths/` | `{IPathHeuristic,ShortestPath,ZeroHeuristic}.cs` (general edge-weighted tier) + `Grids/GridShortestPath.cs` (Grid tier) | two tiers, `Grids/` nested as the second |
| `Searching/` | `BinarySearch.cs`, `SearchRange.cs` | flat — no topology axis at all, generic over `Sequence.IRandomAccessSequence<T>` instead (§13.6) |
| `Sorting/` | `MergeSort.cs`, `SortBounds.cs` | flat — no topology axis at all, generic over `Sequence.IIndexedSequence<T>` instead (§13.6) |
| `TopologicalSort/` | `TopologicalSort.cs` | flat — only one tier exists today (§15) |

`DSAExperimentation.Tests/` mirrors both trees one level deeper. Fixture files distribute to the
utility folder matching the interface they implement, not a shared grab-bag — e.g. the
`Recording*Hooks` fixtures split across `Tests/Algorithms/Traversal/{BreadthFirst,DepthFirst}/Fixtures/`
by which hook interface each implements. Two Graph test-fixture groups from the first reorg still
needed a placement call rather than a mechanical rule: the Trie fixtures
(`TrieNode`/`TrieTopology`/`TrieTrees`/`WordCountFoldAlgebra`/`TrieTests`) travel together to
`Tests/DataStructures/Graph/Contracts/Ordering/`, since their star subject (`SparseArrayChildren`)
is Representation; `TestNode`/`TestTopology`/`TestTrees` anchor at
`Tests/DataStructures/Graph/Fixtures/` (`TestTopology` is itself a Topology fixture) and are
referenced cross-tree by `Algorithms/`-side test classes — harmless for test-only code. A handful
of cross-cutting tests that exercise more than one utility together (`GraphTests.cs`, `ZipTests.cs`
— Fold+Reduce+Traversal in one file; `SharedDescendantFoldTests.cs` — comparing fold tiers) sit
unfoldered at `Tests/Algorithms/` root or their utility's own root, rather than being forced into
one utility's subfolder.

### 13.4 `suppressions.json`

Deliberately deleted before this reorg began and left deleted throughout it — no suppression
ledger was maintained or restored per migration step. Every judgment call `suppressions.json` used
to waive (the `check-class-size` "two clusters" pattern, several test-file literal findings, a
`check-test-coverage` attribution gap) resurfaces as a raw Nomos gate finding rather than a
suppressed one until that ledger is rebuilt — expected, not a regression from this reorg.

### 13.5 Third reorg — Operations rejoins Representation when there's no capability interface

The second reorg's utility-first rule still left nine Operations types under `Algorithms/`, each
named after the single data structure it belongs to: `Buffers`, `Heap`, `HashMap`, `DynamicArray`,
`DisjointSet`, `Deque`, and — one level removed, each hardcoding one of the previous six rather than
a dedicated Storage class — `Stack`, `Queue`, `Set`. §2's literal definition of Operations
("the algorithm/API surface built on top of a Representation constrained by a Topology") covers
them, but that's not the discriminator that actually matters: `ShortestPath`/`MergeSort`/
`BinarySearch`/`Reduce`/`Fold` are generic over an *interface* with genuine multiple implementations
(`IEdgeTopology`, `IRandomAccessSequence`, `IIndexedSequence`,
`ITreeTopology`/`IDagTopology`/`IGraphTopology`) — decoupled from any one representation, and free
to run over a different one tomorrow. These nine are hardwired 1:1 to exactly one concrete type,
with no interface and no second implementation possible — `Heap.cs` only ever composes `HeapArray`,
`Stack.cs` only ever composes `DynamicArray`. They aren't algorithms decoupled from a data
structure; they *are* the data structure, with the bounds-checking layer split out (§5 step 7).

So they moved back into `DataStructures/`, alongside their existing Storage (or, for
`Stack`/`Queue`/`Set`, into a new sibling folder next to the type each one composes) —
`DataStructures/Heap/Heap.cs` beside `HeapArray.cs`, `DataStructures/Stack/Stack.cs` beside the
`DynamicArray.cs` it hardcodes. `DisjointSet` also dropped its `Collections/` wrapper in the same
step, for the same consistency reason the second reorg dropped it from the `Algorithms/` side.
`Algorithms/` now holds exactly the set of things generic over a capability interface with real
substitutability — nothing more, nothing less. `Searching`/`Sorting` are the one case that looks
similar but isn't: they have no *topology* axis either, but `IRandomAccessSequence`/
`IIndexedSequence` each have two real implementations, so `BinarySearch`/`MergeSort` stay in
`Algorithms/`, decoupled from either one.

### 13.6 Fourth reorg — `Searching`/`Sorting` rename to `Sequence`, and a misclassification fix

`DataStructures/Searching/` and `DataStructures/Sorting/` were named after the algorithms that
consume them, not what they are — the same mistake the second reorg fixed on the `Algorithms/`
side, made in the opposite direction: a `DataStructures/` folder is supposed to be named for
identity (§5 step 6), and `IRandomAccessSequence`/`ArraySequence`/`DynamicArraySequence` and
`IIndexedSequence`/`ArrayIndexedSequence`/`DynamicArrayIndexedSequence` are identically "sequence
access contracts," just at two different read/write capability levels — not "a searching thing" or
"a sorting thing." Both sets now co-locate under one identity folder, `DataStructures/Sequence/`,
the same way `MinHeapOrder`/`MaxHeapOrder` already sit as sibling witnesses under `Heap/` — still
two separate, non-reused domains per §5's domain-reuse rule (`Sorting` never reuses `Searching`'s contract, per §11.1),
just grouped by theme rather than split across two algorithm-named folders.

Checking this surfaced an unrelated, pre-existing misclassification: `SearchRange.cs` and
`SortBounds.cs` had been sitting in these same folders, but neither is a Representation of the
sequence being searched/sorted — both are the algorithm's own internal cursor/range-grouping state
(their own doc comments say so directly), the same bucket §12.2 already puts `TrackedVisitGuard` in.
They moved to `Algorithms/Searching/` and `Algorithms/Sorting/`, alongside `BinarySearch.cs`/
`MergeSort.cs`, correcting a classification gap that predates every reorg in this section.

## 14. Worked example: the three Storage/Operations decompositions

Three bundled classes — `ContiguousGroupBuffer`, `HashMap`, `DynamicArray` — used to fuse storage
fields and public operations into one type, unlike `Heap`/`HeapArray` (§4), which was already
split. This is the part of §13's reorg that is genuine decoupling rather than file-shuffling, and
the one part with real behavior-preservation risk: a mechanical move can't change behavior, but a
decomposition can. All three follow the same shape: **Storage owns raw, unchecked, non-throwing
access; Operations owns bounds/precondition checks, throwing, and the one composed Storage field.**
Each was attempted only after the pattern had been rehearsed on a cheaper case first — `Buffers`
(zero consumers anywhere) first, `HashMap` second, `DynamicArray` (the highest fan-out) last — and
each was checked against the full test suite, not just its own domain's tests, before being
trusted. (Both halves of all three now live under `DataStructures/` per §13.5 — none of these three
had a capability interface to be generic over, so their Operations half rejoined Storage there.)

### 14.1 `ContiguousGroupBuffer`

| File | Axis | Why |
| --- | --- | --- |
| `DataStructures/Buffers/ContiguousGroupBufferStorage.cs` | Representation | `_currentGroup`/`_currentKey`/`_hasCurrentKey` fields; unchecked `HasCurrentKey`/`CurrentKey`/`CurrentGroupCount`/`AppendToCurrentGroup`/`SnapshotCurrentGroup`/`SetCurrentKey`/`MarkCurrentKeyConsumed`/`ResetAll` |
| `DataStructures/Buffers/ContiguousGroupBuffer.cs` | Operations | `keyComparer`, `GetCurrentKey`/`ThrowMissingCurrentKey`, composes Storage |

The cheapest possible rehearsal of the pattern — zero existing consumers anywhere in the repo at
the time. `MarkCurrentKeyConsumed` deliberately clears only `_hasCurrentKey`, leaving the stale
`_currentKey` value in place (only `Reset` fully clears it) — preserving `Flush`'s exact
pre-decomposition behavior, which nothing tested before this reorg added a test for it specifically.

### 14.2 `HashMap`

| File | Axis | Why |
| --- | --- | --- |
| `DataStructures/HashMap/HashMapStorage.cs` | Representation | bucket array + separate-chaining entries array + free list; owns `Grow`/`RehashEntryInto`/`CreateEmptyBuckets`/`AllocateEntrySlot` (none touch the comparer) and `Insert(hashCode,key,value)` |
| `DataStructures/HashMap/HashMapEntry.cs` | Representation | unchanged, moved alongside Storage |
| `DataStructures/HashMap/HashMap.cs` | Operations | `_comparer`; `TryUpdateExisting`/`FindEntryIndex`/`Remove`'s chain-walk loops, reading each step via raw Storage accessors (`BucketHead`/`EntryNext`/`EntryHashCode`/`EntryKey`/`EntryValue`) |

The split point here isn't read/write the way `DynamicArray`'s is (§14.3) — it's "needs the
comparer" vs. "doesn't." `Storage.Insert` absorbs the whole grow-if-needed → recompute-bucket →
allocate-slot → write → link pipeline, since "should this insert grow first" isn't a
comparer-dependent decision, and it always recomputes its own fresh bucket index internally after
any grow rather than accepting one as a parameter — structurally ruling out a stale post-grow
bucket index, the same kind of silent-misrouting risk §8's "performance independence" section warns
about, except here it's a correctness risk, not a Big-O one. A test forcing a `Grow()` mid-sequence
and then a `TryGetValue` on a key inserted immediately before the resize exists specifically to
catch a regression here.

### 14.3 `DynamicArray`

| File | Axis | Why |
| --- | --- | --- |
| `DataStructures/DynamicArray/DynamicArrayStorage.cs` | Representation | `_items: T[]`; own `Count`; unchecked `Get`/`Set`/`Add`/`InsertAt`/`RemoveAt` (assume the caller already validated the index); private `EnsureCapacity` |
| `DataStructures/DynamicArray/DynamicArray.cs` | Operations | `ValidateIndex`, the `IndexOutOfRangeMessage` constant; bounds-checked `Get`/`Set`/`RemoveAt`/`Insert` delegate to Storage after validation — identical exception type and message text to before the split |

The highest fan-out of the three: `Stack`, `Sequence.DynamicArraySequence`, and
`Sequence.DynamicArrayIndexedSequence` all compose it, but none call anything beyond
`Add`/`Get`/`Set`/`RemoveAt`/`Count` — all of which keep identical signatures — so none needed a
logic change, only a `using`-line repoint. Decomposed last of the three, once the Storage/Operations
pattern had already been proven twice.

## 15. Worked example: `Algorithms/TopologicalSort`

`Algorithms/TopologicalSort` fills a gap `Folding/CheckedFold.cs` names explicitly in its own
doc comment: "an iterative, stack-safe DAG fold needs a real topological sort... that's a genuine
follow-up, not something to fold in here." It sits at the same untrusted tier `CheckedFold`
already occupies — generic over `IGraphTopology<TNode,TChildren>`, not `IDagTopology`, because
Kahn's algorithm's own leftover-in-degree check *is* the cycle check, the same law `CheckedFold`
already resolved this way. But that tier choice alone doesn't earn this section a place beside
§9–§12/§14 — `Connectivity/ConnectedComponents.cs` already sits at the same bare `IGraphTopology`
tier with nothing more than a §13.3 table row. What actually earns it is new material: a
precondition shape this repo hasn't needed before, and a second occurrence of §10.3's namespace
collision.

| File | Axis | Why |
| --- | --- | --- |
| [`TopologicalSort/TopologicalSort.cs`](DSAExperimentation/Algorithms/TopologicalSort/TopologicalSort.cs) | Operations | `TrySort` — Kahn's algorithm; composes `IGraphTopology`/`IChildren`/`IChildOrder` from `Graph/Contracts/**`, owns none of them, needs no new Representation or Topology contract |

### 15.1 A precondition shape this repo hasn't needed before, and its asymmetric failure

Every prior unenforced precondition in this repo constrains a *property* of the input —
`BinarySearch`'s sortedness (§9.1), `ShortestPath`'s non-negative edge weights (§7). `TrySort`'s
precondition is different in kind: `nodes` must enumerate the *complete vertex set* of the graph,
not merely its sources. A root-seeded walk (`Reduce.Graph`, `ConnectedComponents.Count`) only
needs one representative per component, because it discovers descendants transitively through
`GetChildren`. Kahn's algorithm can't work that way — it needs every vertex's true in-degree
before the first node is ever dequeued, and there is no reverse-adjacency contract anywhere in
`Graph/Contracts/**` (no way to ask a node for its parents), so nothing here could discover a
missing vertex even if it wanted to.

The two ways to violate this precondition fail differently, which is the genuinely new part.
Omitting a *descendant* self-detects: it's still discovered via its parent's edge during the
walk, which inflates the produced ordering past `vertices.Count`, so `TrySort` correctly returns
`false`. Omitting an *ancestor* does not: nothing forward-reachable from the supplied set points
back to it, so `TrySort` returns `true` with an ordering that silently omits it — a caller cannot
tell this happened from the return value alone. A consequence worth stating plainly: a `false`
result therefore doesn't distinguish a true cycle from a caller-omitted descendant either — both
present identically as leftover in-degree.

### 15.2 Free cycle detection, unlike `CheckedFold`'s recursion-path set

`CheckedFold.TryFold` needs an explicit `HashSet<TNode> inProgress` tracking the current
recursion path, because a cycle only reveals itself as "a node still on the path being reached
again" mid-descent. Kahn's algorithm needs no equivalent structure: a cycle *is* the set of nodes
whose in-degree never reaches zero, which the algorithm was already computing for an unrelated
reason (deciding what to dequeue next). This is a case where the untrusted tier's defense comes
free of charge rather than as an added cost over the trusted tier — there is no `DagFold`-style
faster sibling for this utility to skip down to, because there's nothing left to skip.

### 15.3 A second `Algorithms/`-side folder/class name collision

`Algorithms/TopologicalSort/TopologicalSort.cs` names its folder and its class identically — the
same shape §10.3 already documents for `DataStructures/DisjointSet/DisjointSet.cs`, and the same
C# wrinkle results: `TopologicalSortTests.cs` living at namespace
`DSAExperimentation.Tests.Algorithms.TopologicalSort` can't reference a bare `TopologicalSort`
without it resolving to the enclosing namespace segment instead of the type, since namespace-
member lookup wins over `using` directives. Fixed the same way §10.3 fixed it — a differently-
named alias, `using TopologicalSortOperations = DSAExperimentation.Algorithms.TopologicalSort.TopologicalSort;`

This is worth calling out as a new occurrence, not a mechanical reuse: every `Algorithms/` utility
folder before this one deliberately used a *different* word than its primary class
(`Connectivity`/`ConnectedComponents`, `Ancestry`/`LowestCommonAncestor`, `Paths`/
`AllRootToLeafPaths`, `Searching`/`BinarySearch`, `Sorting`/`MergeSort`), because §5 step 6 names
`Algorithms/` folders for *what the algorithm does*, which up to now always read as a broader
category than any one technique's own name. `TopologicalSort` is the first case on the
`Algorithms/` side where the category and the sole technique's name coincide exactly — renaming
the class to something eponymous (`KahnsAlgorithm`) would dodge the collision but breaks this
repo's preference for descriptive over eponymous names for a sole/primary technique (`MergeSort`,
`BinarySearch`); `ShortestPath.Dijkstra`/`.AStar` are the one eponymous precedent here, but as
*methods* distinguishing multiple techniques inside one shared category class, which doesn't
apply when only one technique exists. §10.3's alias fix generalizes cleanly to this case instead.
