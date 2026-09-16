using System.Text;
using DSAExperimentation.Algorithms.Backtracking;
using DSAExperimentation.DataStructures.DynamicArray;

namespace DSAExperimentation.LeetCode.PermutationSequence;

// LeetCode 60. Permutation Sequence: the permutation sitting at a given 1-indexed
// rank in the lexicographic order of the digits 1..digitCount.
//
// GetPermutationByBacktrackEnumeration is the textbook brute force: enumerate every
// permutation in lexicographic order via this repo's own generic Backtrack.TrySearch,
// stopping the instant the requested permutation is found - O(n!) worst case, every
// permutation before the target gets materialized. GetPermutationByFactoradicSelection
// is the clever O(n^2) approach: the factorial number system picks each digit directly
// out of a shrinking pool, over this repo's own DynamicArray<int> - Get(index) reads
// the next digit, RemoveAt(index) shrinks the pool, both operations the classic
// algorithm already needs.
internal static class PermutationSequenceSolution
{
    public static string GetPermutationByBacktrackEnumeration(int digitCount, int rank)
    {
        var progress = new SequenceSearchProgress(rank);
        var state = new PermutationState(digitCount);
        var steps = BuildSteps(digitCount, progress);

        Backtrack.TrySearch<PermutationState, int>(state, steps);

        return progress.Found;
    }

    private static BacktrackingSteps<PermutationState, int> BuildSteps(int digitCount, SequenceSearchProgress progress) => new(
        IsSolution: s => s.Values.Count == digitCount,
        Candidates: s => s.Values.Count == digitCount ? Array.Empty<int>() : UnusedDigits(digitCount, s),
        Choose: (s, d) => { s.Used[d - 1] = true; s.Values.Add(d); },
        Unchoose: (s, d) => { s.Used[d - 1] = false; s.Values.RemoveAt(s.Values.Count - 1); },
        OnSolution: s => ShouldStopSearch(s, progress));

    // Every digit not yet placed - a lazy pipeline the engine re-reads per MoveNext,
    // which is what lets it observe Unchoose restoring a digit to the pool.
    private static IEnumerable<int> UnusedDigits(int digitCount, PermutationState state)
        => Enumerable.Range(1, digitCount).Where(d => !state.Used[d - 1]);

    // Records the permutation the moment the walk reaches the requested rank, and
    // answers whether the whole search should stop there.
    private static bool ShouldStopSearch(PermutationState state, SequenceSearchProgress progress)
    {
        progress.Count++;
        if (progress.Count != progress.Target)
        {
            return false;
        }

        progress.Found = string.Concat(state.Values);
        return true;
    }

    public static string GetPermutationByFactoradicSelection(int digitCount, int rank)
    {
        var digits = new DynamicArray<int>();
        for (var d = 1; d <= digitCount; d++)
        {
            digits.Add(d);
        }

        var factorial = BuildFactorialTable(digitCount);

        rank--;
        var result = new StringBuilder();
        for (var remaining = digitCount; remaining >= 1; remaining--)
        {
            var digit = NextDigit(digits, factorial, remaining, ref rank);
            result.Append(digit);
        }

        return result.ToString();
    }

    private static int[] BuildFactorialTable(int digitCount)
    {
        var factorial = new int[digitCount + 1];
        factorial[0] = 1;
        for (var i = 1; i <= digitCount; i++)
        {
            factorial[i] = factorial[i - 1] * i;
        }

        return factorial;
    }

    private static int NextDigit(DynamicArray<int> digits, int[] factorial, int remaining, ref int rank)
    {
        var index = rank / factorial[remaining - 1];
        rank %= factorial[remaining - 1];
        return TakeDigitAt(digits, index);
    }

    // The chosen digit leaves the pool as it is read, so every later pick runs over the
    // already-shrunken set - which is what makes each digit appear at most once.
    private static int TakeDigitAt(DynamicArray<int> digits, int index)
    {
        var digit = digits.Get(index);
        digits.RemoveAt(index);
        return digit;
    }

    private sealed record PermutationState
    {
        public bool[] Used { get; }
        public List<int> Values { get; } = [];

        public PermutationState(int length) => Used = new bool[length];
    }

    private sealed class SequenceSearchProgress(int target)
    {
        public int Target { get; } = target;
        public int Count { get; set; }
        public string Found { get; set; } = string.Empty;
    }
}
