using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FindKthBitInNthBinaryStringBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - materializing S(n) and indexing it, against walking
// down one level per recursive call - so a harness whose arms disagree has read two different bits
// of the same string. Both answers are one char, so they are compared directly.
//
// Setup pins the bit position to the last bit of S(n), which the construction rule decides outright:
// S(n) ends with the inverted first bit of S(n-1), and S(n-1) always starts with S(1)'s '0', so the
// last bit is '1' for every order the benchmark asks for - a decisive value, not a seed artefact.
public sealed partial class FindKthBitInNthBinaryStringBenchmarksTests
{
    // The smaller of Setup's [Params(10, 20)] string orders.
    private const int SmallestStringOrder = 10;

    // S(n) = S(n-1) + "1" + invert(reverse(S(n-1))), so its last bit is the inverse of S(n-1)'s
    // first bit - and the base case S(1) = "0" makes that first bit '0' at every level.
    private const char ExpectedLastBit = '1';

    [Fact]
    public void Setup_SameStringOrder_RebuildsTheSameBitPosition() =>
        Assert.Equal(
            BuildHarness().BruteForceConstruction(),
            BuildHarness().BruteForceConstruction());

    [Fact]
    public void BruteForceConstruction_LastBitOfTheString_AgreesWithRecursiveBisection()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedLastBit, harness.BruteForceConstruction());
        Assert.Equal(harness.RecursiveBisection(), harness.BruteForceConstruction());
    }

    [Fact]
    public void RecursiveBisection_LastBitOfTheString_AgreesWithBruteForceConstruction()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedLastBit, harness.RecursiveBisection());
        Assert.Equal(harness.BruteForceConstruction(), harness.RecursiveBisection());
    }

    private static FindKthBitInNthBinaryStringBenchmarks BuildHarness()
    {
        var harness = new FindKthBitInNthBinaryStringBenchmarks { StringOrder = SmallestStringOrder };
        harness.Setup();

        return harness;
    }
}
