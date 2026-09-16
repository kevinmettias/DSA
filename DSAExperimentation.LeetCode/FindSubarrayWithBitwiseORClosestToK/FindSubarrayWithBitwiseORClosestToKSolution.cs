namespace DSAExperimentation.LeetCode.FindSubarrayWithBitwiseORClosestToK;

// LeetCode 3171. Find Subarray With Bitwise OR Closest to K: over every
// subarray of nums, minimize |OR(subarray) - targetOr|.
//
// Both strategies answer the same question with the same signature, so the
// test harness can assert they agree and the benchmark harness can time them
// against each other without either restating the algorithm.
internal static class FindSubarrayWithBitwiseORClosestToKSolution
{
    // The textbook answer: for every starting index, extend the window right
    // one element at a time, folding it into a running OR. O(n^2), deliberately
    // BCL-only - the arm the OR-compression sweep below has to justify itself
    // against.
    public static int MinimumDifferenceByBruteForce(int[] nums, int targetOr)
    {
        var best = int.MaxValue;

        for (var start = 0; start < nums.Length; start++)
        {
            var runningOr = 0;

            for (var end = start; end < nums.Length; end++)
            {
                runningOr |= nums[end];
                best = Math.Min(best, Math.Abs(runningOr - targetOr));
            }
        }

        return best;
    }

    // Composed: for each right endpoint, the OR values of every subarray
    // ending there - one per starting index - form a chain ordered by
    // decreasing start index, each entry's bit set a superset of the last
    // (widening the window can only set more bits). A submask is always
    // numerically <= its superset, so that chain is non-decreasing, and it
    // changes value at most once per bit of nums[i] (~30 times for LeetCode's
    // 1e9 bound) - so keeping only the distinct values bounds the list to
    // O(log(max value)) regardless of n. Folding the previous right endpoint's
    // list through `| nums[i]` preserves that same superset ordering, which is
    // what lets a single adjacent-duplicate check do the deduplication. O(n
    // log(max value)) overall, the same list-of-distinct-ORs idiom LC 898 uses.
    public static int MinimumDifferenceByOrCompression(int[] nums, int targetOr)
    {
        var best = int.MaxValue;
        var endingHere = new List<int>();

        foreach (var num in nums)
        {
            endingHere = ExtendWithOrs(endingHere, num);
            best = MinDifference(best, endingHere, targetOr);
        }

        return best;
    }

    // Folds `num` into every OR value of the subarrays ending at the previous right
    // endpoint. Folding preserves that list's superset ordering, so a single
    // adjacent-duplicate check is enough to deduplicate the result.
    private static List<int> ExtendWithOrs(List<int> endingHere, int num)
    {
        var next = new List<int> { num };

        foreach (var previousOr in endingHere)
        {
            var combined = previousOr | num;

            if (next[^1] != combined)
            {
                next.Add(combined);
            }
        }

        return next;
    }

    // The best |OR value - targetOr| reachable through the subarrays ending here.
    private static int MinDifference(int best, List<int> endingHere, int targetOr)
    {
        foreach (var orValue in endingHere)
        {
            best = Math.Min(best, Math.Abs(orValue - targetOr));
        }

        return best;
    }
}
