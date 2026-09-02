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

        var maxBeauty = -1L;
        var count = 0L;
        var state = new ComboState();

        Backtrack.Search<ComboState, int>(
            state,
            st => st.Chosen.Count == k,
            st => CandidateIndices(st, chars.Count, k),
            (st, index) => st.Chosen.Add(index),
            (st, _) => st.Chosen.RemoveAt(st.Chosen.Count - 1),
            st =>
            {
                var beauty = 0L;
                var ways = 1L;

                foreach (var index in st.Chosen)
                {
                    beauty += frequency[chars[index]];
                    ways = ways * frequency[chars[index]] % ModularArithmetic.Modulo;
                }

                if (beauty > maxBeauty)
                {
                    maxBeauty = beauty;
                    count = ways;
                }
                else if (beauty == maxBeauty)
                {
                    count = (count + ways) % ModularArithmetic.Modulo;
                }
            });

        return count;
    }

    private static IEnumerable<int> CandidateIndices(ComboState state, int charCount, int k)
    {
        if (state.Chosen.Count == k)
        {
            return [];
        }

        var start = state.Chosen.Count == 0 ? 0 : state.Chosen[^1] + 1;
        return Enumerable.Range(start, charCount - start);
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
            remaining -= taken;
        }

        return product;
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

    private sealed class ComboState
    {
        public List<int> Chosen { get; } = [];
    }
}
