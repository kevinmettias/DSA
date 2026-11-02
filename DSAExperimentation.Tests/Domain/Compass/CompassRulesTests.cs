using DSAExperimentation.Domain.Compass;

namespace DSAExperimentation.Tests.Domain.Compass;

public sealed partial class CompassRulesTests
{
    public static TheoryData<CompassHeading, CompassHeading> RightTurns => new()
    {
        { CompassHeading.North, CompassHeading.East },
        { CompassHeading.East, CompassHeading.South },
        { CompassHeading.South, CompassHeading.West },
        { CompassHeading.West, CompassHeading.North },
    };

    public static TheoryData<CompassHeading, int, int> Steps => new()
    {
        { CompassHeading.North, 0, 1 },
        { CompassHeading.East, 1, 0 },
        { CompassHeading.South, 0, -1 },
        { CompassHeading.West, -1, 0 },
    };

    public static TheoryData<CompassHeading> Headings => new() { CompassHeading.North, CompassHeading.East, CompassHeading.South, CompassHeading.West };

    // West to North pins the wrap at the end of the cycle.
    [Theory]
    [MemberData(nameof(RightTurns))]
    public void TurnRight_ReturnsTheNextHeadingClockwise(CompassHeading from, CompassHeading expected) =>
        Assert.Equal(expected, CompassRules.TurnRight(from));

    // North to West pins the wrap at the start of the cycle.
    [Theory]
    [MemberData(nameof(RightTurns))]
    public void TurnLeft_UndoesARightTurn(CompassHeading heading, CompassHeading turnedRight) =>
        Assert.Equal(heading, CompassRules.TurnLeft(turnedRight));

    [Theory]
    [MemberData(nameof(Steps))]
    public void StepOf_GoesOneUnitAlongTheHeading(CompassHeading heading, int east, int north) =>
        Assert.Equal((east, north), CompassRules.StepOf(heading));

    [Theory]
    [MemberData(nameof(Headings))]
    public void TurnRight_HeadingCountTimes_ComesBackToTheStart(CompassHeading heading)
    {
        var turned = heading;

        for (var turn = 0; turn < CompassRules.HeadingCount; turn++)
        {
            turned = CompassRules.TurnRight(turned);
        }

        Assert.Equal(heading, turned);
    }
}
