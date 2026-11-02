using DSAExperimentation.Algorithms.NumberTheory;
using DSAExperimentation.Algorithms.Sorting;

namespace DSAExperimentation.LeetCode.CountKSubsequencesOfAStringWithMaximumBeauty;

// LeetCode 2842. Count K-Subsequences of a String With Maximum Beauty: a
// k-subsequence is a subsequence of subsequenceLength characters, all pairwise
// distinct; its beauty is the sum, over its own characters, of how many times
// each occurs in text. Since every subsequence drawing on the SAME set of chosen
// characters has the same beauty (the set alone determines the sum), the
// question reduces to: pick the set of subsequenceLength distinct characters
// maximizing summed frequency, then count how many actual index-subsequences
// realize any set achieving that maximum - freq(c) index choices per chosen
// character c, summed over every maximizing set.
//
// CountByBruteForceCombinations enumerates every subset of text's distinct
// characters of size subsequenceLength by plain choose/recurse/unchoose recursion
// (the same "choose index > last chosen" combination walk CombinationSum/Subsets
// already use) and tracks the max summed frequency and the modular count of
// subsets achieving it directly - correct but O(C(26, k)) in the worst case, the
// baseline the grouped approach has to beat.
//
// CountByGroupedFrequencyProduct sorts each distinct character's own frequency
// descending via MergeSort/ArrayIndexedSequence<int> (same composition
// MaximumEleganceOfAKLengthSubsequenceSolution already uses) then walks groups
// of equal frequency from the top: a group entirely inside the top
// subsequenceLength is forced into every maximizing set (multiply in
// freq^groupSize), and the one boundary group that only PARTIALLY fits
// contributes C(groupSize, remaining) * freq^remaining - choose which `remaining`
// of its tied characters join, and each character in that chosen set is picked in
// freq(c) different actual index-positions. ModularPower computes
// freq^exponent and nCr(n, r) below divides by ModularInverse's Fermat
// inverse, the same factorial+inverse combination RoomWaysAlgebra's
// Factorial/ModularInverse.ByFermat pair already proves out for a
// different counting problem - recomputed per call rather than a shared
// FactorialTable<LeetCodeModulus> since n <= 26 here (one distinct-letter alphabet),
// the same "answers one LeetCode problem, lives beside the solution" scope
// RoomWaysAlgebra's own doc comment already states.
internal static class CountKSubsequencesOfAStringWithMaximumBeautySolution
{
    private const int AlphabetSize = 26;

    public static long CountByBruteForceCombinations(string text, int subsequenceLength)
    {
        var frequency = BuildFrequency(text);
        var chars = DistinctCharIndices(frequency);

        if (chars.Count < subsequenceLength)
        {
            return 0;
        }

        var state = new ComboState(frequency, chars, subsequenceLength);
        var best = FoldCombinations((Beauty: -1L, Count: 0L), state);

        return best.Count;
    }

    public static long CountByGroupedFrequencyProduct(string text, int subsequenceLength)
    {
        var frequency = BuildFrequency(text);
        var chars = DistinctCharIndices(frequency);

        if (chars.Count < subsequenceLength)
        {
            return 0;
        }

        var values = chars.Select(c => frequency[c]).ToArray();
        var descending = Comparer<int>.Create((a, b) => b.CompareTo(a));
        MergeSort.Sort(values, descending);

        var product = 1L;
        var remaining = subsequenceLength;
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
    // Returns the cursor past the group, what is left of subsequenceLength, and the
    // updated product.
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
        product = product * ModularPower.Of(value, taken, LeetCodeModulus.Value) % LeetCodeModulus.Value;
        product = product * Combinations(groupSize, taken) % LeetCodeModulus.Value;

        return (index, remaining - taken, product);
    }

    private static long Combinations(int itemCount, int chooseCount)
    {
        if (chooseCount == 0 || chooseCount == itemCount)
        {
            return 1;
        }

        var numerator = 1L;

        for (var i = 0; i < chooseCount; i++)
        {
            numerator = numerator * (itemCount - i) % LeetCodeModulus.Value;
        }

        var denominator = 1L;

        for (var i = 2; i <= chooseCount; i++)
        {
            denominator = denominator * i % LeetCodeModulus.Value;
        }

        return numerator * ModularInverse.ByFermat<LeetCodeModulus>(denominator) % LeetCodeModulus.Value;
    }

    // One node of the combination tree: a complete size-k subset folds into the running
    // best, a shorter one extends itself by each index past its last, recurses, and
    // removes it again.
    private static (long Beauty, long Count) FoldCombinations((long Beauty, long Count) best, ComboState state)
    {
        if (state.Chosen.Count == state.SubsequenceLength)
        {
            return FoldChosenSubset(best, state);
        }

        // Chosen holds strictly increasing indices, so the next candidate starts one past
        // the last one chosen - and at 0 when nothing is chosen yet, which is that same
        // "one past" read against a missing last index of -1.
        var start = state.Chosen.LastOrDefault(-1) + 1;

        for (var index = start; index < state.Chars.Count; index++)
        {
            state.Chosen.Add(index);
            best = FoldCombinations(best, state);
            state.Chosen.RemoveAt(state.Chosen.Count - 1);
        }

        return best;
    }

    // One complete size-k subset: its beauty is the summed frequencies of its characters
    // and its ways the product of those frequencies. A strictly better beauty replaces the
    // running best; an equal one adds its ways to that best's count.
    private static (long Beauty, long Count) FoldChosenSubset((long Beauty, long Count) best, ComboState chosen)
    {
        var beauty = 0L;
        var ways = 1L;

        foreach (var index in chosen.Chosen)
        {
            var count = chosen.Frequency[chosen.Chars[index]];
            beauty += count;
            ways = ways * count % LeetCodeModulus.Value;
        }

        if (beauty > best.Beauty)
        {
            return (beauty, ways);
        }

        if (beauty == best.Beauty)
        {
            return (best.Beauty, (best.Count + ways) % LeetCodeModulus.Value);
        }

        return best;
    }

    private static int[] BuildFrequency(string text)
    {
        var frequency = new int[AlphabetSize];

        foreach (var c in text)
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

    // The baseline's search: the facts every node reads, and the strictly increasing
    // indices into Chars chosen so far.
    private sealed record ComboState(int[] Frequency, List<int> Chars, int SubsequenceLength)
    {
        public List<int> Chosen { get; } = [];
    }
}
