namespace DSAExperimentation.Domain.Games.Dice;

// A die's faces: a run of consecutive numbers, one per face. Most dice start at 1, but a ten-sided
// die is often numbered 0 to 9, so the lowest face is part of the die and not assumed. Which roll
// may follow which, or what a roll scores, belongs to the game a die is used in (ARCHITECTURE §17.6).
/// <summary>A die: how many faces it has, and the number on the lowest.</summary>
public interface IDie
{
    /// <summary>The number on the lowest face.</summary>
    static abstract int LowestFace { get; }

    /// <summary>How many faces the die has, each numbered one more than the last.</summary>
    static abstract int FaceCount { get; }
}
