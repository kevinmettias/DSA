using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ImplementRand10UsingRand7Benchmarks (ARCHITECTURE 17.9). Both arms are
// uniform Rand10() strategies, but they consume the seeded Rand7 stream differently - the recycled
// sampler draws fewer times per value - so they return different draws and are not asserted to
// agree (ArmAgreement lists the class as a random draw). What each arm owes instead is LeetCode's
// contract over the draws one call returns: one value per call, every value in 1..10, and every
// value about a tenth of the time. Over the smallest Calls, 1,000, a value is expected 100 times
// with a standard deviation of sqrt(1000 * 0.1 * 0.9), about 9.5, so the band of 50..150 is over
// five of those either side, and a strategy that never reached some value, or reached one twice as
// often as it should, falls outside it. The stream is seeded, so the counts are fixed from run to
// run either way.
public sealed partial class ImplementRand10UsingRand7BenchmarksTests
{
    private const int SmallestCalls = 1_000;

    private const int MinimumRand10 = 1;
    private const int Rand10Maximum = 10;

    private const int FewestDrawsOfAValue = 50;
    private const int MostDrawsOfAValue = 150;

    [Fact]
    public void Setup_SameCalls_RebuildsTheSameDrawStream()
    {
        Assert.Equal(BuildHarness().RejectionSampling(), BuildHarness().RejectionSampling());
        Assert.Equal(BuildHarness().RecycledRejectionSampling(), BuildHarness().RecycledRejectionSampling());
    }

    [Fact]
    public void RejectionSampling_SeededStream_SpreadsEvenlyOverOneToTen() =>
        AssertSpreadEvenlyOverOneToTen(BuildHarness().RejectionSampling());

    [Fact]
    public void RecycledRejectionSampling_SeededStream_SpreadsEvenlyOverOneToTen() =>
        AssertSpreadEvenlyOverOneToTen(BuildHarness().RecycledRejectionSampling());

    private static void AssertSpreadEvenlyOverOneToTen(int[] draws)
    {
        Assert.Equal(SmallestCalls, draws.Length);
        Assert.All(draws, draw => Assert.InRange(draw, MinimumRand10, Rand10Maximum));

        for (var value = MinimumRand10; value <= Rand10Maximum; value++)
        {
            var drawsOfValue = draws.Count(draw => draw == value);
            Assert.InRange(drawsOfValue, FewestDrawsOfAValue, MostDrawsOfAValue);
        }
    }

    private static ImplementRand10UsingRand7Benchmarks BuildHarness()
    {
        var harness = new ImplementRand10UsingRand7Benchmarks { Calls = SmallestCalls };
        harness.Setup();

        return harness;
    }
}
