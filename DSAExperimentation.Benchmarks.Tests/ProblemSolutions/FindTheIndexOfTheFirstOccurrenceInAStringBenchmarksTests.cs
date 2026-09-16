using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FindTheIndexOfTheFirstOccurrenceInAStringBenchmarks (ARCHITECTURE 17.9): its
// two arms are competing strategies for one question - the BCL string search against a rolling
// hash that answers the same question, both handed the same Haystack/Needle wrappers - so a
// harness whose arms disagree is timing two different problems. Setup pins needle at the end of an
// all-'a' haystack, so the same Length must rebuild the same pair.
public sealed partial class FindTheIndexOfTheFirstOccurrenceInAStringBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().StringIndexOf(), BuildHarness().StringIndexOf());

    [Fact]
    public void StringIndexOf_SmallestLength_AgreesWithRollingHashSearchFirst()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.RollingHashSearchFirst(), harness.StringIndexOf());
    }

    [Fact]
    public void RollingHashSearchFirst_SmallestLength_AgreesWithStringIndexOf()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.StringIndexOf(), harness.RollingHashSearchFirst());
    }

    private static FindTheIndexOfTheFirstOccurrenceInAStringBenchmarks BuildHarness()
    {
        var harness = new FindTheIndexOfTheFirstOccurrenceInAStringBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
