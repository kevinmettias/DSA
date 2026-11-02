namespace DSAExperimentation.Domain.Games.Zuma;

// Zuma as PopCap made it and LC 488 states it: three or more touching balls of one colour vanish.
/// <summary>Zuma's own rule: three or more touching balls of one colour vanish.</summary>
public readonly struct ClassicZuma : IZumaRule
{
    /// <summary>Three.</summary>
    public static int ShortestVanishingGroup => 3;
}
