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
| `Collections/DynamicArray/DynamicArray.cs` | itself (a manually-doubled `T[]`) | `Add`/`Insert`/`RemoveAt`/`Get`/`Set` | No Topology axis at all — the only invariant is "how it's stored," which §5 step 1 says belongs to Representation, not Topology |
| `Collections/Stack/Stack.cs` | `DynamicArray<T>` (composed, not duplicated) | `Push`/`Pop`/`Peek` | A stack is a sequence plus a LIFO *access constraint* — an Operations-level restriction over an existing Representation, not a new physical layout |
| `Collections/Deque/CircularBuffer.cs` + `Deque.cs` | a wraparound array (`_head`/`Count` modulo length) | `Push`/`Pop`/`Peek` at both ends | A second, distinct Representation (wraparound, not flat) — earned by needing O(1) at *both* ends, which `DynamicArray`'s flat layout can't give the front |
| `Collections/Queue/Queue.cs` | `Deque<T>` (composed) | `Enqueue`/`Dequeue`/`Peek` | FIFO is Deque restricted to one end each — the same "sequence + constraint" relationship Stack has with `DynamicArray`, just against the wraparound Representation |
| `Collections/HashMap/HashMap.cs` (+ `HashMapEntry.cs`) | bucket array + separate-chaining entries array with a free list | `Get`/`Set`/`Remove`/`ContainsKey` | The associative primitive from §2 in code: no Topology witness, because "at most one value per key" is what a map *is*, not a variable choice the way min-vs-max-heap is |
| `Collections/Set/Set.cs` | `HashMap<T,bool>` (composed) | `Add`/`Contains`/`Remove` | Backed by `HashMap` the way `java.util.HashSet` is backed by `HashMap<E,Object>` — membership is "is this key present," so the value is an unused placeholder |

`Stack`/`Queue` composing `DynamicArray`/`Deque` rather than each managing their own array is itself
the philosophical point from earlier in this conversation made concrete: *"a stack isn't a unique
physical structure — it's a sequence plus a LIFO access constraint."* Representation is shared
because the constraint, not the storage, is what varies.

## 5. Recipe for adding a new `Collections/*` structure

1. **Does it have a real invariant worth a Topology witness?** (ordering, uniqueness, balance).
   If yes, model it as a closed set of `struct`-constrained static-abstract witnesses, generic
   over the element/key type only — not over storage. If the only "invariant" is "how it's stored,"
   that's a Representation concern, not a Topology one.
2. **Is there currently exactly one physical layout?** If yes, make Representation a concrete
   class, not an interface. Don't introduce the interface until a second implementation exists to
   justify it — the same way `SparseArrayChildren` earned `IChildren`'s existence by being a second
   consumer, not the first. An interface with one implementation is speculative cost (virtual
   dispatch, boxing) with no present benefit.
3. **Operations** is the mutable instance type (or a static engine, for algorithms that walk an
   externally-owned structure) that composes Representation and Topology via generic constraints.
4. **Never reuse another domain's Representation or Topology contracts.** Replicate the *pattern*,
   not the *type* — a fold's node-list and a heap's backing array are different enough shapes that
   forcing a shared interface between them tends to produce a leaky common surface instead of a
   useful one.

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
only transitively.

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
  ("admissible — never overestimates") is this repo's existing example of a stated semantic *law* —
  `L` already has precedent here, even though its complexity half doesn't yet.
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
