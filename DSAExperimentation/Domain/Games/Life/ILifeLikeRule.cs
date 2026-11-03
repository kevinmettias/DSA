namespace DSAExperimentation.Domain.Games.Life;

// A Life-like cellular automaton's rule: on a grid where every cell is live or dead and counts its
// eight neighbours, which counts bring a dead cell to life and which keep a live one alive. A rule is
// written as Life's own community writes it, in B/S notation: Conway's Game of Life is B3/S23,
// HighLife B36/S23, Seeds B2/S. LifeLike reads the rulestring once into two masks (ARCHITECTURE §17.6:
// the general rule in Domain, each game a configuration of it).
/// <summary>A Life-like rule: the neighbour counts at which a dead cell is born and a live one survives.</summary>
public interface ILifeLikeRule
{
    /// <summary>
    /// The rule in B/S notation, such as "B3/S23": 'B' and the counts at which a dead cell is born, then
    /// '/', 'S' and the counts at which a live cell survives, each count a digit from 0 to 8.
    /// </summary>
    static abstract string Rulestring { get; }
}
