using DSAExperimentation.Algorithms.Backtracking;

namespace DSAExperimentation.LeetCode.CombinationSum;

// LeetCode 39. Combination Sum: every combination of candidates (each usable an
// unlimited number of times, candidates themselves guaranteed unique) that sums
// exactly to target.
//
// Sorting first lets both strategies stop scanning a branch's remaining
// candidates the moment the running sum would exceed target, instead of
// checking every one of them.
internal static class CombinationSumSolution
{
    // The textbook answer: plain recursion over a BCL List<int> path buffer,
    // added to the result list whenever it sums to target. Deliberately BCL-only
    // - the baseline FindCombinationsByBacktracking is measured against.
    public static List<List<int>> FindCombinationsBySpecializedRecursion(int[] candidates, int target)
    {
        var sorted = SortedCopy(candidates);
        var results = new List<List<int>>();
        var path = new List<int>();

        void SearchCombinations(int start, int remaining)
        {
            if (remaining == 0)
            {
                results.Add([.. path]);
                return;
            }

            for (var i = start; i < sorted.Length && sorted[i] <= remaining; i++)
            {
                path.Add(sorted[i]);
                SearchCombinations(i, remaining - sorted[i]);
                path.RemoveAt(path.Count - 1);
            }
        }

        SearchCombinations(0, target);
        return results;
    }

    // This repo's Backtrack.Search primitive drives the identical
    // choose/explore/unchoose shape declaratively; a Stack<int> restores Start on
    // unchoose so backtracking out of a choice never has to re-derive it.
    public static List<List<int>> FindCombinationsByBacktracking(int[] candidates, int target)
    {
        var sorted = SortedCopy(candidates);
        var results = new List<List<int>>();
        var state = new SearchState();

        Backtrack.Search<SearchState, int>(
            state,
            s => s.Sum == target,
            s => s.Sum == target
                ? NoCandidateIndices()
                : CandidateIndices(s, sorted, target),
            (s, i) => { s.Starts.Push(s.Start); s.Values.Add(sorted[i]); s.Sum += sorted[i]; s.Start = i; },
            (s, i) => { s.Start = s.Starts.Pop(); s.Sum -= sorted[i]; s.Values.RemoveAt(s.Values.Count - 1); },
            s => results.Add([.. s.Values]));

        return results;
    }

    private static IEnumerable<int> NoCandidateIndices() => [];

    // The candidate indices still worth trying from this state: those whose value
    // keeps the running sum at or below target.
    private static IEnumerable<int> CandidateIndices(SearchState state, int[] sorted, int target)
        => Enumerable.Range(state.Start, sorted.Length - state.Start)
            .Where(i => state.Sum + sorted[i] <= target);

    private static int[] SortedCopy(int[] candidates)
    {
        var sorted = (int[])candidates.Clone();
        Array.Sort(sorted);
        return sorted;
    }

    private sealed class SearchState
    {
        public Stack<int> Starts { get; } = new();
        public List<int> Values { get; } = [];
        public int Sum { get; set; }
        public int Start { get; set; }
    }
}
