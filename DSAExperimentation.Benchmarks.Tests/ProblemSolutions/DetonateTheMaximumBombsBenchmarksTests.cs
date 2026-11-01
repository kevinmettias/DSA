using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DetonateTheMaximumBombsBenchmarks (ARCHITECTURE 17.9): a bound the
// workload's shape fixes without either arm. Every bomb is generated with a radius of at least one,
// so a triggered bomb always detonates itself and the answer lies in [1, BombCount].
public sealed partial class DetonateTheMaximumBombsBenchmarksTests
{
    private const int SmallestBombCount = 50;
    private const int MinimumDetonationsForARadiusBearingBomb = 1;

    [Fact]
    public void ManualRecursiveVisit_RadiusBearingBombs_DetonatesBetweenOneAndEveryBomb() =>
        Assert.InRange(
            BuildHarness().ManualRecursiveVisit(),
            MinimumDetonationsForARadiusBearingBomb,
            SmallestBombCount);

    private static DetonateTheMaximumBombsBenchmarks BuildHarness()
    {
        var harness = new DetonateTheMaximumBombsBenchmarks { BombCount = SmallestBombCount };
        harness.Setup();

        return harness;
    }
}
