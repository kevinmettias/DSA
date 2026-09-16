using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DecodeWaysIIBenchmarks (ARCHITECTURE 17.9): its two arms are competing strategies
// for the same question - a bottom-up table against the same recurrence memoized from the top - so a
// harness whose arms disagree is timing two different problems. Setup repeats the wildcard pair "2*"
// Length/2 times, which decodes to the fixed count below, so the reading is a decisive value rather
// than an arbitrary one, and the same Length must rebuild the same script and with it the same count.
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
    public void Setup_SameLength_RebuildsTheSameWorkload()
    {
        Assert.Equal(ExpectedDecodingCount, BuildHarness().Tabulation());
        Assert.Equal(BuildHarness().Tabulation(), BuildHarness().Tabulation());
    }

    [Fact]
    public void Tabulation_TenWildcardPairs_AgreesWithMemoized()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.Memoized(), harness.Tabulation());
    }

    [Fact]
    public void Memoized_TenWildcardPairs_AgreesWithTabulation()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.Tabulation(), harness.Memoized());
    }

    private static DecodeWaysIIBenchmarks BuildHarness()
    {
        var harness = new DecodeWaysIIBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
