namespace DSAExperimentation.Algorithms.DynamicProgramming;

// Not an IFoldAlgebra/IReduceAlgebra: LowestCommonAncestor's own doc comment already
// explains why a static-abstract algebra can't close over a runtime value, and a
// memoized recurrence needs that channel twice over - once for the recurrence body
// itself (arbitrary caller logic: a Fibonacci-shaped, knapsack-shaped, or edit-
// distance-shaped recurrence share no closed catalogue this library could
// enumerate), and again for the cache its own recursive calls thread through
// (mutable runtime state a static-abstract member can never own). So this is written
// directly, the same "bespoke, take the varying part as an ordinary parameter" move
// LCA/Dijkstra already make.
//
// DepthFirstSearch.cs is the closer precedent, not Fold/Reduce: it's generic over a
// bare Func with no Representation axis at all, because the space of valid
// successor relations is open-ended, not a set this library enumerates itself
// (§9.2/§12.1's discriminator). A DP recurrence is the same shape - "Operations plus
// an open-runtime-object law, with no Representation axis at all," the mirror image
// of DynamicArray (§12.2) - so Memoize needs no new Representation/Topology file
// either: what it needs is a name for that open bucket, which IRecurrence supplies.
//
// The cache is a plain BCL Dictionary<TState,TResult>, not this repo's own HashMap -
// per §16.5's own test ("lifetime and ownership, not 'is this a TKey-to-int map'"),
// the cache here is built at the top of one Memoize call and discarded when it
// returns, never constructed by or exposed to the caller across multiple calls, the
// same Algorithms/-tier scratch-state bucket ShortestPath.Distances and
// TopologicalSort.inDegree already occupy - not KeyedDisjointSet's DataStructures/-
// tier reusable wrapper.
//
// API shape: the recurrence is a named type, IRecurrence<TState,TResult>, whose single
// method is the whole of what a recurrence owes this engine - Replay(state, rest)
// computes state's result and branches through `rest` for the sub-states it needs.
// That names the recursion at the point of use (`rest.Replay(next, rest)`), and it
// names the decision itself: a caller's recurrence is a type named for what it says,
// typically a private nested class with the branches it recurses through as its body.
// The memo run below is the recursion a recurrence is handed back - an IRecurrence
// whose Replay reads and writes the cache, and which passes `this` rather than holding
// a recursion in a field, so the recursion is the same object at every level and there
// is no second place it could come from. A caller therefore never routes a branch
// through the cache by hand: the only recursion a recurrence is ever given IS the memo
// run, so forgetting to go through the memo is not a failure mode this shape has.
// Inference is complete on both parameters - TState from start, TResult from the
// recurrence instance's own instantiation - so a call site reads
// `Memoize(start, new Ways())` with no annotation at all.
//
// TState : notnull, not : class - mirrors DepthFirstSearch's own TNode : notnull
// reasoning (§12.3): DP states are naturally value types (an int, or a tuple like
// (int Row, int Col) for 2D DP), which work with zero extra ceremony since
// ValueTuple's structural equality/hashing make EqualityComparer<TState>.Default
// correct out of the box. The comparer stays a plain runtime IEqualityComparer
// <TState> parameter - same §9.2 open-bucket reasoning HashMap's own comparer
// already establishes - with a default-comparer overload.
//
// Unchecked precondition, the same shape as BinarySearch's sortedness (§9.1) or
// DepthFirstSearch's finite-reachable-set law (§12.3): the state graph induced by
// recurrence must be well-founded (no cycle through un-cached states) - a cycle
// stack-overflows immediately, since a state only ever populates the cache AFTER
// its own call returns, not hangs the way an unguarded graph walk might. Recursion
// depth is bounded by the state graph's longest dependency chain, not by how many
// distinct states exist - an inherent trade-off of embracing the real call stack
// instead of hand-rolling a heap-allocated one, not a defect to work around here.
internal static class Memoizer
{
    public static TResult Memoize<TState, TResult>(
        TState start,
        IRecurrence<TState, TResult> recurrence)
        where TState : notnull
        => Memoize(start, recurrence, EqualityComparer<TState>.Default);

    public static TResult Memoize<TState, TResult>(
        TState start,
        IRecurrence<TState, TResult> recurrence,
        IEqualityComparer<TState> comparer)
        where TState : notnull
    {
        var cache = new Dictionary<TState, TResult>(comparer);
        var run = new MemoRun<TState, TResult>(cache, recurrence);

        return run.Replay(start, run);
    }

    // The recursion a recurrence is handed: it owns the cache, and its Replay passes
    // `this` on to the next level, so the recursion is this object all the way down.
    // That is also why `rest` goes unread here - a run is already the recursion, and
    // taking it from the parameter instead would let a caller's wrapper redirect a
    // branch past the memo.
    private sealed class MemoRun<TState, TResult>(
        Dictionary<TState, TResult> cache,
        IRecurrence<TState, TResult> recurrence) : IRecurrence<TState, TResult>
        where TState : notnull
    {
        public TResult Replay(TState state, IRecurrence<TState, TResult> rest)
        {
            if (cache.TryGetValue(state, out var cached))
            {
                return cached;
            }

            var computed = recurrence.Replay(state, this);
            cache[state] = computed;

            return computed;
        }
    }
}
