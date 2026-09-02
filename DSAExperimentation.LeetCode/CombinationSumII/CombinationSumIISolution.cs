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

        void Search(int start, int remaining)
        {
            if (remaining == 0)
            {
                results.Add([.. path]);
                return;
            }

            for (var i = start; i < sorted.Length && sorted[i] <= remaining; i++)
            {
                if (i > start && sorted[i] == sorted[i - 1])
                {
                    continue;
                }

                path.Add(sorted[i]);
                Search(i + 1, remaining - sorted[i]);
                path.RemoveAt(path.Count - 1);
            }
        }

        Search(0, target);
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
            s => s.Sum == target ? [] : NextCandidates(sorted, target, s),
            (s, i) => { s.Starts.Push(s.Start); s.Values.Add(sorted[i]); s.Sum += sorted[i]; s.Start = i + 1; },
            (s, i) => { s.Start = s.Starts.Pop(); s.Sum -= sorted[i]; s.Values.RemoveAt(s.Values.Count - 1); },
            s => results.Add([.. s.Values]));

        return results;
    }

    private static IEnumerable<int> NextCandidates(int[] sorted, int target, SearchState s)
    {
        for (var i = s.Start; i < sorted.Length; i++)
        {
            if (i > s.Start && sorted[i] == sorted[i - 1])
            {
                continue;
            }

            if (s.Sum + sorted[i] <= target)
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

    private sealed class SearchState
    {
        public Stack<int> Starts { get; } = new();
        public List<int> Values { get; } = [];
        public int Sum { get; set; }
        public int Start { get; set; }
    }
}
