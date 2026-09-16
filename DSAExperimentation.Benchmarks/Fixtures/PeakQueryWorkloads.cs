namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for the two Peaks In Array siblings - LC 3187 and LC
// 4017 - whose inputs differ in no way at all: the same values, the same query
// script. Nums stay inside 1..length-1 so peaks are neither everywhere nor nowhere,
// and queries alternate a type-1 peak count over the whole array with a type-2 point
// update to a fresh random value, so the O(n) (LC 3187) and O((r-l)^3) (LC 4017)
// rescans the brute-force arms pay per range query always have real work to do.
internal static class PeakQueryWorkloads
{
    public static (int[] Nums, int[][] Queries) Build(int length, int seed)
    {
        var random = new Random(seed);
        var nums = SeededDraws.Values(length, 1, length, random);
        var queries = new int[length][];

        for (var i = 0; i < length; i++)
        {
            var isRangeQuery = i % 2 == 0;
            queries[i] = isRangeQuery ? WholeRangeCount(length) : PointUpdate(length, random);
        }

        return (nums, queries);
    }

    // A type-1 query: count the peaks across the whole array.
    private static int[] WholeRangeCount(int length) => [1, 0, length - 1];

    // A type-2 query: overwrite a random index with a different random value.
    private static int[] PointUpdate(int length, Random random) =>
        [2, random.Next(length), random.Next(1, length)];
}
