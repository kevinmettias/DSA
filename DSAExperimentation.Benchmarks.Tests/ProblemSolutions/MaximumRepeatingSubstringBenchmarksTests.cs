using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MaximumRepeatingSubstringBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - the built-in substring search against the
// prefix-function scan over the same repeated word - so a harness whose arms disagree is timing two
// different problems. Setup concatenates Word end to end from one fixed constant, so the same Length
// must rebuild the same sequence; otherwise two published numbers were never comparable in the first
// place.
public sealed partial class MaximumRepeatingSubstringBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().StringContains(), BuildHarness().StringContains());

    [Fact]
    public void StringContains_WordRepeatedEndToEnd_AgreesWithPrefixFunctionSearchContains()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.PrefixFunctionSearchContains(), harness.StringContains());
    }

    [Fact]
    public void PrefixFunctionSearchContains_WordRepeatedEndToEnd_AgreesWithStringContains()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.StringContains(), harness.PrefixFunctionSearchContains());
    }

    private static MaximumRepeatingSubstringBenchmarks BuildHarness()
    {
        var harness = new MaximumRepeatingSubstringBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
