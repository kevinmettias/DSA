using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MaximumDeletionsOnAStringBenchmarks (ARCHITECTURE 17.9): both arms are
// competing strategies for one question - the most deletion rounds the string admits - so a harness
// whose arms disagree is timing two different problems. Setup draws the text from a fixed seed, so
// the same length must rebuild the same text; neither arm mutates it.
public sealed partial class MaximumDeletionsOnAStringBenchmarksTests
{
    private const int SmallestLength = 80;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().NaiveSubstringComparison(), BuildHarness().NaiveSubstringComparison());

    [Fact]
    public void NaiveSubstringComparison_AgreesWithRollingHashScreenedDp()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.NaiveSubstringComparison(), harness.RollingHashScreenedDp());
    }

    [Fact]
    public void RollingHashScreenedDp_AgreesWithNaiveSubstringComparison()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.RollingHashScreenedDp(), harness.NaiveSubstringComparison());
    }

    private static MaximumDeletionsOnAStringBenchmarks BuildHarness()
    {
        var harness = new MaximumDeletionsOnAStringBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
