using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.LeetCode.NumberOfSubarraysWithANDValueOfK;

// LeetCode 3209. Number of Subarrays With AND Value of K: count subarrays whose
// bitwise AND of every element equals k.
internal static class NumberOfSubarraysWithANDValueOfKSolution
{
    // The textbook O(n^2) scan: extend every start index one element at a time,
    // AND-ing as it grows, and count every extension that lands on k. Correct,
    // and the arm the AND-value-compression strategy below has to beat.
    public static long CountByBruteForce(int[] nums, int targetValue)
    {
        var count = 0L;

        for (var start = 0; start < nums.Length; start++)
        {
            count += CountFrom(nums, start, targetValue);
        }

        return count;
    }

    // Every subarray that begins at `start`, AND-ed as it grows; each extension
    // that lands on targetValue counts.
    private static long CountFrom(int[] nums, int start, int targetValue)
    {
        var count = 0L;
        var andValue = nums[start];

        if (andValue == targetValue)
        {
            count++;
        }

        for (var end = start + 1; end < nums.Length; end++)
        {
            andValue &= nums[end];

            if (andValue == targetValue)
            {
                count++;
            }
        }

        return count;
    }

    // Bitwise AND only ever turns bits off as a window grows, so the subarrays
    // ending at any one index carry at most ~30 distinct AND values (one per bit
    // that can still be cleared) no matter how long the array is. This repo's
    // own HashMap<int, long> collapses same-valued subarrays ending at the
    // current index into one (value, count) entry, so each step does O(30) work
    // instead of rescanning every earlier start.
    public static long CountByAndValueCompression(int[] nums, int targetValue)
    {
        var count = 0L;
        var endingHere = new List<(int Value, long Count)>();

        foreach (var value in nums)
        {
            var next = new HashMap<int, long>();
            Accumulate(next, value, 1);

            foreach (var (priorValue, priorCount) in endingHere)
            {
                Accumulate(next, priorValue & value, priorCount);
            }

            endingHere = ToPairs(next);
            count += next.TryGetValue(targetValue, out var matches) ? matches : 0;
        }

        return count;
    }

    private static void Accumulate(HashMap<int, long> andValueCounts, int andValue, long delta)
    {
        var existing = andValueCounts.TryGetValue(andValue, out var priorCount) ? priorCount : 0;
        andValueCounts.Set(andValue, existing + delta);
    }

    private static List<(int Value, long Count)> ToPairs(HashMap<int, long> andValueCounts)
    {
        var pairs = new List<(int, long)>(andValueCounts.Count);

        foreach (var value in andValueCounts.Keys)
        {
            andValueCounts.TryGetValue(value, out var count);
            pairs.Add((value, count));
        }

        return pairs;
    }
}
