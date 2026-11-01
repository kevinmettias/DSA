using DSAExperimentation.Domain.TicTacToe;

namespace DSAExperimentation.LeetCode.FindWinnerOnATicTacToeGame;

// What LC 1275 itself fixes beside the game's rules: the strings its answer is spelled with, and the
// cell alphabet both strategies mark with (Empty for an untouched square, +/-1 for the two players -
// the sign is what lets the running-counts arm tally a line). Its own type because the solution's
// subject is the pair of scanning strategies; these values beside them would be that file's second
// subject.
//
// The game's rules - the board size, the turn order, the diagonals, how a game ends - are
// Domain/TicTacToe's (ARCHITECTURE §17.6). The board-rescan baseline keeps its own copy of the ones it
// uses, BoardSize and PlayerCount, since a baseline may use no library type (§17.5).
internal static class TicTacToeVocabulary
{
    // The baseline's own copy of LC 1275's 3 x 3 board and of its two players.
    internal const int BoardSize = 3;
    internal const int PlayerCount = 2;

    internal const string PlayerA = "A";
    internal const string PlayerB = "B";
    internal const string Draw = "Draw";
    internal const string Pending = "Pending";

    internal const int Empty = 0;
    internal const int MarkA = 1;
    internal const int MarkB = -1;

    // LC 1275's spelling of how a game stands.
    internal static string AnswerFor(TicTacToeOutcome outcome) =>
        outcome switch
        {
            TicTacToeOutcome.AWins => PlayerA,
            TicTacToeOutcome.BWins => PlayerB,
            TicTacToeOutcome.Draw => Draw,
            _ => Pending,
        };
}
