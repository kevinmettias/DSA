using System.Collections.Immutable;

namespace DSAExperimentation.Domain.Devices.Compass;

// A walker on the plane as LC 874 and LC 1041 state it: it faces one of four compass headings,
// turns a quarter turn left or right, and steps one unit along its heading. These are the
// problems' *what* (ARCHITECTURE §17.6). A step is given as how far it goes east and north, not
// as (x, y) or (row, col), because the problems disagree on which axis is which: each maps the two
// onto its own coordinates. How a solution reads its commands and tests a square stay with each
// strategy; every arm, baselines among them, reads the headings, turns and steps here (§17.5). GridDirections' tables are not headings:
// a heading's order is a turning cycle, not a neighbour set.
//
// The heading count is a power of two, so a turn wraps with a mask rather than a %, whose signed
// form costs a sign fix-up. A step is read from a table by the heading's number rather than
// chosen by a switch: the JIT judged a four-way switch unprofitable to inline, so LC 874's step
// loop made a call per step and ran about 10% slower, where a table read inlines. The table is built
// from the named points, each step placed at its own point's heading, so the order the points are
// listed in cannot put a step under the wrong heading.
/// <summary>How a compass heading turns, and which way one step along it goes.</summary>
public static class CompassRules
{
    /// <summary>The number of headings, a quarter turn apart.</summary>
    public const int HeadingCount = 4;

    // The mask that wraps a heading's number at HeadingCount.
    private const int HeadingMask = HeadingCount - 1;

    // The cardinal points, the one source the step table is built from, declared first because static
    // initializers run in the order they are written.
    private static readonly ImmutableArray<CompassPoint> Cardinal = [CompassPoint.North, CompassPoint.East, CompassPoint.South, CompassPoint.West];

    // Each heading's step, at the heading's number.
    private static readonly CompassStep[] StepsByHeading = PlaceSteps(Cardinal);

    /// <summary>The four cardinal points, clockwise from north.</summary>
    public static ImmutableArray<CompassPoint> Points => Cardinal;

    /// <summary>The heading a quarter turn clockwise of the given one.</summary>
    /// <param name="heading">The heading turned from.</param>
    /// <returns>The heading after turning right.</returns>
    public static CompassHeading TurnRight(CompassHeading heading) => (CompassHeading)(((int)heading + 1) & HeadingMask);

    /// <summary>The heading a quarter turn anticlockwise of the given one.</summary>
    /// <param name="heading">The heading turned from.</param>
    /// <returns>The heading after turning left.</returns>
    public static CompassHeading TurnLeft(CompassHeading heading) => (CompassHeading)(((int)heading + HeadingMask) & HeadingMask);

    /// <summary>How far one step along a heading goes east and north.</summary>
    /// <param name="heading">The heading stepped along.</param>
    /// <returns>The step's eastward and northward parts, each -1, 0 or 1.</returns>
    public static CompassStep StepOf(CompassHeading heading) => StepsByHeading[(int)heading];

    private static CompassStep[] PlaceSteps(ImmutableArray<CompassPoint> points)
    {
        var steps = new CompassStep[HeadingCount];

        foreach (var point in points)
        {
            steps[(int)point.Heading] = point.Step;
        }

        return steps;
    }
}
