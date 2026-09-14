using DSAExperimentation.Domain.Modular;
using RepoIntStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.LeetCode.SumOfTotalStrengthOfWizards;

// LeetCode 2281. Sum of Total Strength of Wizards: over every contiguous subarray,
// sum (that subarray's minimum) * (that subarray's total), modulo 1e9+7.
//
// TotalStrengthByBruteForce is the textbook O(n^2): fix a start index and extend a
// running minimum and a running sum rightwards, adding min * sum at every step.
//
// TotalStrengthByMonotonicStack is SumOfSubarrayMinimumsSolution's contribution
// technique with one extra weight. It flips the question from "what is each
// subarray's min times its sum" to "which subarrays is each element the minimum
// of, and what do they sum to". An element at i owns every subarray whose start
// lies in the run back to the previous strictly smaller element and whose end lies
// in the run forward to the next smaller-or-equal element - the same >= / >
// asymmetry between the two sweeps, resolving a run of equal minimums to its
// leftmost occurrence so no subarray is ever counted twice - and both boundary
// arrays come from one sweep each over this repo's own Stack<int> of pending
// indices. What is new here is the weight: instead of counting those subarrays,
// each one contributes its sum, and the total of all of them is read off a
// prefix-sum-of-prefix-sums array (PP[i+1] = PP[i] + prefix[i]) in O(1) per index
// rather than re-summing each subarray. The whole pass stays O(n).
internal static class SumOfTotalStrengthOfWizardsSolution
{
    // The textbook answer: every subarray's minimum and total, computed directly.
    // Deliberately written without this repo's primitives - it is the arm the
    // composed solution below has to justify itself against.
    public static int TotalStrengthByBruteForce(int[] strength)
    {
        long total = 0;

        for (var start = 0; start < strength.Length; start++)
        {
            var min = strength[start];
            long sum = 0;

            for (var end = start; end < strength.Length; end++)
            {
                min = Math.Min(min, strength[end]);
                sum += strength[end];
                total = (total + (min * (sum % ModularArithmetic.Modulo) % ModularArithmetic.Modulo)) % ModularArithmetic.Modulo;
            }
        }

        return (int)total;
    }

    public static int TotalStrengthByMonotonicStack(int[] strength)
    {
        var previousSmaller = ComputePreviousSmallerIndex(strength);
        var nextSmallerOrEqual = ComputeNextSmallerOrEqualIndex(strength);
        var prefix = ComputePrefixSums(strength);
        var prefixOfPrefix = ComputePrefixOfPrefixSums(prefix);

        return SumWeightedContributions(strength, previousSmaller, nextSmallerOrEqual, prefixOfPrefix);
    }

    // previousSmaller[i] = the nearest index to the left holding a strictly smaller
    // value, or -1 when no such element exists.
    private static int[] ComputePreviousSmallerIndex(int[] strength)
    {
        var left = new int[strength.Length];
        var pendingIndices = new RepoIntStack();

        for (var i = 0; i < strength.Length; i++)
        {
            while (pendingIndices.TryPeek(out var top) && strength[top] >= strength[i])
            {
                pendingIndices.TryPop(out _);
            }

            left[i] = pendingIndices.TryPeek(out var previous) ? previous : -1;
            pendingIndices.Push(i);
        }

        return left;
    }

    // The mirror image, stopping at the next smaller-or-equal element rather than
    // the next strictly smaller one, so equal values never both claim the same
    // subarray. strength.Length when no such element exists.
    private static int[] ComputeNextSmallerOrEqualIndex(int[] strength)
    {
        var right = new int[strength.Length];
        var pendingIndices = new RepoIntStack();

        for (var i = strength.Length - 1; i >= 0; i--)
        {
            while (pendingIndices.TryPeek(out var top) && strength[top] > strength[i])
            {
                pendingIndices.TryPop(out _);
            }

            right[i] = pendingIndices.TryPeek(out var next) ? next : strength.Length;
            pendingIndices.Push(i);
        }

        return right;
    }

    private static long[] ComputePrefixSums(int[] strength)
    {
        var prefix = new long[strength.Length + 1];

        for (var i = 0; i < strength.Length; i++)
        {
            prefix[i + 1] = prefix[i] + strength[i];
        }

        return prefix;
    }

    private static long[] ComputePrefixOfPrefixSums(long[] prefix)
    {
        var prefixOfPrefix = new long[prefix.Length + 1];

        for (var i = 0; i < prefix.Length; i++)
        {
            prefixOfPrefix[i + 1] = prefixOfPrefix[i] + prefix[i];
        }

        return prefixOfPrefix;
    }

    private static int SumWeightedContributions(
        int[] strength, int[] previousSmaller, int[] nextSmallerOrEqual, long[] prefixOfPrefix)
    {
        long total = 0;

        for (var i = 0; i < strength.Length; i++)
        {
            var left = previousSmaller[i];
            var right = nextSmallerOrEqual[i];

            var sumOfSumsEndingAtOrAfterI = prefixOfPrefix[right + 1] - prefixOfPrefix[i + 1];
            var sumOfSumsStartingAtOrBeforeI = prefixOfPrefix[i + 1] - prefixOfPrefix[left + 1];

            var weightedSum = ((long)(i - left) * sumOfSumsEndingAtOrAfterI) - ((long)(right - i) * sumOfSumsStartingAtOrBeforeI);
            var contribution = Mod(strength[i]) * Mod(weightedSum) % ModularArithmetic.Modulo;

            total = (total + contribution) % ModularArithmetic.Modulo;
        }

        return (int)total;
    }

    // weightedSum above can be negative (it is a difference of two large products),
    // and C#'s % preserves the dividend's sign - so a bare `% Modulo` alone can
    // still return a negative remainder, which the double-mod pattern below
    // normalizes back into [0, Modulo).
    private static long Mod(long value) => ((value % ModularArithmetic.Modulo) + ModularArithmetic.Modulo) % ModularArithmetic.Modulo;
}
