namespace DSAExperimentation.Domain.Mathematics.Polyominoes.Tilings;

// List each shape once under this supply: two pieces of one shape would give each placement two
// rows meeting the same cells, and every tiling would be counted once per way of choosing between
// them.
/// <summary>Every piece used any number of times: no piece columns.</summary>
public readonly struct AnyNumber : IPieceSupply
{
    /// <inheritdoc/>
    public static int PrimaryColumnsFor(int pieces) => 0;

    /// <inheritdoc/>
    public static int SecondaryColumnsFor(int pieces) => 0;
}
