using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CarFleetBenchmarks (ARCHITECTURE 17.9): its two arms are competing strategies
// for the same question - recomputing the running maximum arrival time for every car against one
// monotonic stack sweep - so a harness whose arms disagree is timing two different problems. Both
// arms take the same prepared arrival-time sequence, so the comparison also pins that the two
// strategies were handed the same cars in the same position-descending order; building that sequence
// is Setup's job, and Setup draws it from one fixed seed, so the same Length must rebuild it.
public sealed partial class CarFleetBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().RecomputeMaxEachCar(), BuildHarness().RecomputeMaxEachCar());

    [Fact]
    public void RecomputeMaxEachCar_SeededPositionsAndSpeeds_AgreesWithMonotonicStackSweep()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MonotonicStackSweep(), harness.RecomputeMaxEachCar());
    }

    [Fact]
    public void MonotonicStackSweep_SeededPositionsAndSpeeds_AgreesWithRecomputeMaxEachCar()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.RecomputeMaxEachCar(), harness.MonotonicStackSweep());
    }

    private static CarFleetBenchmarks BuildHarness()
    {
        var harness = new CarFleetBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
