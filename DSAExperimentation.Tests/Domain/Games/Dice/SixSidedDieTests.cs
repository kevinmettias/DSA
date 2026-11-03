using DSAExperimentation.Domain.Games.Dice;

namespace DSAExperimentation.Tests.Domain.Games.Dice;

// The faces are the fact itself, so the test states them.
public sealed partial class SixSidedDieTests
{
    [Fact]
    public void Faces_AreOneThroughSixFair() =>
        Assert.Equal<DieFace>(DieFace.FairFaces(1, 2, 3, 4, 5, 6), SixSidedDie.Faces);
}
