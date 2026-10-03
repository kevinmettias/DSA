using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for HouseRobberIIBenchmarks (ARCHITECTURE 17.9). Its two arms are competing
// strategies for the same question - the memoized circular split and the iterative two-pass split -
// so a harness whose arms disagree is timing two different problems: both must report the same
// haul. Setup's circle is fully documented (seeded Random(213), each house drawn from [0, 1000]), so
// the expected haul is derived here from a rebuilt circle by trying every set of houses at the
// smallest size of ten - all 1,024 of them - and keeping the best one with no two neighbours, the
// first and last house counting as neighbours too. That brute force shares nothing with either arm's
// split of the circle into two lines.
public sealed partial class HouseRobberIIBenchmarksTests
{
    private const int SmallestLength = 10;
    private const int RandomSeed = 213;
    private const int MaxValue = 1_000;

    [Fact]
    public void MemoizedRecursion_SeededCircle_ReturnsTheBestNonAdjacentHaul() =>
        Assert.Equal(BruteForceBestHaul(RebuildCircle()), BuildHarness().MemoizedRecursion());

    [Fact]
    public void IterativeTwoPass_SeededCircle_ReturnsTheBestNonAdjacentHaul() =>
        Assert.Equal(BruteForceBestHaul(RebuildCircle()), BuildHarness().IterativeTwoPass());

    [Fact]
    public void IterativeTwoPass_AgreesWithMemoizedRecursion()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MemoizedRecursion(), harness.IterativeTwoPass());
    }

    private static HouseRobberIIBenchmarks BuildHarness()
    {
        var harness = new HouseRobberIIBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }

    // The benchmark's own draw, rebuilt from its documented shape.
    private static int[] RebuildCircle() => SeededDraws.Values(SmallestLength, 0, MaxValue + 1, new Random(RandomSeed));

    // Bit i of a choice robs house i. A choice robs two neighbours when it shares a bit with itself
    // shifted by one, or when it holds both the first bit and the last, which close the circle.
    private static int BruteForceBestHaul(int[] circle)
    {
        var lastHouse = 1 << (circle.Length - 1);

        return Enumerable.Range(0, 1 << circle.Length)
            .Where(choice => (choice & (choice >> 1)) == 0 && !((choice & 1) == 1 && (choice & lastHouse) != 0))
            .Max(choice => Enumerable.Range(0, circle.Length).Where(house => ((choice >> house) & 1) == 1).Sum(house => circle[house]));
    }
}
