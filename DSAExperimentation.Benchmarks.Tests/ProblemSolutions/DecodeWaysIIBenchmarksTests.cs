using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DecodeWaysIIBenchmarks (ARCHITECTURE 17.9): the generic arm check compares the
// arms only with each other, so what stays here is a count derived without either arm. Setup repeats
// the wildcard pair "2*" Length/2 times, which decodes to the fixed count below, so the reading is a
// decisive value rather than an arbitrary one.
public sealed partial class DecodeWaysIIBenchmarksTests
{
    private const int SmallestLength = 20;

    // The decoding count of ten consecutive "2*" pairs - 6 for the pair read as one letter (21..26, the
    // wildcard being 1..9), 9 for the two digits read separately, plus every mixed split of the run -
    // is 1188331105203, and LeetCode 639 requires the answer reduced modulo 1e9+7, which is the value
    // the solution returns.
    private const long UnreducedDecodingCount = 1188331105203;
    private const long RequiredModulus = 1_000_000_007;
    private const long ExpectedDecodingCount = UnreducedDecodingCount % RequiredModulus;

    [Fact]
    public void Tabulation_TenWildcardPairs_CountsTheReducedDecodings() =>
        Assert.Equal(ExpectedDecodingCount, BuildHarness().Tabulation());

    private static DecodeWaysIIBenchmarks BuildHarness()
    {
        var harness = new DecodeWaysIIBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
