using DSAExperimentation.Domain.Devices.Compass;
using DSAExperimentation.Domain.Mathematics.Lattices;

namespace DSAExperimentation.Tests.Domain.Devices.Compass;

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
        Assert.Equal(new CompassStep(east, north), CompassRules.StepOf(heading));

    // Every heading is named by exactly one point, so the step table has no empty or doubled slot.
    [Fact]
    public void Points_NameEveryHeadingOnce() =>
        Assert.Equal(Enum.GetValues<CompassHeading>(), CompassRules.Points.Select(point => point.Heading).Order());

    // The table is built from the pairing, not from the order the points are listed in.
    [Fact]
    public void StepOf_EveryPoint_IsThatPointsOwnStep() =>
        Assert.All(CompassRules.Points, point => Assert.Equal(point.Step, CompassRules.StepOf(point.Heading)));

    // A heading's number is its step's index in the square lattice's turning cycle, read north up,
    // which is what lets a compass turn be the lattice's turn.
    [Fact]
    public void StepOf_EachHeading_IsTheClockwiseStepAtItsNumberReadNorthUp() =>
        Assert.All(Enum.GetValues<CompassHeading>(), heading => Assert.Equal(
            CompassStep.From(GridDirections.Clockwise[(int)heading]),
            CompassRules.StepOf(heading)));

    [Fact]
    public void HeadingCount_IsTheNumberOfPoints() => Assert.Equal(CompassRules.HeadingCount, CompassRules.Points.Length);

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
