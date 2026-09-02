using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SearchA2DMatrixII;

// LeetCode 240. Search a 2D Matrix II: each row is independently sorted
// ascending (unlike LC 74, rows are not chained end-to-start), so this repo's
// own BinarySearch.Find over an ArraySequence<int> witness per row is a
// direct, correct composition of two existing primitives - see the benchmark
// for why it is not the asymptotically fastest option.
public sealed class SearchA2DMatrixIITests
{
    private static readonly int[][] Matrix =
    [
        [1, 4, 7, 11, 15],
        [2, 5, 8, 12, 19],
        [3, 6, 9, 16, 22],
        [10, 13, 14, 17, 24],
        [18, 21, 23, 26, 30],
    ];

    [Theory]
    [InlineData(5, true)]
    [InlineData(20, false)]
    public void SearchMatrix_LeetCodeExamples_FindsPresenceCorrectly(int target, bool expected)
    {
        var found = SearchMatrix(Matrix, target);
        Assert.Equal(expected, found);
    }

    [Fact]
    public void SearchMatrix_EmptyMatrix_ReturnsFalse()
    {
        var found = SearchMatrix([], 1);
        Assert.False(found);
    }

    private static bool SearchMatrix(int[][] matrix, int target)
    {
        foreach (var row in matrix)
        {
            var sequence = new ArraySequence<int>(row);
            if (BinarySearch.Find(sequence, target) is not null)
            {
                return true;
            }
        }

        return false;
    }
}
