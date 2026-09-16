using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for NumberOfPeopleAwareOfASecretBenchmarks (ARCHITECTURE 17.9): both arms return the
// same number of people who know the secret by day Length - the sliding-window baseline against the
// Fenwick range-sum pass - so a harness whose arms disagree is timing two different questions. The
// count modulo 1e9+7 is the problem's whole answer rather than a proxy. Both arms take (n, delay,
// forget) as three integers, so Setup's only job is to pick a delay/forget pair from Length, and the
// same Length must pick the same pair.
public sealed partial class NumberOfPeopleAwareOfASecretBenchmarksTests
{
    private const int SmallestLength = 2_000;

    [Fact]
    public void Setup_SameParameters_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().SlidingWindowSum(), BuildHarness().SlidingWindowSum());

    [Fact]
    public void SlidingWindowSum_AgreesWithFenwickRangeSum()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.FenwickRangeSum(), harness.SlidingWindowSum());
    }

    [Fact]
    public void FenwickRangeSum_AgreesWithSlidingWindowSum()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.SlidingWindowSum(), harness.FenwickRangeSum());
    }

    private static NumberOfPeopleAwareOfASecretBenchmarks BuildHarness()
    {
        var harness = new NumberOfPeopleAwareOfASecretBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
