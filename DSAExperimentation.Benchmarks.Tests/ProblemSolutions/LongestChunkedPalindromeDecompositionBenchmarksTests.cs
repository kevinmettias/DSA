using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for LongestChunkedPalindromeDecompositionBenchmarks (ARCHITECTURE 17.9), for what
// BenchmarkArmsTests cannot pin: the answer itself, known from Setup's construction rather than from
// either arm. The text's only 'a' is its first character, so every prefix starts with 'a' and no
// suffix of the same length does - no chunk can close before the window spans the whole text, and
// the decomposition is the single chunk. That is what forces both strategies to grow their pending
// window to the middle, so it is asserted at both sizes the class runs, LC 1147's cap included.
public sealed partial class LongestChunkedPalindromeDecompositionBenchmarksTests
{
    private const int SingleChunk = 1;

    public static TheoryData<int> Lengths => [200, 1_000];

    [Theory]
    [MemberData(nameof(Lengths))]
    public void StringConcatenation_UniqueFirstLetter_ClosesNoChunkBeforeTheWholeText(int length) =>
        Assert.Equal(SingleChunk, BuildHarness(length).StringConcatenation());

    [Theory]
    [MemberData(nameof(Lengths))]
    public void RollingHashChunking_UniqueFirstLetter_ClosesNoChunkBeforeTheWholeText(int length) =>
        Assert.Equal(SingleChunk, BuildHarness(length).RollingHashChunking());

    private static LongestChunkedPalindromeDecompositionBenchmarks BuildHarness(int length)
    {
        var harness = new LongestChunkedPalindromeDecompositionBenchmarks { Length = length };
        harness.Setup();

        return harness;
    }
}
