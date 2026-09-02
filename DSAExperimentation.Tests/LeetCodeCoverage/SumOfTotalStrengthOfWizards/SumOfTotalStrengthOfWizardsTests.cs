using RepoIntStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SumOfTotalStrengthOfWizards;

// LeetCode 2281. Sum of Total Strength of Wizards: the same monotonic-stack contribution
// technique SumOfSubarrayMinimumsTests already uses over this repo's own Stack<int> - a
// previous-strictly-smaller sweep and a next-smaller-or-equal sweep, with the same </<=
// asymmetry resolving duplicate minimums to their leftmost occurrence so no subarray is ever
// double-counted - except each index now weights its minimum by every subarray SUM it is the
// minimum of, not just a subarray count. That weighted sum is computed in O(1) per index via a
// prefix-sum-of-prefix-sums array (PP[i+1] = PP[i] + prefix[i]) rather than re-summing each
// subarray, which is what keeps the whole pass O(n) instead of O(n^2).
public sealed partial class SumOfTotalStrengthOfWizardsTests
{
    private const int Modulus = 1_000_000_007;

    // Hand-verified by direct enumeration of all 6 subarrays of [1, 2, 3]:
    // [1]=1, [2]=4, [3]=9, [1,2]=1*3=3, [2,3]=2*5=10, [1,2,3]=1*6=6 -> total 33.
    [Fact]
    public void TotalStrength_IncreasingValues_ReturnsHandEnumeratedSum()
        => Assert.Equal(33, TotalStrength([1, 2, 3]));

    // Hand-verified by direct enumeration of all 6 subarrays of [2, 1, 2]:
    // [2]=4, [1]=1, [2]=4, [2,1]=1*3=3, [1,2]=1*3=3, [2,1,2]=1*5=5 -> total 20.
    // The repeated minimum (2 at index 0, 1 at index 1, 2 at index 2) exercises the same
    // </<= tie-breaking SumOfSubarrayMinimumsTests documents.
    [Fact]
    public void TotalStrength_DuplicateMinimums_DoesNotDoubleCount()
        => Assert.Equal(20, TotalStrength([2, 1, 2]));

    [Fact]
    public void TotalStrength_SingleElement_ReturnsItsSquare()
        => Assert.Equal(49, TotalStrength([7]));

    private static int TotalStrength(int[] strength)
    {
        var previousSmaller = ComputePreviousSmallerIndex(strength);
        var nextSmallerOrEqual = ComputeNextSmallerOrEqualIndex(strength);
        var prefix = ComputePrefixSums(strength);
        var prefixOfPrefix = ComputePrefixOfPrefixSums(prefix);

        return SumWeightedContributions(strength, previousSmaller, nextSmallerOrEqual, prefixOfPrefix);
    }

    private static int[] ComputePreviousSmallerIndex(int[] strength)
    {
        var left = new int[strength.Length];
        var stack = new RepoIntStack();

        for (var i = 0; i < strength.Length; i++)
        {
            while (stack.TryPeek(out var top) && strength[top] >= strength[i])
            {
                stack.TryPop(out _);
            }

            left[i] = stack.TryPeek(out var previous) ? previous : -1;
            stack.Push(i);
        }

        return left;
    }

    private static int[] ComputeNextSmallerOrEqualIndex(int[] strength)
    {
        var right = new int[strength.Length];
        var stack = new RepoIntStack();

        for (var i = strength.Length - 1; i >= 0; i--)
        {
            while (stack.TryPeek(out var top) && strength[top] > strength[i])
            {
                stack.TryPop(out _);
            }

            right[i] = stack.TryPeek(out var next) ? next : strength.Length;
            stack.Push(i);
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

    private static int SumWeightedContributions(int[] strength, int[] previousSmaller, int[] nextSmallerOrEqual, long[] prefixOfPrefix)
    {
        long total = 0;

        for (var i = 0; i < strength.Length; i++)
        {
            var left = previousSmaller[i];
            var right = nextSmallerOrEqual[i];

            var sumOfSumsEndingAtOrAfterI = prefixOfPrefix[right + 1] - prefixOfPrefix[i + 1];
            var sumOfSumsStartingAtOrBeforeI = prefixOfPrefix[i + 1] - prefixOfPrefix[left + 1];

            var weightedSum = ((long)(i - left) * sumOfSumsEndingAtOrAfterI) - ((long)(right - i) * sumOfSumsStartingAtOrBeforeI);
            var contribution = Mod(strength[i]) * Mod(weightedSum) % Modulus;

            total = (total + contribution) % Modulus;
        }

        return (int)total;
    }

    // weightedSum above can be negative (it's a difference of two large products), and C#'s %
    // preserves the dividend's sign - so a bare `% Modulus` alone can still return a negative
    // remainder, which the double-mod pattern below normalizes back into [0, Modulus).
    private static long Mod(long value) => ((value % Modulus) + Modulus) % Modulus;
}
