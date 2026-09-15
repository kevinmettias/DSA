using DSAExperimentation.Algorithms.Backtracking;
using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;
using DSAExperimentation.Domain.Modular;

namespace DSAExperimentation.LeetCode.CountKSubsequencesOfAStringWithMaximumBeauty;

// LeetCode 2842. Count K-Subsequences of a String With Maximum Beauty: a
// k-subsequence is a length-k subsequence whose characters are pairwise
// distinct; its beauty is the sum, over its own characters, of how many times
// each occurs in s. Since every subsequence with the SAME set of k distinct
// characters has the same beauty (the set alone determines the sum), the
// question reduces to: pick the k-character SET maximizing summed frequency,
// then count how many actual index-subsequences realize any set achieving that
// maximum - freq(c) index choices per chosen character c, summed over every
// maximizing set.
//
// CountByBruteForceCombinations enumerates every size-k subset of s's distinct
// characters via Backtrack (the same "choose index > last chosen" combination
// walk CombinationSum/Subsets already use) and tracks the max summed frequency
// and the modular count of subsets achieving it directly - correct but O(C(26,
// k)) in the worst case, the baseline the grouped approach has to beat.
//
// CountByGroupedFrequencyProduct sorts each distinct character's own frequency
// descending via MergeSort/ArrayIndexedSequence<int> (same composition
// MaximumEleganceOfAKLengthSubsequenceSolution already uses) then walks groups
// of equal frequency from the top: a group entirely inside the top k is forced
// into every maximizing set (multiply in freq^groupSize), and the one boundary
// group that only PARTIALLY fits contributes C(groupSize, remaining) *
// freq^remaining - choose which `remaining` of its tied characters join, and
// each character in that chosen set is picked in freq(c) different actual
// index-positions. ModularArithmetic.Power computes freq^exponent and
// nCr(n, r) below reuses ModularArithmetic.Inverse for the modular Fermat
// inverse, the same factorial+inverse combination
// RoomWaysAlgebra.Factorial/ModularArithmetic.Inverse already prove out for a
// different counting problem - recomputed per call rather than a shared
// Domain-level nCr helper since n <= 26 here (one distinct-letter alphabet),
// the same "answers one LeetCode problem, lives beside the solution" scope
// RoomWaysAlgebra's own doc comment already states.
internal static class CountKSubsequencesOfAStringWithMaximumBeautySolution
{
    private const int AlphabetSize = 26;

    public static long CountByBruteForceCombinations(string s, int k)
    {
        var frequency = BuildFrequency(s);
        var chars = DistinctCharIndices(frequency);

        if (chars.Count < k)
        {
            return 0;
        }

        var best = (Beauty: -1L, Count: 0L);
        var state = new ComboState();

        Backtrack.Search<ComboState, int>(
            state,
            st => st.Chosen.Count == k,
            st => CandidateIndices(st, chars.Count, k),
            (st, index) => st.Chosen.Add(index),
            (st, _) => st.Chosen.RemoveAt(st.Chosen.Count - 1),
            st => best = FoldChosenSubset(best, st, frequency, chars));

        return best.Count;
    }

    private static IEnumerable<int> CandidateIndices(ComboState state, int charCount, int k)
    {
        if (state.Chosen.Count == k)
        {
            return [];
        }

        // Chosen holds strictly increasing indices, so the next candidate starts one past
        // the last one chosen - and at 0 when nothing is chosen yet, which is that same
        // "one past" read against a missing last index of -1.
        var start = state.Chosen.LastOrDefault(-1) + 1;
        return Enumerable.Range(start, charCount - start);
    }

    // One complete size-k subset: its beauty is the summed frequencies of its characters
    // and its ways the product of those frequencies. A strictly better beauty replaces the
    // running best; an equal one adds its ways to that best's count.
    private static (long Beauty, long Count) FoldChosenSubset(
        (long Beauty, long Count) best, ComboState chosen, int[] frequency, List<int> chars)
    {
        var beauty = 0L;
        var ways = 1L;

        foreach (var index in chosen.Chosen)
        {
            beauty += frequency[chars[index]];
            ways = ways * frequency[chars[index]] % ModularArithmetic.Modulo;
        }

        if (beauty > best.Beauty)
        {
            return (beauty, ways);
        }

        if (beauty == best.Beauty)
        {
            return (best.Beauty, (best.Count + ways) % ModularArithmetic.Modulo);
        }

        return best;
    }

    public static long CountByGroupedFrequencyProduct(string s, int k)
    {
        var frequency = BuildFrequency(s);
        var chars = DistinctCharIndices(frequency);

        if (chars.Count < k)
        {
            return 0;
        }

        var values = chars.Select(c => frequency[c]).ToArray();
        var descending = Comparer<int>.Create((a, b) => b.CompareTo(a));
        MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(values), descending);

        var product = 1L;
        var remaining = k;
        var index = 0;

        while (remaining > 0)
        {
            (index, remaining, product) = TakeTopFrequencyGroup(values, index, remaining, product);
        }

        return product;
    }

    // Consume one group of equal frequency from the top: `remaining` of its characters
    // join the chosen set (all of them when the group fits entirely), each contributing
    // freq choices of index and each set of `remaining` counted by nCr(groupSize, taken).
    // Returns the cursor past the group, what is left of k, and the updated product.
    private static (int Index, int Remaining, long Product) TakeTopFrequencyGroup(
        int[] values, int index, int remaining, long product)
    {
        var value = values[index];
        var groupSize = 0;

        while (index < values.Length && values[index] == value)
        {
            groupSize++;
            index++;
        }

        var taken = Math.Min(groupSize, remaining);
        product = product * ModularArithmetic.Power(value, taken) % ModularArithmetic.Modulo;
        product = product * Combinations(groupSize, taken) % ModularArithmetic.Modulo;

        return (index, remaining - taken, product);
    }

    private static long Combinations(int n, int r)
    {
        if (r == 0 || r == n)
        {
            return 1;
        }

        var numerator = 1L;

        for (var i = 0; i < r; i++)
        {
            numerator = numerator * (n - i) % ModularArithmetic.Modulo;
        }

        var denominator = 1L;

        for (var i = 2; i <= r; i++)
        {
            denominator = denominator * i % ModularArithmetic.Modulo;
        }

        return numerator * ModularArithmetic.Inverse(denominator) % ModularArithmetic.Modulo;
    }

    private static int[] BuildFrequency(string s)
    {
        var frequency = new int[AlphabetSize];

        foreach (var c in s)
        {
            frequency[c - 'a']++;
        }

        return frequency;
    }

    private static List<int> DistinctCharIndices(int[] frequency)
    {
        var chars = new List<int>();

        for (var c = 0; c < AlphabetSize; c++)
        {
            if (frequency[c] > 0)
            {
                chars.Add(c);
            }
        }

        return chars;
    }

    private sealed record ComboState
    {
        public List<int> Chosen { get; } = [];
    }
}
