using System.Collections.Immutable;

namespace DSAExperimentation.Domain.Games.Dice;

// A die in its most general form: a list of faces, each a number with a weight. A common die's faces
// are a fair run, 1 to 6 or 1 to 20; a ten-sided die often runs 0 to 9; a Fudge die shows -1, 0 and
// +1 twice each; a doubling cube shows powers of two; a loaded die weighs its faces unequally. Which
// roll may follow which, or what a roll scores, belongs to the game a die is used in, and how a roll
// is drawn is a sampler's (ARCHITECTURE §17.6: the distribution is the fact, the sampler the algorithm).
/// <summary>A die: its faces, each a number with a weight.</summary>
public interface IDie
{
    /// <summary>The die's faces, at least one, their weights none negative and at least one positive.</summary>
    static abstract ImmutableArray<DieFace> Faces { get; }
}
