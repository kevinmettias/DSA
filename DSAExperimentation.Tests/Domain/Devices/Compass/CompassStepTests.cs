using DSAExperimentation.Domain.Devices.Compass;
using DSAExperimentation.Domain.Mathematics.Lattices;

namespace DSAExperimentation.Tests.Domain.Devices.Compass;

// The named steps are the fact itself, so the tests state them.
public sealed partial class CompassStepTests
{
    [Fact]
    public void Northward_IsOneUnitNorth() => Assert.Equal(new CompassStep(0, 1), CompassStep.Northward);

    [Fact]
    public void Eastward_IsOneUnitEast() => Assert.Equal(new CompassStep(1, 0), CompassStep.Eastward);

    [Fact]
    public void Southward_IsOneUnitSouth() => Assert.Equal(new CompassStep(0, -1), CompassStep.Southward);

    [Fact]
    public void Westward_IsOneUnitWest() => Assert.Equal(new CompassStep(-1, 0), CompassStep.Westward);

    // Rows grow downward on the grid and north is up on the compass, so a step down the rows goes south.
    [Fact]
    public void From_AStepDownAndRight_GoesSouthAndEast() => Assert.Equal(new CompassStep(1, -2), CompassStep.From(GridStep.DownDownRight));
}
