using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for UniqueBinarySearchTreesBenchmarks (ARCHITECTURE 17.9): its two arms are
// UniqueBinarySearchTreesSolution's competing strategies for the same question - the bottom-up
// Catalan table against the memoized Catalan recurrence - so a harness whose arms disagree is
// counting two different families of trees.
//
// Both arms answer with LC 96's own quantity, the number of structurally distinct binary search
// trees over Nodes keys, so agreement is agreement on the whole answer. That count is Catalan(Nodes),
// and Catalan(10) = 16796 fixes the smaller parameter's answer, so the literal is asserted alongside
// the agreement rather than left to the arms to agree on a shared wrong number.
public sealed partial class UniqueBinarySearchTreesBenchmarksTests
{
    // The smaller of the class's [Params(10, 16)] node counts.
    private const int SmallestNodeCount = 10;

    // Catalan(10) = 16796: the number of distinct BST shapes over 10 ordered keys.
    private const int ExpectedTreeCount = 16_796;

    [Fact]
    public void Tabulation_SmallestNodeCount_AgreesWithTheSiblingArm()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedTreeCount, harness.Tabulation());
        Assert.Equal(harness.MemoizedCatalan(), harness.Tabulation());
    }

    [Fact]
    public void MemoizedCatalan_SmallestNodeCount_AgreesWithTheSiblingArm()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedTreeCount, harness.MemoizedCatalan());
        Assert.Equal(harness.Tabulation(), harness.MemoizedCatalan());
    }

    private static UniqueBinarySearchTreesBenchmarks BuildHarness() => new() { Nodes = SmallestNodeCount };
}
