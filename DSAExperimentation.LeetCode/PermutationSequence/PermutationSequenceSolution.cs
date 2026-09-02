using System.Text;
using DSAExperimentation.Algorithms.Backtracking;
using DSAExperimentation.DataStructures.DynamicArray;

namespace DSAExperimentation.LeetCode.PermutationSequence;

// LeetCode 60. Permutation Sequence: the k-th (1-indexed) permutation of "123...n".
//
// GetPermutationByBacktrackEnumeration is the textbook brute force: enumerate every
// permutation in lexicographic order via this repo's own generic Backtrack.TrySearch,
// stopping the instant the k-th is found - O(n!) worst case, every permutation before
// the target gets materialized. GetPermutationByFactoradicSelection is the clever
// O(n^2) approach: the factorial number system picks each digit directly out of a
// shrinking pool, over this repo's own DynamicArray<int> - Get(index) reads the next
// digit, RemoveAt(index) shrinks the pool, both operations the classic algorithm
// already needs.
internal static class PermutationSequenceSolution
{
    public static string GetPermutationByBacktrackEnumeration(int n, int k)
    {
        var progress = new SequenceSearchProgress(k);
        var state = new PermutationState(n);
        var steps = BuildSteps(n, progress);

        Backtrack.TrySearch<PermutationState, int>(state, steps);

        return progress.Found;
    }

    private static BacktrackingSteps<PermutationState, int> BuildSteps(int n, SequenceSearchProgress progress) => new(
        IsSolution: s => s.Values.Count == n,
        Candidates: s => s.Values.Count == n ? [] : Enumerable.Range(1, n).Where(d => !s.Used[d - 1]),
        Choose: (s, d) => { s.Used[d - 1] = true; s.Values.Add(d); },
        Unchoose: (s, d) => { s.Used[d - 1] = false; s.Values.RemoveAt(s.Values.Count - 1); },
        OnSolution: s => OnSolutionFound(s, progress));

    private static bool OnSolutionFound(PermutationState state, SequenceSearchProgress progress)
    {
        progress.Count++;
        if (progress.Count != progress.Target)
        {
            return false;
        }

        progress.Found = string.Concat(state.Values);
        return true;
    }

    public static string GetPermutationByFactoradicSelection(int n, int k)
    {
        var digits = new DynamicArray<int>();
        for (var d = 1; d <= n; d++)
        {
            digits.Add(d);
        }

        var factorial = BuildFactorialTable(n);

        k--;
        var result = new StringBuilder();
        for (var remaining = n; remaining >= 1; remaining--)
        {
            var digit = NextDigit(digits, factorial, remaining, ref k);
            result.Append(digit);
        }

        return result.ToString();
    }

    private static int[] BuildFactorialTable(int n)
    {
        var factorial = new int[n + 1];
        factorial[0] = 1;
        for (var i = 1; i <= n; i++)
        {
            factorial[i] = factorial[i - 1] * i;
        }

        return factorial;
    }

    private static int NextDigit(DynamicArray<int> digits, int[] factorial, int remaining, ref int k)
    {
        var index = k / factorial[remaining - 1];
        k %= factorial[remaining - 1];
        var digit = digits.Get(index);
        digits.RemoveAt(index);
        return digit;
    }

    private sealed class PermutationState(int length)
    {
        public bool[] Used { get; } = new bool[length];
        public List<int> Values { get; } = [];
    }

    private sealed class SequenceSearchProgress(int target)
    {
        public int Target { get; } = target;
        public int Count { get; set; }
        public string Found { get; set; } = string.Empty;
    }
}
