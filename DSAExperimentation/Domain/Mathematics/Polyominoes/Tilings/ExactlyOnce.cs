namespace DSAExperimentation.Domain.Mathematics.Polyominoes.Tilings;

/// <summary>Every piece used exactly once: a primary column per piece.</summary>
public readonly struct ExactlyOnce : IPieceSupply
{
    /// <inheritdoc/>
    public static int PrimaryColumnsFor(int pieces) => pieces;

    /// <inheritdoc/>
    public static int SecondaryColumnsFor(int pieces) => 0;
}
