using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MaximizeTheNumberOfPartitionsAfterOperationsBenchmarks (ARCHITECTURE 17.9):
// both arms are competing strategies for one question - the most partitions reachable after
// recoloring at most one character - so a harness whose arms disagree is timing two different
// problems. Setup draws the text from a fixed seed, so the same length must rebuild the same text;
// neither arm mutates it.
public sealed partial class MaximizeTheNumberOfPartitionsAfterOperationsBenchmarksTests
{
    private const int SmallestLength = 100;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().BruteForceRecolor(), BuildHarness().BruteForceRecolor());

    [Fact]
    public void BruteForceRecolor_AgreesWithBitmaskMemo()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForceRecolor(), harness.BitmaskMemo());
    }

    [Fact]
    public void BitmaskMemo_AgreesWithBruteForceRecolor()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BitmaskMemo(), harness.BruteForceRecolor());
    }

    private static MaximizeTheNumberOfPartitionsAfterOperationsBenchmarks BuildHarness()
    {
        var harness = new MaximizeTheNumberOfPartitionsAfterOperationsBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
