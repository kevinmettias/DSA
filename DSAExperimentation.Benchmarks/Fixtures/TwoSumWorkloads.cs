namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 1 - length values whose only pair summing to Target is the
// last two. Every other value is drawn from [1, 999], and the last two are planted at 1,000 and
// 1,001, which no drawn value can complete: a drawn pair sums to at most 1,998, and a drawn value
// would have to be 1,000 or 1,001 itself to meet either planted one. So exactly one answer
// exists, as LC 1 promises, and it is the last pair either strategy reaches. The last two draws
// are still made, so the planted values move no other draw.
internal static class TwoSumWorkloads
{
    public const int Target = PlantedLow + PlantedHigh;

    private const int DrawnValueUpperBoundExclusive = 1_000;
    private const int PlantedLow = 1_000;
    private const int PlantedHigh = 1_001;

    public static int[] BuildValues(int length, int seed)
    {
        var values = SeededDraws.Values(length, 1, DrawnValueUpperBoundExclusive, new Random(seed));
        values[^2] = PlantedLow;
        values[^1] = PlantedHigh;

        return values;
    }
}
