using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DeliveringBoxesFromStorageToPortsBenchmarks (ARCHITECTURE 17.9): the generic arm
// check compares the arms only with each other, so what stays here is a bound derived without either
// arm. Every trip carries at most MaxBoxes, so the reading's documented floor is one trip per full load.
public sealed partial class DeliveringBoxesFromStorageToPortsBenchmarksTests
{
    private const int SmallestLength = 2_000;

    // [GlobalSetup] fixes the per-trip box limit the floor is derived from.
    private const int MaxBoxes = 50;

    private const int MinimumTripCount = ((SmallestLength - 1) / MaxBoxes) + 1;

    [Fact]
    public void RescanWindowEachPosition_TwoThousandSeededBoxes_NeedsAtLeastOneTripPerFullLoad() =>
        Assert.InRange(BuildHarness().RescanWindowEachPosition(), MinimumTripCount, SmallestLength);

    private static DeliveringBoxesFromStorageToPortsBenchmarks BuildHarness()
    {
        var harness = new DeliveringBoxesFromStorageToPortsBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
