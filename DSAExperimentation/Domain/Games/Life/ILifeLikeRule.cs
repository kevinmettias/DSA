namespace DSAExperimentation.Domain.Games.Life;

// A Life-like cellular automaton's rule: on a grid where every cell is live or dead and counts its
// eight neighbours, which counts bring a dead cell to life and which keep a live one alive. Conway's
// Game of Life is one such rule, B3/S23; HighLife, B36/S23, is another. Each count set is a mask,
// bit n standing for n live neighbours, so a rule is two constants and LifeLike reads them with a
// shift (ARCHITECTURE §17.6: the general rule in Domain, each game a configuration of it).
/// <summary>A Life-like rule: the neighbour counts at which a dead cell is born and a live one survives.</summary>
public interface ILifeLikeRule
{
    /// <summary>The counts at which a dead cell is born, bit n standing for n live neighbours.</summary>
    static abstract int BirthMask { get; }

    /// <summary>The counts at which a live cell survives, bit n standing for n live neighbours.</summary>
    static abstract int SurvivalMask { get; }
}
