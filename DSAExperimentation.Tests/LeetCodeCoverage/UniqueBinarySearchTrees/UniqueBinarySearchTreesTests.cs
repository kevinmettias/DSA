using DSAExperimentation.LeetCode.UniqueBinarySearchTrees;

namespace DSAExperimentation.Tests.LeetCodeCoverage.UniqueBinarySearchTrees;

// Harness only. Both strategies are UniqueBinarySearchTreesSolution's - this
// file just pins them to LeetCode's published examples.
public sealed class UniqueBinarySearchTreesTests
{
    public static TheoryData<int, int> Examples =>
        new()
        {
            { 3, 5 },
            { 1, 1 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void NumTreesByTabulation_LeetCodeExamples_ReturnsCatalanCount(int n, int expected) =>
        Assert.Equal(expected, UniqueBinarySearchTreesSolution.NumTreesByTabulation(n));

    [Theory]
    [MemberData(nameof(Examples))]
    public void NumTreesByMemoizedCatalan_LeetCodeExamples_ReturnsCatalanCount(int n, int expected) =>
        Assert.Equal(expected, UniqueBinarySearchTreesSolution.NumTreesByMemoizedCatalan(n));
}
