using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Tests.LeetCodeCoverage.UniqueBinarySearchTrees;

public sealed partial class UniqueBinarySearchTreesTests
{
    [Theory]
    [InlineData(3, 5)]
    [InlineData(1, 1)]
    public void NumTrees_LeetCodeExamples_ReturnsCatalanCount(int n, int expected) => Assert.Equal(expected, Count(n));
    private static int Count(int n) => Memoizer.Memoize<int, int>(n, Catalan);
    private static int Catalan(int nodes, Func<int, int> count) { if (nodes <= 1) return 1; var total = 0; for (var left = 0; left < nodes; left++) total += count(left) * count(nodes - left - 1); return total; }
}
