namespace DSAExperimentation.LeetCode.ThresholdMajorityQueries;

// LeetCode 3636. Threshold Majority Queries: for each [l, r, threshold] query,
// return the element of nums[l..r] with frequency >= threshold, preferring the
// highest frequency (ties broken to the smallest value), or -1 if none qualifies.
//
// The threshold never changes *which* element wins: the range's overall mode (its
// single highest-frequency element, tie-broken smallest) already has frequency at
// least as large as any other element's, so if the mode clears threshold it is
// necessarily the answer, and if it doesn't, no lower-frequency element can either.
// Both strategies therefore just compute the range mode and compare it to
// threshold - they differ only in how the mode query itself is answered.
internal static class ThresholdMajorityQueriesSolution
{
    // The textbook per-query scan: count nums[l..r] into a fresh frequency table
    // and track the running (highest freq, smallest value on tie) winner. O(range
    // length) per query - the block-index strategy below has to beat it.
    public static int[] SubarrayMajorityByBruteForce(int[] nums, int[][] queries)
    {
        var answers = new int[queries.Length];

        for (var q = 0; q < queries.Length; q++)
        {
            var (l, r, threshold) = (queries[q][0], queries[q][1], queries[q][2]);
            var freq = new Dictionary<int, int>();
            var best = (Value: 0, Freq: 0);

            for (var i = l; i <= r; i++)
            {
                freq.TryGetValue(nums[i], out var count);
                count++;
                freq[nums[i]] = count;

                if (count > best.Freq || (count == best.Freq && nums[i] < best.Value))
                {
                    best = (nums[i], count);
                }
            }

            answers[q] = best.Freq >= threshold ? best.Value : LeetCodeAnswer.None;
        }

        return answers;
    }

    // Sqrt decomposition: ThresholdMajorityBlockIndex's block-mode table plus each
    // value's sorted occurrence list turn every query into O(sqrt(n)) exactly-
    // verified candidates instead of a full range scan. Index construction is the
    // separable cost a benchmark charges to [GlobalSetup] - see the hoisted
    // overload below.
    public static int[] SubarrayMajorityByBlockMode(int[] nums, int[][] queries) =>
        SubarrayMajorityByBlockMode(ThresholdMajorityBlockIndex.Build(nums), queries);

    public static int[] SubarrayMajorityByBlockMode(ThresholdMajorityBlockIndex index, int[][] queries)
    {
        var answers = new int[queries.Length];

        for (var q = 0; q < queries.Length; q++)
        {
            answers[q] = index.Query(queries[q][0], queries[q][1], queries[q][2]);
        }

        return answers;
    }
}
