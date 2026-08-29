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
// either.
//
// The cache is a plain BCL Dictionary<TState,TResult>, not this repo's own HashMap -
// per §16.5's own test ("lifetime and ownership, not 'is this a TKey-to-int map'"),
// the cache here is built at the top of one Memoize call and discarded when it
// returns, never constructed by or exposed to the caller across multiple calls, the
// same Algorithms/-tier scratch-state bucket ShortestPath.Distances and
// TopologicalSort.inDegree already occupy - not KeyedDisjointSet's DataStructures/-
// tier reusable wrapper.
//
// API shape: recurrence takes a second parameter, itself a Func<TState,TResult> -
// the memoized recursive call-back, Y-combinator style - so callers write natural-
// looking recursion (`(n, fib) => n <= 1 ? n : fib(n-1) + fib(n-2)`) instead of
// manually routing every recursive branch through a cache object by hand (a silent,
// easy-to-forget runtime footgun the alternative shape invites). One real ergonomic
// cost: TResult appears only inside recurrence's own parameter type, so neither an
// inline lambda nor a named local function's method group gives the compiler enough
// to infer it from - TState infers fine from start, but nothing pins TResult down
// until the method group itself is bound, which needs TResult already known, a
// circularity C#'s inference does not attempt to break (CS0411 either way). Every
// call site below states both type arguments explicitly
// (`Memoizer.Memoize<TState, TResult>(start, Recurrence)`) - one annotation, and the
// failure mode is a loud compile error, not (as the alternative shape below risks)
// silent exponential blowup from a forgotten cache route-through.
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
        Func<TState, Func<TState, TResult>, TResult> recurrence)
        where TState : notnull
        => Memoize(start, recurrence, EqualityComparer<TState>.Default);

    public static TResult Memoize<TState, TResult>(
        TState start,
        Func<TState, Func<TState, TResult>, TResult> recurrence,
        IEqualityComparer<TState> comparer)
        where TState : notnull
    {
        // binding-scope: allow -- cache must be declared here, one block out from
        // its only direct reads/writes inside ResultFor, not moved into ResultFor's own
        // body: it has to be the SAME dictionary shared across every recursive call,
        // which is the entire memoization mechanism. Declaring it inside ResultFor
        // would silently replace it with a fresh, empty dictionary on every call,
        // defeating memoization without any compiler or test failure to catch it.
        var cache = new Dictionary<TState, TResult>(comparer);

        TResult ResultFor(TState state)
        {
            if (cache.TryGetValue(state, out var cached))
            {
                return cached;
            }

            var result = recurrence(state, ResultFor);
            cache[state] = result;
            return result;
        }

        return ResultFor(start);
    }
}
