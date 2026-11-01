using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountLatticePointsInsideACircleBenchmarks (ARCHITECTURE 17.9):
// BenchmarkArmsTests already holds the arms to agreeing on a fresh harness, so this asserts what it
// cannot: a lower bound on the answer that comes from the workload's shape rather than from either
// arm, and that a harness which has already answered once answers as a rebuild does, since
// BenchmarkDotNet calls an arm many times on one harness while the generic check builds a fresh
// harness for every call. Setup seeds the circles, so the same CircleCount must rebuild the same
// ones.
public sealed partial class CountLatticePointsInsideACircleBenchmarksTests
{
    private const int SmallestCircleCount = 50;

    [Fact]
    public void Setup_SmallestCircleCount_RebuildsTheSameWorkload()
    {
        var first = BuildHarness();
        var second = BuildHarness();

        // The documented shape: every circle's radius is drawn from [1, 6), so each one covers at
        // least its own centre and the union the answer counts can never be empty.
        Assert.InRange(first.FullGridScan(), 1, int.MaxValue);
        Assert.Equal(first.FullGridScan(), second.FullGridScan());
    }

    private static CountLatticePointsInsideACircleBenchmarks BuildHarness()
    {
        var harness = new CountLatticePointsInsideACircleBenchmarks { CircleCount = SmallestCircleCount };
        harness.Setup();

        return harness;
    }
}
