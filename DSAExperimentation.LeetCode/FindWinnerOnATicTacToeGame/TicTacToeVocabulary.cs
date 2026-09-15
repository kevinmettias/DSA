namespace DSAExperimentation.LeetCode.FindWinnerOnATicTacToeGame;

// Everything LC 1275 fixes: the board's extent, the two marks a cell can hold, the
// player labels and the two outcomes that are not a win. Its own type because the
// solution's subject is the pair of scanning strategies - nine values beside them
// would be that file's second subject, so somebody changing a mark or a label
// would have to read an implementation to find it and somebody reading the
// implementation would step over the values first.
//
// The marks are the board's cell alphabet (Empty is the untouched square,
// +/-1 the two players) and the labels are what the answer is spelled with, which
// is why they sit together rather than being split by "is this a number or a
// string": they are one vocabulary the problem defines and the solution only
// reads.
internal static class TicTacToeVocabulary
{
    // LC 1275's board is 3 x 3, and a line is three marks long. The one value a
    // caller may want to vary, which the strategies expose as their own
    // `boardSize` parameter for exactly that reason.
    internal const int BoardSize = 3;

    internal const string PlayerA = "A";
    internal const string PlayerB = "B";
    internal const string Draw = "Draw";
    internal const string Pending = "Pending";

    internal const int PlayerCount = 2;
    internal const int Empty = 0;
    internal const int MarkA = 1;
    internal const int MarkB = -1;
}
