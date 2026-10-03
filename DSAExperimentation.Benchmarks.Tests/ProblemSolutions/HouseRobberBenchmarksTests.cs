using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for HouseRobberBenchmarks (ARCHITECTURE 17.9). Its two arms are competing
// strategies for the same question - the memoized recurrence and the rolling-totals pass - so a
// harness whose arms disagree is timing two different problems: both must report the same haul.
// Setup's street is fully documented (seeded Random(198), each house drawn from [0, 400]), so the
// expected haul is derived here from a rebuilt street by trying every set of houses at the smallest
// size of ten - all 1,024 of them - and keeping the best one with no two neighbours, a brute force
// that shares nothing with either arm's recurrence.
public sealed partial class HouseRobberBenchmarksTests
{
    private const int SmallestLength = 10;
    private const int RandomSeed = 198;
    private const int MaxValue = 400;

    [Fact]
    public void MemoizedRecursion_SeededStreet_ReturnsTheBestNonAdjacentHaul() =>
        Assert.Equal(BruteForceBestHaul(RebuildStreet()), BuildHarness().MemoizedRecursion());

    [Fact]
    public void IterativeRollingTotals_SeededStreet_ReturnsTheBestNonAdjacentHaul() =>
        Assert.Equal(BruteForceBestHaul(RebuildStreet()), BuildHarness().IterativeRollingTotals());

    [Fact]
    public void IterativeRollingTotals_AgreesWithMemoizedRecursion()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MemoizedRecursion(), harness.IterativeRollingTotals());
    }

    private static HouseRobberBenchmarks BuildHarness()
    {
        var harness = new HouseRobberBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }

    // The benchmark's own draw, rebuilt from its documented shape.
    private static int[] RebuildStreet() => SeededDraws.Values(SmallestLength, 0, MaxValue + 1, new Random(RandomSeed));

    // Bit i of a choice robs house i, and a choice robs two neighbours exactly when it shares a bit
    // with itself shifted by one.
    private static int BruteForceBestHaul(int[] street) =>
        Enumerable.Range(0, 1 << street.Length)
            .Where(choice => (choice & (choice >> 1)) == 0)
            .Max(choice => Enumerable.Range(0, street.Length).Where(house => ((choice >> house) & 1) == 1).Sum(house => street[house]));
}
