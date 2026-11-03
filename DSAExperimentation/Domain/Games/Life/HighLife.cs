namespace DSAExperimentation.Domain.Games.Life;

// HighLife, B36/S23: Conway's rule with one more birth, on six live neighbours, which lets a pattern
// copy itself. A well-known second Life-like rule, named here as a preset (ARCHITECTURE §17.6).
/// <summary>HighLife, the Life-like rule B36/S23.</summary>
public readonly struct HighLife : ILifeLikeRule
{
    /// <summary>B36/S23: born on three or six, survives on two or three.</summary>
    public static string Rulestring => "B36/S23";
}
