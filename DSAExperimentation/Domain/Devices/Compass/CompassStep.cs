namespace DSAExperimentation.Domain.Devices.Compass;

// One unit step on the plane, given as how far it goes east and how far north, because the problems
// disagree on which axis is which (CompassRules). The four steps a compass heading takes are named,
// so a reader sees Northward rather than decoding (0, 1). A positional record already names its
// parts East and North, so the steps are named for where they go: Northward, Eastward.
/// <summary>A step on the plane: how far it goes east, and how far north.</summary>
/// <param name="East">How far the step goes east; negative goes west.</param>
/// <param name="North">How far the step goes north; negative goes south.</param>
public readonly record struct CompassStep(int East, int North)
{
    /// <summary>One unit north: (0, 1).</summary>
    public static CompassStep Northward { get; } = new(0, 1);

    /// <summary>One unit east: (1, 0).</summary>
    public static CompassStep Eastward { get; } = new(1, 0);

    /// <summary>One unit south: (0, -1).</summary>
    public static CompassStep Southward { get; } = new(0, -1);

    /// <summary>One unit west: (-1, 0).</summary>
    public static CompassStep Westward { get; } = new(-1, 0);
}
