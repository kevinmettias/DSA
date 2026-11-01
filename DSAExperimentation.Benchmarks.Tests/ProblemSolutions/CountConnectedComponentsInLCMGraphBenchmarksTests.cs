using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountConnectedComponentsInLCMGraphBenchmarks (ARCHITECTURE 17.9):
// BenchmarkArmsTests already holds the arms to agreeing on a rebuilt workload, so this asserts only
// the workload's documented shape: Setup's values really do connect, and a component count below the
// value count is what makes either arm do join work instead of reporting n isolated nodes.
public sealed partial class CountConnectedComponentsInLCMGraphBenchmarksTests
{
    private const int SmallestLength = 50;

    [Fact]
    public void MultipleUnion_FiftyDistinctValues_JoinsSomeValues() =>
        Assert.True(BuildHarness().MultipleUnion() < SmallestLength);

    private static CountConnectedComponentsInLCMGraphBenchmarks BuildHarness()
    {
        var harness = new CountConnectedComponentsInLCMGraphBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
