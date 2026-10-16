using System.Text;
using DSAExperimentation.DataStructures.DynamicArray;

namespace DSAExperimentation.LeetCode.PermutationSequence;

// LeetCode 60. Permutation Sequence: the permutation sitting at a given 1-indexed
// rank in the lexicographic order of the digits 1..digitCount.
//
// GetPermutationByBacktrackEnumeration is the textbook brute force: enumerate every
// permutation in lexicographic order by plain choose/recurse/unchoose recursion,
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

        TrySearchPermutations(state, digitCount, progress);

        return progress.Found;
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

    // One node of the permutation tree: a full permutation counts toward the rank, a
    // shorter one extends itself by each unused digit. True once the requested rank is
    // reached, which stops the whole search.
    private static bool TrySearchPermutations(PermutationState state, int digitCount, SequenceSearchProgress progress)
    {
        if (state.Values.Count == digitCount)
        {
            return ShouldStopSearch(state, progress);
        }

        return TryEachUnusedDigit(state, digitCount, progress);
    }

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

    // Each digit not yet placed, ascending so the permutations arrive in lexicographic
    // order: place it, search below it, take it back, and stop as soon as one branch has
    // reached the requested rank.
    private static bool TryEachUnusedDigit(PermutationState state, int digitCount, SequenceSearchProgress progress)
    {
        for (var digit = 1; digit <= digitCount; digit++)
        {
            if (state.Used[digit - 1])
            {
                continue;
            }

            state.Used[digit - 1] = true;
            state.Values.Add(digit);
            var stopped = TrySearchPermutations(state, digitCount, progress);
            state.Used[digit - 1] = false;
            state.Values.RemoveAt(state.Values.Count - 1);

            if (stopped)
            {
                return true;
            }
        }

        return false;
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
