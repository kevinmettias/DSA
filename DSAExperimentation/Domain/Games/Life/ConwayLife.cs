namespace DSAExperimentation.Domain.Games.Life;

// John Conway's Game of Life, B3/S23: a dead cell with exactly three live neighbours is born, and a
// live cell with two or three survives. LC 289 plays it.
/// <summary>Conway's Game of Life, the Life-like rule B3/S23.</summary>
public readonly struct ConwayLife : ILifeLikeRule
{
    /// <summary>B3/S23: born on three, survives on two or three.</summary>
    public static string Rulestring => "B3/S23";
}
