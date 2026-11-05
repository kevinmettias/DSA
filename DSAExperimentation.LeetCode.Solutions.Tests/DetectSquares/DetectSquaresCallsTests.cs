using DSAExperimentation.LeetCode.Solutions.DetectSquares;

namespace DSAExperimentation.LeetCode.Solutions.Tests.DetectSquares;

// LeetCode's example: (3, 10), (11, 2) and (3, 2) added, so (11, 10) closes one square.
public sealed partial class DetectSquaresCallsTests
{
    [Fact]
    public void Count_PointClosingASquare_CountsIt() => Assert.Equal(1, DetectSquaresCalls.Count(Squares(), [11, 10]));

    [Fact]
    public void Count_PointClosingNone_IsZero() => Assert.Equal(0, DetectSquaresCalls.Count(Squares(), [14, 8]));

    [Fact]
    public void Add_PointAddedTwice_CountsEachSquareTwice()
    {
        var squares = Squares();

        DetectSquaresCalls.Add(squares, [3, 2]);

        Assert.Equal(2, DetectSquaresCalls.Count(squares, [11, 10]));
    }

    private static DetectSquaresSolution.IDetectSquares Squares()
    {
        var squares = DetectSquaresSolution.CreateByHashMapGroupedByX();
        DetectSquaresCalls.Add(squares, [3, 10]);
        DetectSquaresCalls.Add(squares, [11, 2]);
        DetectSquaresCalls.Add(squares, [3, 2]);

        return squares;
    }
}
