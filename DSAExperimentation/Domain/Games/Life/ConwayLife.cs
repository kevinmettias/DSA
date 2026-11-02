namespace DSAExperimentation.Domain.Games.Life;

// John Conway's Game of Life, B3/S23: a dead cell with exactly three live neighbours is born, and a
// live cell with two or three survives. LC 289 plays it.
/// <summary>Conway's Game of Life, the Life-like rule B3/S23.</summary>
public readonly struct ConwayLife : ILifeLikeRule
{
    /// <summary>Born on three live neighbours.</summary>
    public static int BirthMask => 1 << 3;

    /// <summary>Survives on two or three live neighbours.</summary>
    public static int SurvivalMask => (1 << 2) | (1 << 3);
}
