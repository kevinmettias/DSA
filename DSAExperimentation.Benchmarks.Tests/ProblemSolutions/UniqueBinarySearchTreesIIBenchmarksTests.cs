using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for UniqueBinarySearchTreesIIBenchmarks (ARCHITECTURE 17.9): its two arms are
// UniqueBinarySearchTreesIISolution's competing strategies for the same question - the unmemoized
// range recursion against the memoized one - so a harness whose arms disagree is building two
// different sets of trees.
//
// The arms report only .Count of the built list, which LC 95's own tests already pin the trees
// through; here the reported number is the whole of what the arms return, so agreement witnesses
// that both strategies built the same NUMBER of trees on the same key range, not that they built
// the same trees. The count is still decisive - it is Catalan(8) = 1430 for the smaller parameter,
// the same constant UniqueBinarySearchTreesBenchmarks' counting sibling is anchored to - so the
// literal is asserted alongside the agreement rather than left to a shared wrong number.
public sealed partial class UniqueBinarySearchTreesIIBenchmarksTests
{
    // The smaller of the class's [Params(8, 12)] node counts.
    private const int SmallestNodeCount = 8;

    // Catalan(8) = 1430: the number of distinct BST shapes over 8 ordered keys.
    private const int ExpectedTreeCount = 1_430;

    [Fact]
    public void PlainRecursion_SmallestNodeCount_AgreesWithTheSiblingArm()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedTreeCount, harness.PlainRecursion());
        Assert.Equal(harness.MemoizedRange(), harness.PlainRecursion());
    }

    [Fact]
    public void MemoizedRange_SmallestNodeCount_AgreesWithTheSiblingArm()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedTreeCount, harness.MemoizedRange());
        Assert.Equal(harness.PlainRecursion(), harness.MemoizedRange());
    }

    private static UniqueBinarySearchTreesIIBenchmarks BuildHarness() => new() { Nodes = SmallestNodeCount };
}
