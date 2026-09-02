using DSAExperimentation.DataStructures.HashMap;

using RepoRangeFenwickTree = DSAExperimentation.DataStructures.RangeFenwickTree.RangeFenwickTree<long, DSAExperimentation.DataStructures.RangeFenwickTree.ScaledSumOperation<long>>;

namespace DSAExperimentation.LeetCode.SubarraysDistinctElementSumOfSquaresII;

// LeetCode 2916. Subarrays Distinct Element Sum of Squares II: sum, over every
// subarray [l, r], of distinct(l, r)^2, modulo 1e9+7.
//
// Both strategies answer the same question with the same signature, so the test
// harness can assert they agree and the benchmark harness can time them against
// each other without either restating the algorithm.
internal static class SubarraysDistinctElementSumOfSquaresIISolution
{
    private const long Mod = 1_000_000_007;

    // Textbook O(n^2): for each start l, grow r rightward tracking distinct count
    // in a plain HashSet, adding its square to the running total. The arm the
    // Fenwick-tree strategy has to beat.
    public static long SumOfSquaresByBruteForce(int[] nums)
    {
        var answer = 0L;

        for (var left = 0; left < nums.Length; left++)
        {
            var seen = new HashSet<int>();

            for (var right = left; right < nums.Length; right++)
            {
                seen.Add(nums[right]);
                answer = (answer + ((long)seen.Count * seen.Count)) % Mod;
            }
        }

        return answer;
    }

    // Composed: fix D[l] = distinct(l, r) for the current right endpoint r, for
    // every start l in [0, r]. Extending r by one value only raises D[l] for
    // l in (previousOccurrence, r] - the value is already present for any earlier
    // start - a range +1 this repo's own RangeFenwickTree<long, ScaledSumOperation<long>>
    // applies in O(log n) (same composition MaximizeTheMinimumPoweredCityTests
    // uses). Squaring is not additive, but its *change* is:
    // (d+1)^2 - d^2 = 2d + 1, so summing 2 * RangeSum(before the update) + rangeLength
    // keeps a running sum of squares exactly - no D[l] value ever needs reading back
    // out individually. Previous occurrences are tracked with this repo's own
    // HashMap<int,int>, the same role TwoSumSolution's HashMap plays.
    public static long SumOfSquaresByRangeFenwickTree(int[] nums)
    {
        var n = nums.Length;
        var tree = new RepoRangeFenwickTree(n);
        var lastSeen = new HashMap<int, int>();
        var sumOfSquares = 0L;
        var answer = 0L;

        for (var right = 0; right < n; right++)
        {
            var value = nums[right];
            var left = lastSeen.TryGetValue(value, out var previous) ? previous + 1 : 0;
            lastSeen.Set(value, right);

            var rangeSumBefore = tree.Query(left, right);
            var rangeLength = right - left + 1;
            sumOfSquares += (2 * rangeSumBefore) + rangeLength;

            tree.RangeAdd(left, right, 1);
            answer = (answer + sumOfSquares) % Mod;
        }

        return answer;
    }
}
