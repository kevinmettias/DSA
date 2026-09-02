using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ValidSquare;

// LeetCode 593. Valid Square: computes all 6 pairwise squared distances among the
// 4 points, sorts them with this repo's own MergeSort over ArrayIndexedSequence
// (the same "sort, then scan the sorted shape" idiom HIndexTests uses), then checks
// the sorted shape a square must have - four equal positive "side" distances
// followed by two equal "diagonal" distances worth exactly double a side. Squared
// distances (not the square root) avoid floating-point comparisons entirely.
public sealed partial class ValidSquareTests
{
    [Fact]
    public void ValidSquare_UnitSquareInAnyOrder_ReturnsTrue()
    {
        int[] p1 = [0, 0];
        int[] p2 = [1, 1];
        int[] p3 = [1, 0];
        int[] p4 = [0, 1];

        var isValidSquare = ValidSquare(p1, p2, p3, p4);

        Assert.True(isValidSquare);
    }

    [Fact]
    public void ValidSquare_ThreeCornersPlusAFarPoint_ReturnsFalse()
    {
        int[] p1 = [0, 0];
        int[] p2 = [1, 1];
        int[] p3 = [1, 0];
        int[] p4 = [0, 12];

        var isValidSquare = ValidSquare(p1, p2, p3, p4);

        Assert.False(isValidSquare);
    }

    [Fact]
    public void ValidSquare_FourCollinearPoints_ReturnsFalse()
    {
        int[] p1 = [0, 0];
        int[] p2 = [1, 1];
        int[] p3 = [2, 2];
        int[] p4 = [3, 3];

        var isValidSquare = ValidSquare(p1, p2, p3, p4);

        Assert.False(isValidSquare);
    }

    [Fact]
    public void ValidSquare_FourCoincidentPoints_ReturnsFalse()
    {
        int[] p1 = [5, 5];
        int[] p2 = [5, 5];
        int[] p3 = [5, 5];
        int[] p4 = [5, 5];

        var isValidSquare = ValidSquare(p1, p2, p3, p4);

        Assert.False(isValidSquare);
    }

    private static bool ValidSquare(int[] p1, int[] p2, int[] p3, int[] p4)
    {
        var points = new[] { p1, p2, p3, p4 };
        var distances = new long[6];
        var next = 0;

        for (var i = 0; i < points.Length; i++)
        {
            for (var j = i + 1; j < points.Length; j++)
            {
                var dx = points[i][0] - points[j][0];
                var dy = points[i][1] - points[j][1];
                distances[next++] = ((long)dx * dx) + ((long)dy * dy);
            }
        }

        MergeSort.Sort<long, ArrayIndexedSequence<long>>(new ArrayIndexedSequence<long>(distances));

        var side = distances[0];
        return side > 0
            && distances[1] == side && distances[2] == side && distances[3] == side
            && distances[4] == distances[5] && distances[4] == 2 * side;
    }
}
