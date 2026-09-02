using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.MaximizeTheNumberOfPartitionsAfterOperations;

// LeetCode 3003. Maximize the Number of Partitions After Operations: change at
// most one character of s, then repeatedly strip the longest prefix containing at
// most k distinct characters as one partition until s is empty. Maximize the
// resulting partition count.
//
// Both strategies answer the same question with the same signature, so the test
// harness can assert they agree and the benchmark harness can time them against
// each other without either restating the algorithm.
internal static class MaximizeTheNumberOfPartitionsAfterOperationsSolution
{
    private const int AlphabetSize = 26;

    // The textbook baseline: try "no change" plus every one of the (at most)
    // 25 * Length single-character recolorings, and for each candidate string run
    // the greedy O(Length) partition walk directly - the O(Length^2 * 26) arm the
    // bitmask DP below has to beat.
    public static int MaxPartitionsByBruteForceRecolor(string s, int k)
    {
        var best = CountPartitions(s, k);
        var chars = s.ToCharArray();

        for (var i = 0; i < chars.Length; i++)
        {
            var original = chars[i];

            for (var letter = 0; letter < AlphabetSize; letter++)
            {
                var candidate = (char)('a' + letter);

                if (candidate == original)
                {
                    continue;
                }

                chars[i] = candidate;
                best = Math.Max(best, CountPartitions(new string(chars), k));
            }

            chars[i] = original;
        }

        return best;
    }

    private static int CountPartitions(string s, int k)
    {
        var partitions = 0;
        var mask = 0;
        var distinct = 0;

        foreach (var c in s)
        {
            var bit = 1 << (c - 'a');

            if ((mask & bit) == 0 && distinct == k)
            {
                partitions++;
                mask = 0;
                distinct = 0;
            }

            if ((mask & bit) == 0)
            {
                distinct++;
            }

            mask |= bit;
        }

        return mask == 0 ? partitions : partitions + 1;
    }

    // Memoizer walks the string once per reachable (position, current-partition
    // mask, change-still-available) state, trying every letter substitution right
    // where the recursion stands instead of materializing a whole new string per
    // candidate the way the brute force does. Same tuple-state
    // Memoizer.Memoize<TState,TResult> composition
    // CountTheNumberOfSquareFreeSubsetsSolution.CountByBitmaskMemo and
    // NumberOfBeautifulIntegersInTheRangeSolution.CountByDigitDpMemo already prove
    // out for unrelated counting recurrences.
    public static int MaxPartitionsByBitmaskMemo(string s, int k) =>
        Memoizer.Memoize<(int Position, int Mask, bool CanChange), int>(
            (0, 0, true),
            (state, recurse) => PartitionsFrom(state, s, k, recurse));

    private static int PartitionsFrom(
        (int Position, int Mask, bool CanChange) state,
        string s,
        int k,
        Func<(int Position, int Mask, bool CanChange), int> recurse)
    {
        if (state.Position == s.Length)
        {
            return state.Mask == 0 ? 0 : 1;
        }

        var originalLetter = s[state.Position] - 'a';
        var best = ExtendWith(state.Position, state.Mask, originalLetter, state.CanChange, k, recurse);

        if (!state.CanChange)
        {
            return best;
        }

        for (var letter = 0; letter < AlphabetSize; letter++)
        {
            if (letter == originalLetter)
            {
                continue;
            }

            best = Math.Max(best, ExtendWith(state.Position, state.Mask, letter, false, k, recurse));
        }

        return best;
    }

    private static int ExtendWith(
        int position, int mask, int letter, bool canChange, int k,
        Func<(int Position, int Mask, bool CanChange), int> recurse)
    {
        var extended = mask | (1 << letter);

        return System.Numerics.BitOperations.PopCount((uint)extended) <= k
            ? recurse((position + 1, extended, canChange))
            : 1 + recurse((position + 1, 1 << letter, canChange));
    }
}
