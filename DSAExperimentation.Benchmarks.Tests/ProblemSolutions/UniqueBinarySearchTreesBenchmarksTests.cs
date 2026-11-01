using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for UniqueBinarySearchTreesBenchmarks (ARCHITECTURE 17.9): both arms answer with
// LC 96's own quantity, the number of structurally distinct binary search trees over Nodes keys. That
// count is Catalan(Nodes), and Catalan(10) = 16796 fixes the smaller parameter's answer, so the
// literal is asserted of the baseline rather than left to the generic arm check's agreement, which
// two arms sharing a wrong number would pass.
public sealed partial class UniqueBinarySearchTreesBenchmarksTests
{
    // The smaller of the class's [Params(10, 16)] node counts.
    private const int SmallestNodeCount = 10;

    // Catalan(10) = 16796: the number of distinct BST shapes over 10 ordered keys.
    private const int ExpectedTreeCount = 16_796;

    [Fact]
    public void Tabulation_SmallestNodeCount_CountsTheCatalanNumber() =>
        Assert.Equal(ExpectedTreeCount, BuildHarness().Tabulation());

    private static UniqueBinarySearchTreesBenchmarks BuildHarness() => new() { Nodes = SmallestNodeCount };
}
