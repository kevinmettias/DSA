namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for the two "pick sides, maximize perimeter"
// problems, LC 2971 and LC 976, whose inputs are the same thing: a bag of
// random side lengths. Each caller picks its own sizes off its own baseline's
// cost - LC 2971 stays tiny (16/20) because its brute-force arm enumerates
// subsets, the same small-n convention CountTheNumberOfGoodPartitionsBenchmarks
// uses for its own exponential baseline, while LC 976's merely-cubic
// every-triple arm affords 80/300 - and both still exercise the sorted arm's
// O(n log n) path on the same input.
internal static class PolygonWorkloads
{
    public static int[] BuildSides(int count, int seed)
    {
        var random = new Random(seed);
        var sides = new int[count];

        for (var i = 0; i < count; i++)
        {
            sides[i] = random.Next(1, 1000);
        }

        return sides;
    }
}
