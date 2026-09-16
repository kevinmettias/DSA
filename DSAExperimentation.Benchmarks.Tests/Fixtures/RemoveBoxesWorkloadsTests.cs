using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for RemoveBoxesWorkloads (ARCHITECTURE 17.7). LC 546's boxes are colors, and
// the reading depends on the scatter genuinely repeating them: the un-memoized arm is exponential
// in the (left, right, extra) state space, and a run with no repeated color would collapse that
// space into a plain sweep.
public sealed partial class RemoveBoxesWorkloadsTests
{
    private const int BoxCount = 16;
    private const int Seed = 546; // LC problem number
    private const int MinBoxColor = 1;
    private const int MaxBoxColor = 3; // one below the fixture's own exclusive ceiling of 4

    [Fact]
    public void BuildBoxes_Count_ReturnsOneBoxPerPosition() =>
        Assert.Equal(BoxCount, RemoveBoxesWorkloads.BuildBoxes(BoxCount, Seed).Length);

    [Fact]
    public void BuildBoxes_EveryBox_IsAColorInsideTheDocumentedBand() =>
        Assert.All(
            RemoveBoxesWorkloads.BuildBoxes(BoxCount, Seed),
            box => Assert.InRange(box, MinBoxColor, MaxBoxColor));

    [Fact]
    public void BuildBoxes_Boxes_ContainARepeatedColor() =>
        Assert.Contains(
            RemoveBoxesWorkloads.BuildBoxes(BoxCount, Seed).GroupBy(box => box),
            group => group.Count() > 1);

    [Fact]
    public void BuildBoxes_SameSeed_ReturnsTheSameBoxes() =>
        Assert.Equal(
            RemoveBoxesWorkloads.BuildBoxes(BoxCount, Seed),
            RemoveBoxesWorkloads.BuildBoxes(BoxCount, Seed));
}
