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

### 3.1 Representation

- [`Graph/Contracts/Ordering/IChildren.cs`](DSAExperimentation/Graph/Contracts/Ordering/IChildren.cs) —
  an indexable view over a node's children. Deliberately *not* `IReadOnlyList<TNode>`: `Get` is a
  named method, not an indexer, specifically so implementations stay thin structs that JIT-specialize
  to direct, non-virtual calls with no boxing.
- [`ListChildren.cs`](DSAExperimentation/Graph/Contracts/Ordering/ListChildren.cs) — pointer/`List`-backed.
- [`SparseArrayChildren.cs`](DSAExperimentation/Graph/Contracts/Ordering/SparseArrayChildren.cs) —
  fixed slot-array backed (e.g. a trie's 26-letter alphabet), scans past nulls instead of storing a
  materialized list.
- [`Graph/Algorithms/Grids/GridChildren.cs`](DSAExperimentation/Graph/Algorithms/Grids/GridChildren.cs) —
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

- [`IGraphTopology.cs`](DSAExperimentation/Graph/Contracts/Topologies/IGraphTopology.cs) — the
  general contract every walker/fold/reduce actually needs: given a node, what's adjacent to it.
  Algorithms constrained on this bare interface can't assume acyclicity and must defend against
  cycles/sharing themselves.
- [`IDagTopology.cs`](DSAExperimentation/Graph/Engines/Dags/IDagTopology.cs) — promises acyclicity.
  This is exactly what a fold needs and exactly what bare `IGraphTopology` doesn't promise, which is
  why `CheckedFold` has to accept any `IGraphTopology` and defend itself at runtime while `DagFold`,
  constrained on this tier, skips that defense entirely.
- [`ITreeTopology.cs`](DSAExperimentation/Graph/Engines/Dags/Trees/ITreeTopology.cs) — additionally
  promises unique ancestry, which is what lets the tree-only walkers skip visited-tracking/memoization
  and stay zero-cost.

The payoff is concrete, not just documentation: [`UnguardedVisit.cs`](DSAExperimentation/Graph/Engines/Walking/UnguardedVisit.cs)
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

[`IPathHeuristic.cs`](DSAExperimentation/Graph/Algorithms/ShortestPaths/IPathHeuristic.cs)/`ZeroHeuristic.cs`
is the template for "one algorithm, one injected axis, not two parallel implementations" —
`ShortestPath.Dijkstra` is `ShortestPath.AStar` closed over `ZeroHeuristic`. `Collections/Heap`'s
`IHeapOrder` reuses this exact idiom in §4.

## 4. Worked example: `Collections/Heap`

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
[`ByPriorityOrder.cs`](DSAExperimentation/Graph/Algorithms/ShortestPaths/ByPriorityOrder.cs) projects
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
  [`HeapArray<T>`](DSAExperimentation/Collections/Heap/HeapArray.cs)'s `Get`/`Set`/`Swap` are O(1).
  A syntactic contract like `Get(index)` says nothing about that cost. Swap `HeapArray<T>`'s backing
  store for something with O(n) indexed access and `Heap`'s complexity claim silently degrades to
  O(n log n) — no compiler error, no test failure, since correctness tests still pass regardless of
  how long they take.

**The actionable rule** (extends §5's recipe): when an Operations-layer type's complexity guarantee
depends on a specific cost from its Representation layer, say so in that type's doc comment. The
type system can't enforce Big-O, but a written dependency is what stops a well-intentioned
representation swap from silently regressing performance instead of failing loudly.
[`Heap.cs`](DSAExperimentation/Collections/Heap/Heap.cs) and
[`HeapArray.cs`](DSAExperimentation/Collections/Heap/HeapArray.cs) carry exactly this cross-reference
as the worked example.

## 9. Worked example: `Searching/BinarySearch`

`Searching/BinarySearch` is this repo's first non-Graph, non-Collections domain, and its first
algorithm bound to a Representation contract it defines for itself rather than one it's handed.
Where `Collections/Heap` needed one new Topology witness (`IHeapOrder`) over an already-obvious
Representation (a flat array), binary search needs the opposite: no Topology witness at all
(sortedness isn't a variant to choose between — see §9.1), but a brand-new Representation contract,
because "the sequence being searched" is the one part of this problem genuinely open to more than
one physical layout.

| File | Axis | Why |
| --- | --- | --- |
| [`Searching/IRandomAccessSequence.cs`](DSAExperimentation/Searching/IRandomAccessSequence.cs) | Representation | An indexable view over an already-sorted sequence, generic over the element type alone — `Find`'s only physical-layout requirement |
| [`ArraySequence.cs`](DSAExperimentation/Searching/ArraySequence.cs), [`DynamicArraySequence.cs`](DSAExperimentation/Searching/DynamicArraySequence.cs) | Representation | Two witnesses satisfying the O(1)-`Get` obligation above, for two different physical reasons |
| [`BinarySearch.cs`](DSAExperimentation/Searching/BinarySearch.cs) | Operations | `Find` — the bisection loop; owns neither Representation witness |

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

`Collections/DisjointSet` is this repo's first equivalence-class structure, and its first
demonstration that a choice which *looks* exactly like Heap's Min-vs-Max axis can still fail to
earn a Topology witness. Where `Collections/Heap` needed a Topology witness over an obvious
Representation, and `Searching/BinarySearch` needed a new Representation over an already-settled
Topology, Union-Find needs neither: the representation is a concrete two-array forest, and the
one axis that looks variable turns out to be a Complexity law instead.

| File | Axis | Why |
| --- | --- | --- |
| [`Collections/DisjointSet/DisjointSetForest.cs`](DSAExperimentation/Collections/DisjointSet/DisjointSetForest.cs) | Representation | Parallel `parent`/`rank` `int[]` fields, fixed-size at construction — concrete, not an interface, since exactly one physical layout exists today (§5 step 3) |
| [`Collections/DisjointSet/DisjointSet.cs`](DSAExperimentation/Collections/DisjointSet/DisjointSet.cs) | Operations | `Find`/`Union`/`Connected` — path compression and union-by-rank, both hardcoded, not swappable |

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

`Sorting/MergeSort` is this repo's second checklist-first algorithm after `Searching/BinarySearch`,
and the first built specifically to show that "the same capability shape" is not grounds to reuse
another domain's Representation contract. Both algorithms need indexed access to a sequence; only
one of them needs to write it back.

| File | Axis | Why |
| --- | --- | --- |
| [`Sorting/IIndexedSequence.cs`](DSAExperimentation/Sorting/IIndexedSequence.cs) | Representation | Indexed get/set view, generic over the element type alone — earns the interface on day one via two witnesses, same §5-step-3 exception `IRandomAccessSequence<T>` used |
| [`Sorting/ArrayIndexedSequence.cs`](DSAExperimentation/Sorting/ArrayIndexedSequence.cs), [`Sorting/DynamicArrayIndexedSequence.cs`](DSAExperimentation/Sorting/DynamicArrayIndexedSequence.cs) | Representation | Two witnesses satisfying the doubled O(1) obligation, for the same two physical reasons `ArraySequence`/`DynamicArraySequence` already do |
| [`Sorting/SortBounds.cs`](DSAExperimentation/Sorting/SortBounds.cs) | Representation | Groups the `[Low, High]` range being sorted — an immutable value bundle, not a mutable shared cursor |
| [`Sorting/MergeSort.cs`](DSAExperimentation/Sorting/MergeSort.cs) | Operations | `Sort` — top-down recursive split/merge; owns neither witness |

### 11.1 Domain separation is the reason for a new contract, not read/write alone

`IIndexedSequence<T>` exists because `Sorting` is a distinct domain from `Searching`, per §5.5 —
that is the primary reason, and it holds regardless of operation-set overlap. Stating "it needs
`Set` and `IRandomAccessSequence<T>` doesn't have one" as the *sole* justification would wrongly
imply that a future `Sorting` algorithm needing only `Get` could then reuse `Searching`'s
interface; §5.5's rule fires on domain ownership, not on whether the method lists happen to
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

`Traversal/DepthFirstSearch` is this repo's first algorithm with no Representation axis at all,
and its first demonstration that a capability which looks exactly like a Graph `Topology` contract
can still resolve to a plain runtime object once it leaves Graph's domain. It is built entirely
outside `Graph/**`, generic over nothing but a bare successor function.

| File | Axis | Why |
| --- | --- | --- |
| [`Traversal/DepthFirstSearch.cs`](DSAExperimentation/Traversal/DepthFirstSearch.cs) | Operations | `Traverse` — the only file this domain needs; there is no Representation to write |

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
codebase is Graph's own, and reusing it is exactly what §5.5 forbids.

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
