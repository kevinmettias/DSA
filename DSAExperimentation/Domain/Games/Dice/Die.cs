namespace DSAExperimentation.Domain.Games.Dice;

// The range a die's faces cover, read from its IDie witness. The witness's members are static
// abstract members of a struct, so each die's bounds fold to constants in a caller's loop.
/// <summary>The faces of a die, from its lowest to its highest.</summary>
/// <typeparam name="TDie">The die, such as <see cref="SixSidedDie"/>.</typeparam>
public static class Die<TDie>
    where TDie : struct, IDie
{
    /// <summary>The number on the lowest face.</summary>
    public static int LowestFace => TDie.LowestFace;

    /// <summary>The number on the highest face.</summary>
    public static int HighestFace => TDie.LowestFace + TDie.FaceCount - 1;
}
