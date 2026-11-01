using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for HappyNumberBenchmarks (ARCHITECTURE 17.9): BenchmarkArmsTests already holds
// the arms to agreeing on the class's single input, so this asserts only the verdict that input
// fixes independently of either arm: the constant is a member of the canonical non-happy cycle, and
// a number on that cycle repeats a value before ever reaching 1, so the answer must be false.
public sealed partial class HappyNumberBenchmarksTests
{
    // A number on the canonical non-happy cycle (4 -> 16 -> ... -> 4) never reaches 1.
    private const bool ExpectedVerdict = false;

    [Fact]
    public void IsHappyByVisitedSet_NonHappyCycleMember_ReportsNotHappy() =>
        Assert.Equal(ExpectedVerdict, BuildHarness().IsHappyByVisitedSet());

    private static HappyNumberBenchmarks BuildHarness() => new();
}
