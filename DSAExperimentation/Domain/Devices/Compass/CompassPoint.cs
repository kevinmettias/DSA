namespace DSAExperimentation.Domain.Devices.Compass;

// A cardinal point of the compass: a heading, and the step a walker facing it takes. Each point is a
// named value pairing the two, so CompassRules lists the points by name and builds its step table
// from the pairing itself, not from the order the steps happen to be written in.
/// <summary>A cardinal point: a heading and the step taken facing it.</summary>
/// <param name="Heading">The heading.</param>
/// <param name="Step">The unit step a walker facing <paramref name="Heading"/> takes.</param>
public readonly record struct CompassPoint(CompassHeading Heading, CompassStep Step)
{
    /// <summary>North, stepping northward.</summary>
    public static CompassPoint North { get; } = new(CompassHeading.North, CompassStep.Northward);

    /// <summary>East, stepping eastward.</summary>
    public static CompassPoint East { get; } = new(CompassHeading.East, CompassStep.Eastward);

    /// <summary>South, stepping southward.</summary>
    public static CompassPoint South { get; } = new(CompassHeading.South, CompassStep.Southward);

    /// <summary>West, stepping westward.</summary>
    public static CompassPoint West { get; } = new(CompassHeading.West, CompassStep.Westward);
}
