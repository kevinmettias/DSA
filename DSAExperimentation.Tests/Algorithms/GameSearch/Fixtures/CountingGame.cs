using System.Numerics;
using DSAExperimentation.Domain.Mathematics.GameTheory;

namespace DSAExperimentation.Tests.Algorithms.GameSearch.Fixtures;

// A game whose rules are another's, counting every move a search makes into the one-element
// array it is given: how many positions the search reached, so two searches' work can be compared.
internal readonly struct CountingGame<TGame, TState, TScore>(TGame game, long[] moves)
    : IAlternatingGame<TState, TScore>
    where TGame : struct, IAlternatingGame<TState, TScore>
    where TScore : INumber<TScore>
{
    public int MoveCount(TState state) => game.MoveCount(state);

    public TState Next(TState state, int move)
    {
        moves[0]++;

        return game.Next(state, move);
    }

    public TScore Gain(TState state, int move) => game.Gain(state, move);

    public TScore ValueWhenOver(TState state) => game.ValueWhenOver(state);
}
