using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for TwoSumWorkloads (ARCHITECTURE 17.7). The reading depends on the guarantee the
// planted pair exists for: LC 1 promises exactly one answer, so exactly one pair of positions may sum
// to the target, and it has to be the last two, which is what makes both strategies scan to the end.
// Every pair is counted here by value frequency, without either strategy, at both of
// TwoSumBenchmarks' lengths and its own seed.
public sealed partial class TwoSumWorkloadsTests
{
    // Mirrors TwoSumBenchmarks' own private Seed and its two lengths.
    private const int Seed = 1;
    private const int SmallestLength = 200;
    private const int LargestLength = 5_000;

    private const int MaxMagnitude = 1_000_000_000;
    private const int ExpectedPairCount = 1;

    // The planted pair is the array's last two values.
    private const int PlantedPairLength = 2;

    public static TheoryData<int> Lengths => new([SmallestLength, LargestLength]);

    [Theory]
    [MemberData(nameof(Lengths))]
    public void BuildValues_BenchmarkLengths_HoldExactlyOnePairSummingToTarget(int length) =>
        Assert.Equal(ExpectedPairCount, PairsSummingToTarget(TwoSumWorkloads.BuildValues(length, Seed)));

    [Theory]
    [MemberData(nameof(Lengths))]
    public void BuildValues_LastTwoValues_SumToTarget(int length)
    {
        var values = TwoSumWorkloads.BuildValues(length, Seed);

        Assert.Equal(TwoSumWorkloads.Target, values.TakeLast(PlantedPairLength).Sum());
        Assert.All(values, value => Assert.InRange(value, -MaxMagnitude, MaxMagnitude));
    }

    // Walks the values once, counting for each one how many earlier values complete it to the target.
    private static int PairsSummingToTarget(int[] values)
    {
        var seen = new Dictionary<int, int>();
        var pairs = 0;

        foreach (var value in values)
        {
            pairs += seen.GetValueOrDefault(TwoSumWorkloads.Target - value);
            seen[value] = seen.GetValueOrDefault(value) + 1;
        }

        return pairs;
    }
}
