using DSAExperimentation.LeetCode.UniqueBinarySearchTrees;

namespace DSAExperimentation.LeetCode.Tests.UniqueBinarySearchTrees;

// Harness only. Both strategies are UniqueBinarySearchTreesSolution's - this
// file just pins them to LeetCode's published examples.
public sealed partial class UniqueBinarySearchTreesSolutionTests
{
    public static TheoryData<int, int> Examples =>
        new()
        {
            { 3, 5 },
            { 1, 1 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountTreesByTabulation_LeetCodeExamples_ReturnsCatalanCount(int nodeCount, int expected) =>
        Assert.Equal(expected, UniqueBinarySearchTreesSolution.CountTreesByTabulation(nodeCount));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountTreesByMemoizedCatalan_LeetCodeExamples_ReturnsCatalanCount(int nodeCount, int expected) =>
        Assert.Equal(expected, UniqueBinarySearchTreesSolution.CountTreesByMemoizedCatalan(nodeCount));
}
