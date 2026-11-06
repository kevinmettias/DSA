using DSAExperimentation.Domain.Mathematics.GameTheory;

namespace DSAExperimentation.Tests.Algorithms.GameSearch.Fixtures;

// A token on a directed graph: a turn moves it along one edge out of its vertex, and a player who
// finds no edge out is stuck, with the outcome the rules give a stuck player: a loss under normal
// play, a win under misere play, or a draw. Both players have the same moves, so a position is the
// token's vertex alone, numbered as the graph numbers it. A cycle lets play go on for ever, which
// is how a position comes to be drawn.
internal readonly struct TokenGraphGame(int[][] edges, GameOutcome whenStuck) : IGameGraph<int>
{
    public int PositionCount => edges.Length;

    public int PositionAt(int index) => index;

    public int IndexOf(int position) => position;

    public int MoveCount(int position) => edges[position].Length;

    public int Next(int position, int move) => edges[position][move];

    public GameOutcome OutcomeWhenOver(int position) => whenStuck;
}
