using DSAExperimentation.Domain.Games.PositionalGames.InARow;

namespace DSAExperimentation.Tests.Algorithms.GameSearch.PositionalGames.Fixtures;

// Gomoku positions drawn by hand, test-only, as GomokuDiagram reads them: X to move, rows numbered
// from the top. Each says what X can force.
internal static class GomokuPositions
{
    // X's stones on three lines that cross: (5,6) makes a broken four along row 5 and a four up the
    // anti-diagonal through (6,5) at once, a double four O cannot block, so the shortest win is
    // three plies; longer ones run through the diagonal and row 7.
    public static readonly string[] DoubleFour =
    [
        "O.............O",
        "...............",
        "...............",
        "..O.......O....",
        "...X...X.X.....",
        "....X..XX......",
        ".....X.........",
        "....XX.........",
        "...............",
        "...............",
        "............O..",
        "...............",
        ".O.............",
        ".......O.......",
        "O.............O",
    ];

    // X X _ X X along row 7, closed at both ends: the gap is the only winning cell.
    public static readonly string[] BrokenFour =
    [
        "O..............",
        "...............",
        "...............",
        "...............",
        "...............",
        "...............",
        "...............",
        "..OXX.XXO......",
        "...............",
        "...............",
        "...............",
        "...............",
        "...............",
        "...............",
        "..............O",
    ];

    // X's only fours are on row 7. (7,8) is blocked at (7,7) and leads nowhere; (7,7) forces O to
    // (7,8), which makes O's own four down column 8, so X must take (6,8) with a four, and cannot.
    // Ignoring O's four, X would go on to an open four down column 7, and O would win first.
    public static readonly string[] CounterFour =
    [
        "...............",
        "...............",
        "...............",
        "...............",
        "...............",
        "...............",
        "...............",
        "...OXXX........",
        ".......XO......",
        ".......XO......",
        "........O......",
        "........X......",
        "...............",
        "...............",
        "...............",
    ];

    // The same, with X's two stones up the anti-diagonal through (6,8): taking O's winning cell
    // there makes X an open four, so the win follows O's counter-four, five plies deep.
    public static readonly string[] CounterFourBlockedByAFour =
    [
        "...............",
        "...............",
        "...............",
        "...............",
        "..........X....",
        ".........X.....",
        "...............",
        "...OXXX........",
        ".......XO......",
        ".......XO......",
        "........O......",
        "........X......",
        "...............",
        "...............",
        "...............",
    ];

    // X X _ X X X along row 7 between O's stones: the gap makes six, a win in freestyle only.
    public static readonly string[] OnlyAnOverline =
    [
        "O..............",
        "...............",
        "...............",
        "...............",
        "...............",
        "...............",
        "...............",
        ".OXX.XXXO......",
        "...............",
        "...............",
        "...............",
        "...............",
        "...............",
        "...............",
        "..............O",
    ];

    // Drawn at random and kept because its shortest victory by continuous fours is six fours
    // long, thirteen plies, beyond what alpha-beta searches.
    public static readonly string[] SixFoursDeep =
    [
        "...............",
        "...............",
        "...............",
        "...O...OOX.....",
        "......X........",
        "...............",
        "....OX.X..O....",
        "......O..XX....",
        ".....O.OX.X....",
        "......X...O....",
        "......O..O.....",
        ".....XX.OX.....",
        "...............",
        "...............",
        "...............",
    ];

    // An open three of X's on row 7 and a closed four of O's down column 2: O wins at (11,2)
    // unless it is blocked, and X has no win of its own.
    public static readonly string[] OpponentsFour =
    [
        "...............",
        "...............",
        "...............",
        "...............",
        "...............",
        "...............",
        "..X............",
        "..O...XXX......",
        "..O............",
        "..O............",
        "..O............",
        "...............",
        "...............",
        "...............",
        "...............",
    ];

    // The same with a fourth stone of X's on row 7: X wins at (7,5) or (7,10) before O can.
    public static readonly string[] BothFours =
    [
        "...............",
        "...............",
        "...............",
        "...............",
        "...............",
        "...............",
        "..X............",
        "..O...XXXX.....",
        "..O............",
        "..O............",
        "..O............",
        "...............",
        "...............",
        "...............",
        "...............",
    ];

    public static readonly string[] EmptyBoard =
        [.. Enumerable.Repeat(new string('.', GomokuBoard.Columns), GomokuBoard.Rows)];
}
