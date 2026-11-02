namespace DSAExperimentation.Domain.Games.Life;

// HighLife, B36/S23: Conway's rule with one more birth, on six live neighbours, which lets a pattern
// copy itself. A well-known second Life-like rule, named here as a preset (ARCHITECTURE §17.6).
/// <summary>HighLife, the Life-like rule B36/S23.</summary>
public readonly struct HighLife : ILifeLikeRule
{
    /// <summary>Born on three or six live neighbours.</summary>
    public static int BirthMask => (1 << 3) | (1 << 6);

    /// <summary>Survives on two or three live neighbours, as in Conway's rule.</summary>
    public static int SurvivalMask => (1 << 2) | (1 << 3);
}
