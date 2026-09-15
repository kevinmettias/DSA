using DSAExperimentation.LeetCode.NumberOfPairsAfterIncrement;

namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 3943 - nums1 stays at the problem's own
// length cap (5) since that bound is part of the puzzle itself, not a
// measurement choice; what this builds is how large nums2 and the query stream
// are, and the query mix (roughly one range-add per count query, alternating)
// both NumberOfPairsAfterIncrementSolution strategies replay identically.
internal static class NumberOfPairsAfterIncrementWorkloads
{
    private const int Nums1Length = 5;
    private const int ValueExclusiveBound = 100_000;

    public static int[] BuildNums1(int seed)
    {
        var random = new Random(seed);

        return [.. Enumerable.Range(0, Nums1Length).Select(_ => random.Next(1, ValueExclusiveBound))];
    }

    public static int[] BuildNums2(int length, int seed)
    {
        var random = new Random(seed);

        return [.. Enumerable.Range(0, length).Select(_ => random.Next(1, ValueExclusiveBound))];
    }

    public static PairQuery[] BuildQueries(int count, int nums2Length, int seed)
    {
        var random = new Random(seed);
        var queries = new PairQuery[count];

        for (var i = 0; i < count; i++)
        {
            if (i % 2 == 0)
            {
                var left = random.Next(nums2Length);
                var right = random.Next(left, nums2Length);
                var delta = random.Next(1, ValueExclusiveBound);
                queries[i] = PairQuery.Increment(left, right, delta);
            }
            else
            {
                var total = random.Next(2, 2 * ValueExclusiveBound);
                queries[i] = PairQuery.Count(total);
            }
        }

        return queries;
    }
}
