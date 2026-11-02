namespace DSAExperimentation.Domain.Compass;

// The four headings in clockwise order from North, numbered so that a quarter turn right is one
// step along them and the numbers wrap at CompassRules.HeadingCount.
/// <summary>A heading on the compass: one of the four cardinal directions.</summary>
public enum CompassHeading
{
    /// <summary>Facing north, where a step adds one to the northward coordinate.</summary>
    North = 0,

    /// <summary>Facing east, a quarter turn right of north.</summary>
    East = 1,

    /// <summary>Facing south, a half turn from north.</summary>
    South = 2,

    /// <summary>Facing west, a quarter turn left of north.</summary>
    West = 3,
}
