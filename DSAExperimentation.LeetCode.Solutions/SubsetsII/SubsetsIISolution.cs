using DSAExperimentation.Algorithms.Backtracking;

namespace DSAExperimentation.LeetCode.Solutions.SubsetsII;

// LeetCode 90. Subsets II: given an integer array that may contain duplicates,
// return every possible subset (the power set), with no duplicate subset in the
// result.
//
// The [Benchmark(Baseline = true)] "IterativeDedup" arm this migrated out of never
// implemented an independent algorithm - its body built an unused HashSet<string>,
// looped over a discarded HashSet<int> doing nothing, and then returned this same
// backtracking arm's own value directly. It was dead code wrapping the one real
// strategy, not a second strategy. Both strategies now are that one walk - sort, then
// skip a value that repeats the one before it at the same depth - written as plain
// recursion, or run on this repo's backtracking combinator.
internal static class SubsetsIISolution
{
    // The textbook answer: the sort-then-skip walk as plain recursion, the arm the
    // backtracking combinator below is measured against (ARCHITECTURE 17.5).
    public static List<List<int>> FindAllSubsetsBySkipDuplicatesRecursion(int[] nums)
    {
        var sorted = (int[])nums.Clone();
        Array.Sort(sorted);
        var results = new List<List<int>>();

        CollectSubsets(sorted, 0, [], results);

        return results;
    }

    // This repo's own backtracking combinator: sort first so duplicates sit
    // adjacent, then at each depth the candidate set skips an index whose value
    // repeats the previous one unless it is the first choice being tried at this
    // depth, so a duplicate value is never re-explored as if it were a fresh
    // branch and no subset is ever emitted twice.
    public static List<List<int>> FindAllSubsetsByBacktrackSkipDuplicates(int[] nums)
    {
        var sorted = (int[])nums.Clone();
        Array.Sort(sorted);

        return SearchSubsets(sorted);
    }

    // The search itself: at every depth the candidate set skips an index whose value
    // repeats the previous one unless it is the first choice tried there, and every
    // state the walk reaches is one subset of the result.
    private static List<List<int>> SearchSubsets(int[] sorted)
    {
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

    // Every subset that extends `chosen` with values from `start` on: `chosen` itself, then
    // each next value in turn, skipping one that repeats the value before it at this depth
    // so a duplicate is never explored as a fresh branch.
    private static void CollectSubsets(int[] sorted, int start, List<int> chosen, List<List<int>> results)
    {
        results.Add([.. chosen]);

        for (var i = start; i < sorted.Length; i++)
        {
            if (i > start && sorted[i] == sorted[i - 1])
            {
                continue;
            }

            chosen.Add(sorted[i]);
            CollectSubsets(sorted, i + 1, chosen, results);
            chosen.RemoveAt(chosen.Count - 1);
        }
    }

    private sealed class BacktrackState
    {
        public List<int> Values { get; } = [];
        public int Start { get; set; }
        public Stack<int> Starts { get; } = new();
    }
}
