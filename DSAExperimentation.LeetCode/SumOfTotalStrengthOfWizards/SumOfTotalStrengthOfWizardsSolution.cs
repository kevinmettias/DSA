using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.ElementAlgebra;
using DSAExperimentation.DataStructures.PrefixSums;
using DSAExperimentation.Domain.Modular;

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
// arrays come from one NearestBoundary sweep each. What is new here is the weight: instead of counting those subarrays,
// each one contributes its sum, and the total of all of them is read off PrefixSums
// taken over the prefix-sum array itself (PP.TotalBefore(i+1) = PP.TotalBefore(i) +
// prefix[i]) in O(1) per index rather than re-summing each subarray. The whole pass
// stays O(n).
internal static class SumOfTotalStrengthOfWizardsSolution
{
    private const int NoSmallerElementToTheLeft = -1;

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
        // The >= / > asymmetry in the class doc, named here at the two call sites: the sweep
        // back to the left takes a value equal to this one, the sweep forward to the right
        // refuses it, so an equal minimum resolves against the earlier index and no subarray
        // is claimed twice. Both sentinels are one step outside the array, which is what lets
        // SumWeightedContributions read prefixOfPrefix.TotalBefore(left + 1) and
        // TotalBefore(right + 1) with no special case at either end.
        var previousSmaller = NearestBoundary.SmallerToTheLeft(strength, NoSmallerElementToTheLeft);
        var nextSmallerOrEqual = NearestBoundary.SmallerOrEqualToTheRight(strength, strength.Length);
        var prefix = ComputePrefixSums(strength);
        var prefixOfPrefix = new PrefixSums<long, SumOperation<long>>(prefix);

        return SumWeightedContributions(strength, previousSmaller, nextSmallerOrEqual, prefixOfPrefix);
    }

    // The prefix sums are themselves the values the outer PrefixSums totals, so they are
    // built as an array here: PrefixSums keeps its totals to itself.
    private static long[] ComputePrefixSums(int[] strength)
    {
        var prefix = new long[strength.Length + 1];

        for (var i = 0; i < strength.Length; i++)
        {
            prefix[i + 1] = prefix[i] + strength[i];
        }

        return prefix;
    }

    private static int SumWeightedContributions(
        int[] strength, int[] previousSmaller, int[] nextSmallerOrEqual, PrefixSums<long, SumOperation<long>> prefixOfPrefix)
    {
        long total = 0;

        for (var i = 0; i < strength.Length; i++)
        {
            var left = previousSmaller[i];
            var right = nextSmallerOrEqual[i];

            var sumOfSumsEndingAtOrAfterI = prefixOfPrefix.TotalBefore(right + 1) - prefixOfPrefix.TotalBefore(i + 1);
            var sumOfSumsStartingAtOrBeforeI = prefixOfPrefix.TotalBefore(i + 1) - prefixOfPrefix.TotalBefore(left + 1);

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
