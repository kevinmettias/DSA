using DSAExperimentation.Algorithms.Backtracking;

namespace DSAExperimentation.LeetCode.CombinationSumIII;

// LeetCode 216. Combination Sum III: choose k distinct digits from 1..9 that sum to
// n, returned as every valid combination - LeetCode's own answer shape, a
// List<List<int>>, not merely whether one exists.
//
// The two strategies differ in how the k-subset space is pruned: brute force walks
// every increasing digit sequence of length k and checks the sum only once the
// sequence is complete; the backtracking strategy uses this repo's Backtrack.Search
// engine to stop descending the moment a partial sum can no longer reach n, so it
// never finishes exploring a branch it already knows is too large.
internal static class CombinationSumIIISolution
{
    private const int MinDigit = 1;
    private const int MaxDigit = 9;

    public static List<List<int>> CombinationsByBruteForce(int k, int n)
    {
        var results = new List<List<int>>();
        var chosen = new List<int>();
        EnumerateSubsets(MinDigit, k, n, chosen, results);
        return results;
    }

    private static void EnumerateSubsets(int next, int remainingCount, int remainingSum, List<int> chosen, List<List<int>> results)
    {
        if (remainingCount == 0)
        {
            if (remainingSum == 0)
            {
                results.Add([.. chosen]);
            }

            return;
        }

        for (var digit = next; digit <= MaxDigit; digit++)
        {
            chosen.Add(digit);
            EnumerateSubsets(digit + 1, remainingCount - 1, remainingSum - digit, chosen, results);
            chosen.RemoveAt(chosen.Count - 1);
        }
    }

    public static List<List<int>> CombinationsByBacktrackEngine(int k, int n)
    {
        var results = new List<List<int>>();
        var state = new SearchState();

        Backtrack.Search<SearchState, int>(
            state,
            isSolution: s => s.Values.Count == k && s.Sum == n,
            candidates: s => s.Values.Count == k || s.Sum >= n
                ? []
                : Enumerable.Range(s.Start, MaxDigit - s.Start + 1).Where(v => s.Sum + v <= n),
            choose: (s, v) =>
            {
                s.Starts.Push(s.Start);
                s.Values.Add(v);
                s.Sum += v;
                s.Start = v + 1;
            },
            unchoose: (s, v) =>
            {
                s.Start = s.Starts.Pop();
                s.Sum -= v;
                s.Values.RemoveAt(s.Values.Count - 1);
            },
            onSolution: s => results.Add([.. s.Values]));

        return results;
    }

    private sealed class SearchState
    {
        public List<int> Values { get; } = [];

        public Stack<int> Starts { get; } = new();

        public int Sum { get; set; }

        public int Start { get; set; } = MinDigit;
    }
}
