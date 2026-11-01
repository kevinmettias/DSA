using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for AsteroidCollisionBenchmarks (ARCHITECTURE 17.9): the generic check holds the arms to each
// other, and this asserts what agreement alone cannot - that the surviving field is collision-free: nothing that
// still moves right may stand anywhere before something that still moves left, since that pair would have met.
public sealed partial class AsteroidCollisionBenchmarksTests
{
    // The smaller of Setup's [Params(200, 3_000)] field sizes; a resolved field never grows,
    // so it also bounds the surviving length.
    private const int SmallestLength = 200;

    [Fact]
    public void RepeatedScan_TwoHundredAsteroidField_LeavesAFullyResolvedField()
    {
        var survivors = BuildHarness().RepeatedScan();

        Assert.InRange(survivors.Length, 0, SmallestLength);
        Assert.True(IsFullyResolved(survivors));
    }

    private static AsteroidCollisionBenchmarks BuildHarness()
    {
        var harness = new AsteroidCollisionBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }

    // A right-moving asteroid still standing to the left of a left-moving one would have to
    // collide with it, so no such pair may survive in a finished field.
    private static bool IsFullyResolved(int[] survivors)
    {
        var hasSurvivingRightMover = false;

        foreach (var asteroid in survivors)
        {
            if (asteroid > 0)
            {
                hasSurvivingRightMover = true;
            }
            else if (hasSurvivingRightMover)
            {
                return false;
            }
        }

        return true;
    }
}
