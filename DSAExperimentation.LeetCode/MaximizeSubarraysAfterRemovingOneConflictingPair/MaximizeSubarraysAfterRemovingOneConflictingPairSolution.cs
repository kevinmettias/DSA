using DSAExperimentation.DataStructures.DynamicArray;

namespace DSAExperimentation.LeetCode.MaximizeSubarraysAfterRemovingOneConflictingPair;

// LeetCode 3480. Maximize Subarrays After Removing One Conflicting Pair: nums is
// implicitly [1..n]; conflictingPairs bans any subarray containing both endpoints
// of a pair that survives removal. Remove exactly one pair, maximize the count of
// still-valid non-empty subarrays.
internal static class MaximizeSubarraysAfterRemovingOneConflictingPairSolution
{
    // The textbook answer: try removing each pair in turn and, for that choice,
    // check literally every subarray against literally every surviving pair -
    // O(n^2 * m^2) overall. Deliberately exhaustive rather than clever, the arm the
    // bucketed sweep below has to justify itself against.
    public static int MaxSubarraysByBruteForce(int n, int[][] conflictingPairs)
    {
        var best = 0;

        for (var removedIndex = 0; removedIndex < conflictingPairs.Length; removedIndex++)
        {
            best = Math.Max(best, CountValidSubarrays(n, conflictingPairs, removedIndex));
        }

        return best;
    }

    private static int CountValidSubarrays(int n, int[][] pairs, int removedIndex)
    {
        var count = 0;

        for (var left = 1; left <= n; left++)
        {
            for (var right = left; right <= n; right++)
            {
                if (IsValidSubarray(pairs, removedIndex, left, right))
                {
                    count++;
                }
            }
        }

        return count;
    }

    private static bool IsValidSubarray(int[][] pairs, int removedIndex, int left, int right)
    {
        for (var i = 0; i < pairs.Length; i++)
        {
            if (i == removedIndex)
            {
                continue;
            }

            var a = pairs[i][0];
            var b = pairs[i][1];

            if (a >= left && a <= right && b >= left && b <= right)
            {
                return false;
            }
        }

        return true;
    }

    // One O(n + m) sweep: bucket every pair by its larger endpoint into this repo's
    // own DynamicArray<int>, then walk right = 1..n tracking the two largest
    // smaller-endpoints seen so far (maxLeft, secondMaxLeft, cumulative across the
    // whole sweep since a pair bucketed at some right stays in force for every
    // later right too). A subarray ending at right is valid once its start exceeds
    // maxLeft, so validSubarrays += right - maxLeft; if the single pair currently
    // driving maxLeft were removed, that same right would only cost secondMaxLeft,
    // so the per-right gain (maxLeft - secondMaxLeft) accumulates into
    // gainByLeftBound[maxLeft] - indexed by the bound VALUE rather than a pair
    // identity, which is exactly right: once a second pair shares that same left
    // bound, removing either one stops helping and the gain correctly stops
    // growing (maxLeft == secondMaxLeft from then on). The answer is the
    // unmodified count plus the single largest accumulated gain.
    public static int MaxSubarraysByGroupedBoundSweep(int n, int[][] conflictingPairs)
    {
        var conflictsByRightEndpoint = new DynamicArray<int>[n + 1];

        for (var right = 1; right <= n; right++)
        {
            conflictsByRightEndpoint[right] = new DynamicArray<int>();
        }

        foreach (var pair in conflictingPairs)
        {
            var rightEndpoint = Math.Max(pair[0], pair[1]);
            var leftEndpoint = Math.Min(pair[0], pair[1]);
            conflictsByRightEndpoint[rightEndpoint].Add(leftEndpoint);
        }

        var validSubarrays = 0;
        var maxLeft = 0;
        var secondMaxLeft = 0;
        var gainByLeftBound = new int[n + 1];

        for (var right = 1; right <= n; right++)
        {
            var bucket = conflictsByRightEndpoint[right];

            for (var i = 0; i < bucket.Count; i++)
            {
                var left = bucket.Get(i);

                if (left > maxLeft)
                {
                    secondMaxLeft = maxLeft;
                    maxLeft = left;
                }
                else if (left > secondMaxLeft)
                {
                    secondMaxLeft = left;
                }
            }

            validSubarrays += right - maxLeft;
            gainByLeftBound[maxLeft] += maxLeft - secondMaxLeft;
        }

        var bestGain = 0;

        foreach (var gain in gainByLeftBound)
        {
            bestGain = Math.Max(bestGain, gain);
        }

        return validSubarrays + bestGain;
    }
}
