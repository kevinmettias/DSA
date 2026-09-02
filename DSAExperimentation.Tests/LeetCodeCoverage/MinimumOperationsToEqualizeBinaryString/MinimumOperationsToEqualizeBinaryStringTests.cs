using DSAExperimentation.LeetCode.MinimumOperationsToEqualizeBinaryString;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumOperationsToEqualizeBinaryString;

// Harness only. The zero-count graph itself is EqualizeStateGraph and both search
// strategies are MinimumOperationsToEqualizeBinaryStringSolution's - this file just
// pins them to LeetCode's published examples, including the unreachable case.
public sealed class MinimumOperationsToEqualizeBinaryStringTests
{
    public static TheoryData<string, int, int> Examples =>
        new()
        {
            { "110", 1, 1 },
            { "0101", 3, 2 },
            { "101", 2, -1 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinOperationsByMutationQueue_LeetCodeExamples_ReturnsFewestFlipsToAllOnes(
        string s, int k, int expected) =>
        Assert.Equal(expected, MinimumOperationsToEqualizeBinaryStringSolution.MinOperationsByMutationQueue(s, k));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinOperationsByReduceGraph_LeetCodeExamples_ReturnsFewestFlipsToAllOnes(
        string s, int k, int expected) =>
        Assert.Equal(expected, MinimumOperationsToEqualizeBinaryStringSolution.MinOperationsByReduceGraph(s, k));
}
