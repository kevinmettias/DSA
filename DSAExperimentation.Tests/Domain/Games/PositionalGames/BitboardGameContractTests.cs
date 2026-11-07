using DSAExperimentation.Domain.Games.PositionalGames;
using DSAExperimentation.Domain.Games.PositionalGames.InARow;
using DSAExperimentation.Domain.Games.Turns;

namespace DSAExperimentation.Tests.Domain.Games.PositionalGames;

// IBitboardGame's contract, written once and run over both forms on the same m,n,k board under
// free placement: BitboardGame, the template, through its winning sets (BitboardGameTests), and
// InARowGame, the strategy for boards beyond 64 cells, through its shifts (InARowGameTests). A
// class that derives from this one names its shape, its game and its position, and builds the
// game on a board. Every rule is held to PositionalGame's, the generic winning-set rule over
// MnkGame's lines on an array of owners: the moves and their numbering, each claim's verdict and
// the value at the end, over random games played to their end, and the refusals.
public abstract partial class BitboardGameContractTests<TShape, TGame, TState>
    where TShape : struct, IMnkShape
    where TGame : struct, IBitboardGame<TState>
{
    private const int RandomGames = 150;
    private const int Seed = 1950;
    private const int Draw = 0;
    private const int WinValue = BitboardGame<FreePlacement>.WinValue;

    private static int Cells => MnkGame<TShape>.CellCount;

    // A run of k from a start cell along a step: along the top row from the left edge, down the
    // last column, the diagonal from the top left, and the anti-diagonal from the top right.
    public static TheoryData<int, int, int, int> RunsAlongEachDirection => new()
    {
        { 0, 0, 0, 1 },
        { 0, TShape.Columns - 1, 1, 0 },
        { 0, 0, 1, 1 },
        { 0, TShape.Columns - 1, 1, -1 },
    };

    [Fact]
    public void MoveCount_EmptyBoard_IsEveryCell() =>
        Assert.Equal(Cells, Full().MoveCount(Full().Replay([])));

    [Fact]
    public void ChoiceOf_EveryMove_IsTheOpenCellsLowestFirst()
    {
        int[] played = [3, Cells - 1, 0, TShape.Columns];
        var game = Full();
        var position = game.Replay(played);
        int[] open = [.. Enumerable.Range(0, Cells).Except(played)];

        Assert.Equal(
            open,
            Enumerable.Range(0, game.MoveCount(position))
                .Select(move => game.ChoiceOf(position, move)));
    }

    [Theory]
    [MemberData(nameof(RunsAlongEachDirection))]
    public void Replay_KInARow_LeavesThePlayerToMoveLost(
        int row, int column, int rowStep, int columnStep)
    {
        var game = Full();
        var won = game.Replay(RunWithReplies(row, column, rowStep, columnStep));
        var claimed = (2 * TShape.InARow) - 1;

        Assert.Equal((0, claimed - WinValue), (game.MoveCount(won), game.ValueWhenOver(won)));
    }

    [Fact]
    public void Replay_ACellTaken_IsRefused() =>
        Assert.Throws<ArgumentException>(() => Full().Replay([1, 1]));

    [Fact]
    public void Replay_ACellOffTheBoard_IsRefused() =>
        Assert.Throws<ArgumentException>(() => Full().Replay([Cells]));

    [Fact]
    public void Replay_AHole_IsRefused() =>
        Assert.Throws<ArgumentException>(() => GameOn(new BoardMask(Cells, 2)).Replay([2]));

    [Fact]
    public void Replay_AfterAWin_IsRefused() =>
        Assert.Throws<ArgumentException>(
            () => Full().Replay([.. RunWithReplies(0, 0, 0, 1), Cells - 1]));

    // Two cells left by holes and no line on them: the board fills with nobody's run.
    [Fact]
    public void ValueWhenOver_AFullBoardWithNoRun_IsADraw()
    {
        var game = GameOn(new BoardMask(Cells, [.. Enumerable.Range(2, Cells - 2)]));
        var full = game.Replay([0, 1]);

        Assert.Equal((0, Draw), (game.MoveCount(full), game.ValueWhenOver(full)));
    }

    [Fact]
    public void Gain_IsNothing() =>
        Assert.Equal(0, Full().Gain(Full().Replay([]), 0));

    // Random games to their end, each claim judged against PositionalGame's rule on an array of
    // owners: the moves open, the cell each move claims, whether the claim ends the game, and the
    // value at the end.
    [Fact]
    public void RandomGames_EveryMove_AgreesWithTheWinningLines() =>
        Assert.All(PlayedGames(), game => Assert.Equal(game.Expected, game.Found));

    // Builds the game on a board of the shape.
    protected abstract TGame GameOn(BoardMask board);

    private TGame Full() => GameOn(MnkGame<TShape>.Board);

    // The first player's run of k from a start along a step, the second player answering each move
    // on the cells of the bottom row from the right, away from the run.
    private static int[] RunWithReplies(int row, int column, int rowStep, int columnStep)
    {
        var moves = new List<int>();

        for (var at = 0; at < TShape.InARow; at++)
        {
            moves.Add(MnkGame<TShape>.CellAt(row + (at * rowStep), column + (at * columnStep)));

            if (at + 1 < TShape.InARow)
            {
                moves.Add(MnkGame<TShape>.CellAt(TShape.Rows - 1, TShape.Columns - 1 - at));
            }
        }

        return [.. moves];
    }

    private IEnumerable<(int[] Expected, int[] Found)> PlayedGames()
    {
        var random = new Random(Seed);
        var rules = new PositionalGame<TwoPlayersAlternating>(
            MnkGame<TShape>.Board, MnkGame<TShape>.Lines);

        for (var played = 0; played < RandomGames; played++)
        {
            yield return new RandomGame(Full(), rules).PlayOut(random);
        }
    }

    // One game played at random to its end, each claim judged twice: by PositionalGame's rule on
    // an array of owners, and by the game under test. Per move, the open count, the cell claimed
    // and whether the claim ended the game; at the end, the value.
    private sealed class RandomGame(TGame game, PositionalGame<TwoPlayersAlternating> rules)
    {
        private readonly int[] _owners = MnkGame<TShape>.Board.EmptyPosition();
        private readonly List<int> _expected = [];
        private readonly List<int> _found = [];
        private TState _position = game.Replay([]);
        private bool _isOver;

        public (int[] Expected, int[] Found) PlayOut(Random random)
        {
            for (var seat = 0; !_isOver && _owners.Contains(BoardMask.Unclaimed); seat = 1 - seat)
            {
                Play(seat, random);
            }

            _expected.Add(ValueByTheOwners());
            _found.Add(game.ValueWhenOver(_position));

            return ([.. _expected], [.. _found]);
        }

        // A random open cell's move, judged by the owners' rule, then by the game.
        private void Play(int seat, Random random)
        {
            int[] open =
                [.. Enumerable.Range(0, Cells).Where(cell => _owners[cell] == BoardMask.Unclaimed)];
            var move = random.Next(open.Length);
            JudgeByTheOwners(seat, open, move);
            JudgeByTheGame(move);
        }

        private void JudgeByTheOwners(int seat, int[] open, int move)
        {
            var cell = open[move];
            _owners[cell] = seat;
            _isOver = rules.HasCompletedASet(_owners, cell, seat);
            _expected.AddRange([open.Length, cell, _isOver ? 1 : 0]);
        }

        private void JudgeByTheGame(int move)
        {
            _found.AddRange([game.MoveCount(_position), game.ChoiceOf(_position, move)]);
            _position = game.Next(_position, move);
            _found.Add(IsLost() ? 1 : 0);
        }

        // A win is worth WinValue less the claimed cells to the winner, so the loser to move holds
        // its negation; a full board is a draw.
        private int ValueByTheOwners()
        {
            if (!_isOver)
            {
                return Draw;
            }

            return _owners.Count(owner => owner != BoardMask.Unclaimed) - WinValue;
        }

        // Whether the player to move has lost: no move left, and a value below a draw.
        private bool IsLost() =>
            game.MoveCount(_position) == 0 && game.ValueWhenOver(_position) < Draw;
    }
}
