using DSAExperimentation.LeetCode.Harness;
using DSAExperimentation.LeetCode.SpiralMatrixIV;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SpiralMatrixIV;

// Harness only. Both strategies are SpiralMatrixIVSolution's - including the
// direction-array walk, which the benchmark used to own privately as its baseline
// and nothing asserted. SinglyLinkedListNode<int> is internal, so it cannot appear
// in a public TheoryData<...> member (CS0053); the examples state the node values
// and LeetCodeWireFormat builds the chain.
public sealed class SpiralMatrixIVTests
{
    public static TheoryData<int, int, int[], int[][]> Examples =>
        new()
        {
            // LC example 1: the list stops one cell short of the centre.
            {
                3, 5, [3, 0, 2, 6, 8, 1, 7, 9, 4, 2, 5, 5, 3, 9],
                [
                    [3, 0, 2, 6, 8],
                    [5, 3, 9, -1, 1],
                    [5, 2, 4, 9, 7],
                ]
            },

            // LC example 2: a single row, so the walk never turns.
            { 1, 4, [0, 1, 2], [[0, 1, 2, -1]] },

            // A single column, the transposed case: the walk turns once, at the
            // very first cell, and then only runs downwards.
            { 4, 1, [1, 2, 3], [[1], [2], [3], [-1]] },

            // Far shorter list than grid: the walk stops on the first side and
            // every cursor pull has to cope with a list that is already spent.
            { 2, 2, [7], [[7, -1], [-1, -1]] },

            // Exactly fills the grid, so the last write lands on the final cell
            // and the next-cell computation runs with nowhere legal left to go.
            { 2, 3, [1, 2, 3, 4, 5, 6], [[1, 2, 3], [6, 5, 4]] },

            // Runs out midway down the right-hand side, so the bottom and left
            // sides are never walked at all.
            {
                3, 3, [1, 2, 3, 4, 5],
                [
                    [1, 2, 3],
                    [-1, -1, 4],
                    [-1, -1, 5],
                ]
            },

            // The smallest grid LC allows: one cell, one node, no turn.
            { 1, 1, [5], [[5]] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void SpiralMatrixByDirectionArray_LeetCodeExamples_FillsSpiralPathAndPadsWithNegativeOne(
        int rowCount, int columnCount, int[] values, int[][] expected)
    {
        var head = LeetCodeWireFormat.ToLinkedList(values);
        var actual = SpiralMatrixIVSolution.SpiralMatrixByDirectionArray(rowCount, columnCount, head);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void SpiralMatrixByBoundaryShrink_LeetCodeExamples_FillsSpiralPathAndPadsWithNegativeOne(
        int rowCount, int columnCount, int[] values, int[][] expected)
    {
        var head = LeetCodeWireFormat.ToLinkedList(values);
        var actual = SpiralMatrixIVSolution.SpiralMatrixByBoundaryShrink(rowCount, columnCount, head);

        Assert.Equal(expected, actual);
    }
}
