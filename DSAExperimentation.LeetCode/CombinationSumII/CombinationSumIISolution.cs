using DSAExperimentation.Algorithms.Backtracking;

namespace DSAExperimentation.LeetCode.CombinationSumII;

// LeetCode 40. Combination Sum II: every combination of candidates (each usable at
// most once, duplicate values allowed in the input) that sums to target, with no
// duplicate combination in the output.
//
// Sorting groups equal values together so both strategies can skip a repeated
// candidate at the same recursion depth - the standard way to avoid the same
// combination being emitted twice without needing a results set.
internal static class CombinationSumIISolution
{
    // The textbook answer: plain recursion over a BCL List<int> path buffer,
    // skipping a duplicate candidate at the same depth. Deliberately BCL-only -
    // the baseline FindCombinationsByBacktracking is measured against.
    public static List<List<int>> FindCombinationsBySpecializedRecursion(int[] candidates, int target)
    {
        var sorted = SortedCopy(candidates);
        var results = new List<List<int>>();
        var path = new List<int>();

        SearchCandidates((sorted, results, path), 0, target);

        return results;
    }

    // This repo's Backtrack.Search primitive drives the identical
    // choose/explore/unchoose shape declaratively, with the same duplicate-skip
    // folded into the candidate enumeration; a Stack<int> restores Start on
    // unchoose.
    public static List<List<int>> FindCombinationsByBacktracking(int[] candidates, int target)
    {
        var sorted = SortedCopy(candidates);
        var results = new List<List<int>>();
        var state = new SearchState();

        Backtrack.Search<SearchState, int>(
            state,
            s => s.Sum == target,
            s => s.Sum == target ? NoCandidates() : NextCandidates(sorted, target, s),
            (s, i) => { s.Starts.Push(s.Start); s.Values.Add(sorted[i]); s.Sum += sorted[i]; s.Start = i + 1; },
            (s, i) => { s.Start = s.Starts.Pop(); s.Sum -= sorted[i]; s.Values.RemoveAt(s.Values.Count - 1); },
            s => results.Add([.. s.Values]));

        return results;
    }

    // The running sum has already reached the target, so the pruned enumeration has
    // nothing left to offer.
    private static IEnumerable<int> NoCandidates() => [];

    private static IEnumerable<int> NextCandidates(int[] sorted, int target, SearchState state)
    {
        for (var i = state.Start; i < sorted.Length; i++)
        {
            if (i > state.Start && sorted[i] == sorted[i - 1])
            {
                continue;
            }

            if (state.Sum + sorted[i] <= target)
            {
                yield return i;
            }
        }
    }

    private static int[] SortedCopy(int[] candidates)
    {
        var sorted = (int[])candidates.Clone();
        Array.Sort(sorted);
        return sorted;
    }

    // The recursion itself: take each distinct candidate at this depth, explore past it
    // and then unchoose it. The sorted candidates, the path being built and the results
    // collected travel together as the one search state they are.
    private static void SearchCandidates(
        (int[] Sorted, List<List<int>> Results, List<int> Path) state, int start, int remaining)
    {
        if (remaining == 0)
        {
            state.Results.Add([.. state.Path]);
            return;
        }

        for (var i = start; i < state.Sorted.Length && state.Sorted[i] <= remaining; i++)
        {
            if (i > start && state.Sorted[i] == state.Sorted[i - 1])
            {
                continue;
            }

            state.Path.Add(state.Sorted[i]);
            SearchCandidates(state, i + 1, remaining - state.Sorted[i]);
            state.Path.RemoveAt(state.Path.Count - 1);
        }
    }

    private sealed class SearchState
    {
        public Stack<int> Starts { get; } = new();
        public List<int> Values { get; } = [];
        public int Sum { get; set; }
        public int Start { get; set; }
    }
}
