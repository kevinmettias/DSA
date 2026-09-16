using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for AllPossibleFullBinaryTreesBenchmarks (ARCHITECTURE 17.9): its two arms are
// AllPossibleFullBinaryTreesSolution's competing strategies for the same question - plain recursion against a
// memo keyed by node count - so a harness whose arms disagree is enumerating two different shapes. The class has
// no [GlobalSetup]: the single [Params] node count is the whole input, so the harness is constructed per size and
// the arms called directly. Both report only a count, which for a full binary tree is the Catalan number of
// (n-1)/2 - a decisive literal for the smaller size rather than a restatement of the arms' own output.
public sealed partial class AllPossibleFullBinaryTreesBenchmarksTests
{
    // The smaller of [Params(13, 19)] node counts. A full binary tree has an odd node count,
    // and 13 leaves 6 internal nodes to split.
    private const int SmallestNodes = 13;

    // The 6th Catalan number: the number of full binary trees over 13 nodes.
    private const int ExpectedFullTreeCount = 132;

    [Fact]
    public void Naive_ThirteenNodes_AgreesWithMemoized()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedFullTreeCount, harness.Naive());
        Assert.Equal(harness.Memoized(), harness.Naive());
    }

    [Fact]
    public void Memoized_ThirteenNodes_AgreesWithNaive()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedFullTreeCount, harness.Memoized());
        Assert.Equal(harness.Naive(), harness.Memoized());
    }

    private static AllPossibleFullBinaryTreesBenchmarks BuildHarness() =>
        new() { Nodes = SmallestNodes };
}
