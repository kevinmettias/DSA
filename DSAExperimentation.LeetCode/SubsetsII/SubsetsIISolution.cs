using DSAExperimentation.Algorithms.Backtracking;

namespace DSAExperimentation.LeetCode.SubsetsII;

// LeetCode 90. Subsets II: given an integer array that may contain duplicates,
// return every possible subset (the power set), with no duplicate subset in the
// result.
//
// The [Benchmark(Baseline = true)] "IterativeDedup" arm this migrated out of never
// implemented an independent algorithm - its body built an unused HashSet<string>,
// looped over a discarded HashSet<int> doing nothing, and then returned this same
// backtracking arm's own value directly. It was dead code wrapping the one real
// strategy, not a second strategy, so only that one strategy survives here.
internal static class SubsetsIISolution
{
    // This repo's own backtracking combinator: sort first so duplicates sit
    // adjacent, then at each depth the candidate set skips an index whose value
    // repeats the previous one unless it is the first choice being tried at this
    // depth, so a duplicate value is never re-explored as if it were a fresh
    // branch and no subset is ever emitted twice.
    public static List<List<int>> FindAllSubsetsByBacktrackSkipDuplicates(int[] nums)
    {
        var sorted = (int[])nums.Clone();
        Array.Sort(sorted);

        var results = new List<List<int>>();
        var state = new BacktrackState();

        Backtrack.Search<BacktrackState, int>(
            state,
            _ => true,
            s => Enumerable.Range(s.Start, sorted.Length - s.Start)
                .Where(i => i == s.Start || sorted[i] != sorted[i - 1]),
            (s, i) =>
            {
                s.Starts.Push(s.Start);
                s.Values.Add(sorted[i]);
                s.Start = i + 1;
            },
            (s, _) =>
            {
                s.Start = s.Starts.Pop();
                s.Values.RemoveAt(s.Values.Count - 1);
            },
            s => results.Add([.. s.Values]));

        return results;
    }

    private sealed class BacktrackState
    {
        public List<int> Values { get; } = [];
        public int Start { get; set; }
        public Stack<int> Starts { get; } = new();
    }
}
