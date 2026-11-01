using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for LinkedListCycleBenchmarks (ARCHITECTURE 17.9): BenchmarkArmsTests already
// holds the arms to agreeing on the smallest workload, so this asserts only the verdict the fixture
// fixes independently of either arm. Setup builds a Length-node chain whose tail rejoins its head,
// so every arm must walk the full cycle and answer true. Agreement alone would not catch both arms
// returning false for the same wrong reason - only the pinned verdict does that.
public sealed partial class LinkedListCycleBenchmarksTests
{
    private const int SmallestLength = 200;
    private const bool ExpectedCycleDetected = true;

    [Fact]
    public void HasCycleByVisitedSet_TailRejoinsHead_DetectsTheCycle() =>
        Assert.Equal(ExpectedCycleDetected, BuildHarness().HasCycleByVisitedSet());

    private static LinkedListCycleBenchmarks BuildHarness()
    {
        var harness = new LinkedListCycleBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
