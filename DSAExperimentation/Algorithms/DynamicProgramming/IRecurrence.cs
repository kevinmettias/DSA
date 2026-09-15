namespace DSAExperimentation.Algorithms.DynamicProgramming;

// A memoized recurrence, named rather than passed: the decision this type exists to
// name is the one a bare callable parameter leaves anonymous - what the recursive
// call-back IS. A caller implements this once for a recurrence, and Memoizer hands
// that implementation back to itself for the branches that recurse, so a recursive
// call is a method on a named type at every level rather than an opaque delegate
// value.
//
// Replay takes another IRecurrence, not a callable for one: a strategy type whose own
// method then took a bare callable would keep the anonymous recursion one level down,
// which relocates the missing contract rather than stating it. `rest` is therefore the
// whole of the recursion a recurrence is given - the memo run in production, and
// whatever a caller or a test chooses to wrap around it.
internal interface IRecurrence<TState, TResult>
{
    TResult Replay(TState state, IRecurrence<TState, TResult> rest);
}
