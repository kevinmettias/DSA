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
        EnumerateSubsets(MinDigit, (k, n), chosen, results);
        return results;
    }

    public static List<List<int>> CombinationsByBacktrackEngine(int k, int n)
    {
        var results = new List<List<int>>();
        var state = new SearchState();

        Backtrack.Search<SearchState, int>(
            state,
            isSolution: s => s.Values.Count == k && s.Sum == n,
            candidates: s => s.Values.Count == k || s.Sum >= n
                ? NoCandidates()
                : CandidatesFor(s, n),
            choose: ApplyChoice,
            unchoose: UndoChoice,
            onSolution: s => results.Add([.. s.Values]));

        return results;
    }

    // No digit is still admissible: the combination is complete, or its running
    // sum has already reached the target, which the positive digits can only
    // overshoot from here.
    private static IEnumerable<int> NoCandidates() => [];

    // The digits still admissible from a state: the increasing run from its next
    // digit through 9, keeping only those that leave the running sum at or below
    // the target.
    private static IEnumerable<int> CandidatesFor(SearchState state, int targetSum) =>
        Enumerable.Range(state.Start, MaxDigit - state.Start + 1).Where(v => state.Sum + v <= targetSum);

    // Applies one chosen digit to the running state: remember where the admissible run
    // started, extend the chosen values, add the digit to the running sum, and move past
    // it - the four mutations Backtrack.Search's `choose` hook exists to perform.
    private static void ApplyChoice(SearchState state, int value)
    {
        state.Starts.Push(state.Start);
        state.Values.Add(value);
        state.Sum += value;
        state.Start = value + 1;
    }

    // The exact inverse of ApplyChoice, in reverse order: restore the start of the
    // admissible run, drop the digit from the running sum, and drop it from the values.
    private static void UndoChoice(SearchState state, int value)
    {
        state.Start = state.Starts.Pop();
        state.Sum -= value;
        state.Values.RemoveAt(state.Values.Count - 1);
    }

    // What is still owed to the target: how many more digits must be chosen and how
    // much sum they must still supply - the two counts the recursion decrements in
    // lockstep and tests together at its base case.
    private static void EnumerateSubsets(
        int next, (int Count, int Sum) remaining, List<int> chosen, List<List<int>> results)
    {
        var (remainingCount, remainingSum) = remaining;

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
            EnumerateSubsets(digit + 1, (remainingCount - 1, remainingSum - digit), chosen, results);
            chosen.RemoveAt(chosen.Count - 1);
        }
    }

    private sealed class SearchState
    {
        public List<int> Values { get; } = [];

        public Stack<int> Starts { get; } = new();

        public int Sum { get; set; }

        public int Start { get; set; } = MinDigit;
    }
}
