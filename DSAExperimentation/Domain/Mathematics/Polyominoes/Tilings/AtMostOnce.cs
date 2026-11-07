namespace DSAExperimentation.Domain.Mathematics.Polyominoes.Tilings;

/// <summary>Every piece used once or not at all: a secondary column per piece.</summary>
public readonly struct AtMostOnce : IPieceSupply
{
    /// <inheritdoc/>
    public static int PrimaryColumnsFor(int pieces) => 0;

    /// <inheritdoc/>
    public static int SecondaryColumnsFor(int pieces) => pieces;
}
