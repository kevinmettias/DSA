using DSAExperimentation.Domain.Games.SlidingPuzzle;

namespace DSAExperimentation.LeetCode.SlidingPuzzle;

// LC 773's board, two rows of three: this problem's configuration of the general puzzle in
// Domain/Games/SlidingPuzzle, beside the 8- and 15-puzzles it names.
internal readonly struct TwoByThreeBoard : IPuzzleBoard
{
    public static int Rows => 2;

    public static int Columns => 3;
}
