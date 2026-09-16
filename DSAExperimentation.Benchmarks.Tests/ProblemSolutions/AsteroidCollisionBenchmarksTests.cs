using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for AsteroidCollisionBenchmarks (ARCHITECTURE 17.9): its two arms are AsteroidCollisionSolution's
// competing strategies for the same question - a repeated restart-from-the-beginning scan against one textbook
// stack pass - so a harness whose arms disagree has resolved two different fields. Beyond agreeing, the surviving
// field is asserted to be collision-free: nothing that still moves right may stand anywhere before something that
// still moves left, since that pair would have met. Setup draws magnitudes and directions from one seed, so the
// same Length must rebuild the same asteroids.
public sealed partial class AsteroidCollisionBenchmarksTests
{
    // The smaller of Setup's [Params(200, 3_000)] field sizes; a resolved field never grows,
    // so it also bounds the surviving length.
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().RepeatedScan()),
            AnswerText.Of(BuildHarness().RepeatedScan()));

    [Fact]
    public void RepeatedScan_TwoHundredAsteroidField_AgreesWithStackPass()
    {
        var harness = BuildHarness();
        var survivors = harness.RepeatedScan();

        Assert.InRange(survivors.Length, 0, SmallestLength);
        Assert.True(IsFullyResolved(survivors));
        Assert.Equal(AnswerText.Of(harness.StackPass()), AnswerText.Of(survivors));
    }

    [Fact]
    public void StackPass_TwoHundredAsteroidField_AgreesWithRepeatedScan()
    {
        var harness = BuildHarness();
        var survivors = harness.StackPass();

        Assert.InRange(survivors.Length, 0, SmallestLength);
        Assert.True(IsFullyResolved(survivors));
        Assert.Equal(AnswerText.Of(harness.RepeatedScan()), AnswerText.Of(survivors));
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
