using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for RandomPickWithBlacklistBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - a uniformly random int in [0, RangeSize) that is
// never blacklisted - so a harness whose arms disagree is timing two different problems.
//
// Here the arms are seeded (both factories take Seed), so Setup's determinism IS checkable through
// them. The arms themselves are still not comparable value for value, and no assertion pretends
// otherwise: rejection sampling redraws from [0, RangeSize) until it escapes the blacklist, while
// the remap arm draws once from [0, whitelistBound), so the two consume their identical seeded
// streams completely differently and equal totals would mean two different sampling procedures
// made the same draws. What both genuinely share is the whitelist they sample - [RangeSize -
// WhitelistSize, RangeSize) - so each arm's mean over PickCalls draws has to sit at the middle of
// that interval. The tolerance is the sampling noise of that mean, and the assertion is weaker
// than arm agreement: it witnesses that both arms return whitelisted values, not that they drew
// the same ones. Reported as such.
public sealed partial class RandomPickWithBlacklistBenchmarksTests
{
    private const int SmallestRangeSize = 2_000;

    // Both values are restated from the benchmark's own constants: it blacklists everything below
    // RangeSize - WhitelistSize, so exactly these values are ever left to be picked.
    private const int WhitelistSize = 10;
    private const int PickCalls = 200;

    private const int SmallestWhitelistValue = SmallestRangeSize - WhitelistSize;
    private const int LargestWhitelistValue = SmallestRangeSize - 1;
    private const double ExpectedWhitelistMean = (SmallestWhitelistValue + LargestWhitelistValue) / 2.0;

    // Sampling noise of the mean of PickCalls uniform draws over the whitelist, not numeric slack:
    // its standard error is about 0.2 there, far inside this band and far narrower than an arm
    // centred on the blacklisted region could reach.
    private const double RelativeTolerance = 0.002;

    [Fact]
    public void Setup_SameRangeSize_RebuildsTheSameWorkload()
    {
        Assert.Equal(BuildHarness().SetHashMapRemap(), BuildHarness().SetHashMapRemap());
        Assert.Equal(BuildHarness().RejectionSampling(), BuildHarness().RejectionSampling());
    }

    [Fact]
    public void RejectionSampling_SmallestRangeSize_DrawsFromTheWhitelistOnly() =>
        Assert.InRange(
            Mean(BuildHarness().RejectionSampling()),
            ExpectedWhitelistMean * (1 - RelativeTolerance),
            ExpectedWhitelistMean * (1 + RelativeTolerance));

    [Fact]
    public void SetHashMapRemap_SmallestRangeSize_DrawsFromTheWhitelistOnly() =>
        Assert.InRange(
            Mean(BuildHarness().SetHashMapRemap()),
            ExpectedWhitelistMean * (1 - RelativeTolerance),
            ExpectedWhitelistMean * (1 + RelativeTolerance));

    private static double Mean(long sumOfPicks) => sumOfPicks / (double)PickCalls;

    private static RandomPickWithBlacklistBenchmarks BuildHarness()
    {
        var harness = new RandomPickWithBlacklistBenchmarks { RangeSize = SmallestRangeSize };
        harness.Setup();

        return harness;
    }
}
