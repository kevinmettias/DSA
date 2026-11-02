namespace DSAExperimentation.Algorithms.DynamicProgramming;

// Memoizer's engine for a recurrence whose states are the ints 0..stateCount-1 - a bitmask
// over n items, an index, a remaining budget - with the cache as two flat arrays indexed by
// the state instead of a Dictionary. The recurrence is the same IRecurrence<int, TResult>, so
// one written for Memoizer runs here unchanged, and the run hands itself down as the recursion
// exactly as MemoRun does.
//
// A type of its own rather than an option on Memoizer, because the two make opposite bets.
// Memoizer pays per state it REACHES: a hash, an entry, and a resize now and then, which costs
// nothing for the states it never reaches. This pays for every state in the range up front, a
// result slot and a byte, and then nothing per lookup but an index. So it saves memory when the
// states reached fill much of their range, and wastes it when they are a sparse few in a huge one.
// It saves time only while the range is small. Measured with BenchmarkDotNet on LC 3530, two
// short-job passes:
// - 6 to 15 nodes: 1.1 to 2.6 times faster than Memoizer, allocating a fraction as much.
// - 22 nodes, edgeless DAG, all 2^22 placed sets reached: 38 MB against the Dictionary's 324 MB,
//   in about the same time (716-735 ms against 778-818 ms).
// - 22 nodes, sparse DAG: the same memory as the Dictionary, and about 15% slower (50-52 ms
//   against 43-45 ms).
// At that size the time goes to the recurrence's own branching and to cache misses on 32 MB of
// results indexed by mask. The Dictionary's entries sit in the order they were computed, close to
// the states that read them next.
//
// The caller states the range, and that is the whole of the extra contract. A state outside it
// makes an array access throw IndexOutOfRangeException. Memoizer's own precondition carries over
// unchanged: the states must be well-founded, since a state is stored only after its own call
// returns.
/// <summary>
/// Memoizes a recurrence over the int states 0 to stateCount - 1, keeping the cache in arrays indexed by state.
/// </summary>
public static class DenseMemoizer
{
    /// <summary>Runs a recurrence from a start state, caching each state's result in an array slot.</summary>
    /// <param name="start">The state whose result is wanted.</param>
    /// <param name="stateCount">One more than the largest state the recurrence can reach.</param>
    /// <param name="recurrence">The recurrence to run; a cycle among its states overflows the stack.</param>
    /// <returns>The result <paramref name="recurrence"/> gives for <paramref name="start"/>.</returns>
    public static TResult Memoize<TResult>(int start, int stateCount, IRecurrence<int, TResult> recurrence)
    {
        var run = new DenseMemoRun<TResult>(new TResult[stateCount], new bool[stateCount], recurrence);

        return run.Replay(start, run);
    }

    // The recursion a recurrence is handed, as in MemoRun: it owns the cache and passes `this`
    // on, so `rest` goes unread and a caller's wrapper cannot route a branch past the memo.
    // `known` is kept apart from `results` because a TResult has no value that could mean
    // "not computed yet".
    private sealed class DenseMemoRun<TResult>(TResult[] results, bool[] known, IRecurrence<int, TResult> recurrence)
        : IRecurrence<int, TResult>
    {
        public TResult Replay(int state, IRecurrence<int, TResult> rest)
        {
            if (known[state])
            {
                return results[state];
            }

            var computed = recurrence.Replay(state, this);
            results[state] = computed;
            known[state] = true;

            return computed;
        }
    }
}
