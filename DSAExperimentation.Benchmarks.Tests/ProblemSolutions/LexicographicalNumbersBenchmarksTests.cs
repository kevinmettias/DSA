using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for LexicographicalNumbersBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - sorting the numbers as strings against walking the
// digits as a tree - so a harness whose arms disagree is timing two different problems. The answer's
// outer order is pinned by the problem itself (lexicographic), so the two lists are rendered
// positionally rather than as an unordered set.
//
// There is no [GlobalSetup] here: UpperBound is the whole input, so there is no prepared workload to
// rebuild and no Setup member to cover.
public sealed partial class LexicographicalNumbersBenchmarksTests
{
    private const int SmallestUpperBound = 1_000;

    [Fact]
    public void StringSort_OneThousandIntegers_AgreesWithDepthFirstDigitTree()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(harness.DepthFirstDigitTree()), AnswerText.Of(harness.StringSort()));
    }

    [Fact]
    public void DepthFirstDigitTree_OneThousandIntegers_AgreesWithStringSort()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(harness.StringSort()), AnswerText.Of(harness.DepthFirstDigitTree()));
    }

    private static LexicographicalNumbersBenchmarks BuildHarness() =>
        new() { UpperBound = SmallestUpperBound };
}
