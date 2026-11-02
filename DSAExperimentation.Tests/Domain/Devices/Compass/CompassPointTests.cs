using DSAExperimentation.Domain.Devices.Compass;

namespace DSAExperimentation.Tests.Domain.Devices.Compass;

// Each point pairs a heading with the step a walker facing it takes.
public sealed partial class CompassPointTests
{
    [Fact]
    public void North_StepsNorthward() =>
        Assert.Equal(new CompassPoint(CompassHeading.North, CompassStep.Northward), CompassPoint.North);

    [Fact]
    public void East_StepsEastward() =>
        Assert.Equal(new CompassPoint(CompassHeading.East, CompassStep.Eastward), CompassPoint.East);

    [Fact]
    public void South_StepsSouthward() =>
        Assert.Equal(new CompassPoint(CompassHeading.South, CompassStep.Southward), CompassPoint.South);

    [Fact]
    public void West_StepsWestward() =>
        Assert.Equal(new CompassPoint(CompassHeading.West, CompassStep.Westward), CompassPoint.West);
}
