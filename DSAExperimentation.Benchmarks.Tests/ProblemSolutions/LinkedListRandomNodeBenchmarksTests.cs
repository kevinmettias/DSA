using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for LinkedListRandomNodeBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - reservoir sampling against a one-time DynamicArray cache - so a
// harness whose arms disagree is timing two different problems. Setup builds the 0..Length-1 chain
// from the workload fixture, so the same Length must rebuild the same chain; otherwise two published
// numbers were never comparable in the first place.
//
// The two arms are NOT comparable value for value, and no assertion here pretends otherwise: each
// draws from its own Random(1), and they advance that stream by a different amount per call (the
// reservoir arm once per node, the cached arm once per call), so equal sums would mean two different
// sampling procedures made the same draws - a property neither arm has. What both genuinely share is
// the distribution they exist to sample: a uniformly random value from the chain's 0..Length-1
// values, so each arm's mean over CallCount draws has to sit at the middle of that range. The
// tolerance is the sampling noise of that mean, not numeric slack, and the assertion is weaker than
// arm agreement: it witnesses that both arms sample the whole chain, not that they drew the same
// values. Reported as such with this batch.
public sealed partial class LinkedListRandomNodeBenchmarksTests
{
    private const int SmallestLength = 200;
    private const long CallCount = 2_000;

    // Middle of the 0..SmallestLength-1 value range the workload fixture builds.
    private const double Midpoint = (SmallestLength - 1) / 2.0;

    // Sampling noise of the mean of CallCount uniform draws from that range: the standard error is
    // about 1.3 at this call count, so this band is several times that and still far narrower than
    // any arm that sampled a narrow slice of the chain could reach.
    private const double RelativeTolerance = 0.05;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameChain() =>
        Assert.Equal(BuildHarness().ReservoirSampling(), BuildHarness().ReservoirSampling());

    [Fact]
    public void ReservoirSampling_TwoThousandDraws_IsCentredOnTheMiddleOfTheValueRange() =>
        Assert.InRange(
            Mean(BuildHarness().ReservoirSampling()),
            Midpoint - (RelativeTolerance * (SmallestLength - 1)),
            Midpoint + (RelativeTolerance * (SmallestLength - 1)));

    [Fact]
    public void DynamicArrayCache_TwoThousandDraws_IsCentredOnTheMiddleOfTheValueRange() =>
        Assert.InRange(
            Mean(BuildHarness().DynamicArrayCache()),
            Midpoint - (RelativeTolerance * (SmallestLength - 1)),
            Midpoint + (RelativeTolerance * (SmallestLength - 1)));

    private static double Mean(long sumOfValues) => sumOfValues / (double)CallCount;

    private static LinkedListRandomNodeBenchmarks BuildHarness()
    {
        var harness = new LinkedListRandomNodeBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
