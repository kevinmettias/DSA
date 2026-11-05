using DSAExperimentation.Domain.Mathematics.GameTheory;

namespace DSAExperimentation.Tests.Algorithms.GameSearch.Fixtures;

// A pile of stones from which each turn takes one to `most`; whoever takes the last stone wins (normal
// play). It is both an alternating game, valued +1 for a win and -1 for a loss, and an impartial game,
// so one fixture checks both solvers against the known answer: with most = 3, the player to move loses
// exactly when the pile is a multiple of 4, and its Grundy number is the pile modulo 4.
internal readonly struct TakeAwayGame(int most) : IAlternatingGame<int, int>, IImpartialGame<int>
{
    private const int Loss = -1;

    public int MoveCount(int state) => Math.Min(state, most);

    // Move i takes i + 1 stones.
    public int Next(int state, int move) => state - (move + 1);

    public int Gain(int state, int move) => 0;

    public int ValueWhenOver(int state) => Loss;
}
