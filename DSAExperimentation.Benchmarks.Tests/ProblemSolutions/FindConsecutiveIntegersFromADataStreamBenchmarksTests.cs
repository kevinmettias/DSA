using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FindConsecutiveIntegersFromADataStreamBenchmarks (ARCHITECTURE 17.9): the
// generic arms check only compares the arms with each other, so this pins the baseline's count to
// what Setup's construction fixes independently of any arm. Both arms return only a count of the
// true answers. Setup draws every arrival from [1, 1_000) and the window is 20 long with a single
// value that would fill it, so the documented "Value is rare in the stream" workload answers false
// throughout.
public sealed partial class FindConsecutiveIntegersFromADataStreamBenchmarksTests
{
    private const int SmallestLength = 2_000;

    // Twenty consecutive arrivals of the single tracked value would be needed to fill the window;
    // with every arrival drawn from [1, 1_000) that never happens over the seeded stream.
    private const int ExpectedConsecutiveRunCount = 0;

    [Fact]
    public void UnboundedHistoryRescan_SeededStreamOfRareValue_NeverFillsTheWindow() =>
        Assert.Equal(ExpectedConsecutiveRunCount, BuildHarness().UnboundedHistoryRescan());

    private static FindConsecutiveIntegersFromADataStreamBenchmarks BuildHarness()
    {
        var harness = new FindConsecutiveIntegersFromADataStreamBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
