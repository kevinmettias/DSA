using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for LongestHappyPrefixBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - the O(n^2) shrink-and-compare candidate scan
// against this repo's KMP prefix function - so a harness whose arms disagree is timing two
// different problems. Both arms return the prefix itself, compared directly. Setup's text is
// Length - 1 copies of 'a' followed by one 'b': no proper prefix ends in 'b', so no non-empty
// candidate can be a suffix too and the answer is the empty prefix, which is the decisive value
// both arms must reach and the same Length must rebuild.
public sealed partial class LongestHappyPrefixBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SmallestLength_RebuildsTheSameWorkload()
    {
        Assert.Equal(string.Empty, BuildHarness().PrefixFunctionLookup());
        Assert.Equal(BuildHarness().ShrinkAndCompare(), BuildHarness().ShrinkAndCompare());
    }

    [Fact]
    public void ShrinkAndCompare_SmallestLength_AgreesWithPrefixFunctionLookup()
    {
        var harness = BuildHarness();

        Assert.Equal(string.Empty, harness.ShrinkAndCompare());
        Assert.Equal(harness.PrefixFunctionLookup(), harness.ShrinkAndCompare());
    }

    [Fact]
    public void PrefixFunctionLookup_SmallestLength_AgreesWithShrinkAndCompare()
    {
        var harness = BuildHarness();

        Assert.Equal(string.Empty, harness.PrefixFunctionLookup());
        Assert.Equal(harness.ShrinkAndCompare(), harness.PrefixFunctionLookup());
    }

    private static LongestHappyPrefixBenchmarks BuildHarness()
    {
        var harness = new LongestHappyPrefixBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
