using DSAExperimentation.LeetCode.Solutions.QueriesOnAPermutationWithKey;

namespace DSAExperimentation.LeetCode.Solutions.Tests.QueriesOnAPermutationWithKey;

// Harness only: both strategies live in QueriesOnAPermutationWithKeySolution and
// are asserted against the same cases - LeetCode's two, plus a single-query
// case, a repeated-query case that must report 0 once the value is already at
// the front, and the last-element case that exercises a full-length scan.
public sealed partial class QueriesOnAPermutationWithKeySolutionTests
{
    public static TheoryData<int[], int, int[]> Cases =>
        new()
        {
            { [1], 1, [0] },
            { [2, 2, 2], 3, [1, 0, 0] },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void AnswerQueriesByListMoveToFront_Cases_ReturnsQueriedIndices(
        int[] queries,
        int permutationSize,
        int[] expected)
    {
        var actual = QueriesOnAPermutationWithKeySolution.AnswerQueriesByListMoveToFront(queries, permutationSize);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void AnswerQueriesByDynamicArrayMoveToFront_Cases_ReturnsQueriedIndices(
        int[] queries,
        int permutationSize,
        int[] expected)
    {
        var actual = QueriesOnAPermutationWithKeySolution.AnswerQueriesByDynamicArrayMoveToFront(queries, permutationSize);

        Assert.Equal(expected, actual);
    }
}
