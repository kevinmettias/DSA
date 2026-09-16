using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for LongestChunkedPalindromeDecompositionBenchmarks (ARCHITECTURE 17.9): its
// two arms are competing strategies for the same question - growing a pending window by repeated
// string concatenation against rolling-hash chunking - so a harness whose arms disagree is timing
// two different problems. Both arms return the chunk count, a scalar compared directly. Setup
// builds a text with all-distinct characters, so no two candidate chunks can ever be equal and
// every candidate length has to be grown all the way before it is rejected: that shape must be
// rebuilt by the same Length.
public sealed partial class LongestChunkedPalindromeDecompositionBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SmallestLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            BuildHarness().RollingHashChunking(),
            BuildHarness().RollingHashChunking());

    [Fact]
    public void StringConcatenation_SmallestLength_AgreesWithRollingHashChunking()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.RollingHashChunking(), harness.StringConcatenation());
    }

    [Fact]
    public void RollingHashChunking_SmallestLength_AgreesWithStringConcatenation()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.StringConcatenation(), harness.RollingHashChunking());
    }

    private static LongestChunkedPalindromeDecompositionBenchmarks BuildHarness()
    {
        var harness = new LongestChunkedPalindromeDecompositionBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
