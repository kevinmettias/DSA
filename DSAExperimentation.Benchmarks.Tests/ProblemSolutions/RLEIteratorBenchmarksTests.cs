using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for RLEIteratorBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - the sequence of values a fixed next(elementCount) script
// drains out of one run-length encoding - so a harness whose arms disagree is timing two different
// problems. Both arms return a scalar: the last value the script saw, which the drain loop sums
// nothing into, so the comparison is direct. Setup builds the encoding and the query script from
// one fixed seed, so the same TotalCount must rebuild the same pair.
public sealed partial class RLEIteratorBenchmarksTests
{
    private const int SmallestTotalCount = 2_000;

    [Fact]
    public void Setup_SameTotalCount_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().DecompressedArrayCursor(), BuildHarness().DecompressedArrayCursor());

    [Fact]
    public void DecompressedArrayCursor_SeededEncodingAndQueryStream_AgreesWithRunLengthQueue()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.DecompressedArrayCursor(), harness.RunLengthQueue());
    }

    [Fact]
    public void RunLengthQueue_SeededEncodingAndQueryStream_AgreesWithTheComposedArm()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.RunLengthQueue(), harness.DecompressedArrayCursor());
    }

    private static RLEIteratorBenchmarks BuildHarness()
    {
        var harness = new RLEIteratorBenchmarks { TotalCount = SmallestTotalCount };
        harness.Setup();

        return harness;
    }
}
