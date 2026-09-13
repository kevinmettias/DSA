using DSAExperimentation.LeetCode.QueriesOnAPermutationWithKey;

namespace DSAExperimentation.Tests.LeetCodeCoverage.QueriesOnAPermutationWithKey;

// Harness only: both strategies live in QueriesOnAPermutationWithKeySolution and
// are asserted against the same examples - LeetCode's two, plus a single-query
// case, a repeated-query case that must report 0 once the value is already at
// the front, and the last-element case that exercises a full-length scan.
public sealed class QueriesOnAPermutationWithKeyTests
{
    public static TheoryData<int[], int, int[]> Examples =>
        new()
        {
            { [3, 1, 2, 1], 5, [2, 1, 2, 1] },
            { [4, 1, 2, 2], 4, [3, 1, 2, 0] },
            { [1], 1, [0] },
            { [2, 2, 2], 3, [1, 0, 0] },
            { [5, 4, 3, 2, 1], 5, [4, 4, 4, 4, 4] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void ProcessQueriesByListMoveToFront_LeetCodeExamples_ReturnsQueriedIndices(
        int[] queries,
        int m,
        int[] expected) =>
        Assert.Equal(expected, QueriesOnAPermutationWithKeySolution.ProcessQueriesByListMoveToFront(queries, m));

    [Theory]
    [MemberData(nameof(Examples))]
    public void ProcessQueriesByDynamicArrayMoveToFront_LeetCodeExamples_ReturnsQueriedIndices(
        int[] queries,
        int m,
        int[] expected) =>
        Assert.Equal(
            expected,
            QueriesOnAPermutationWithKeySolution.ProcessQueriesByDynamicArrayMoveToFront(queries, m));
}
