using DSAExperimentation.Domain.Games.TicTacToe;

namespace DSAExperimentation.LeetCode.Solutions.FindWinnerOnATicTacToeGame;

// What LC 1275 itself fixes beside the game's rules: the strings its answer is spelled with, and the
// cell alphabet both strategies mark with (Empty for an untouched square, +/-1 for the two players -
// the sign is what lets the running-counts arm tally a line). Its own type because the solution's
// subject is the pair of scanning strategies; these values beside them would be that file's second
// subject.
//
// The game's rules - the board size, the turn order, the diagonals, how a game ends - are
// Domain/Games/TicTacToe's (ARCHITECTURE §17.6), and the board-rescan baseline reads the board's
// size there too (§17.5).
internal static class TicTacToeVocabulary
{
    // The two players the board-rescan baseline tallies a move for.
    internal const int PlayerCount = 2;

    // LC 1275 names the game's X "A" and its O "B".
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
            TicTacToeOutcome.XWins => PlayerA,
            TicTacToeOutcome.OWins => PlayerB,
            TicTacToeOutcome.Draw => Draw,
            _ => Pending,
        };
}
