using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CatAndMouseIIBenchmarks (ARCHITECTURE 17.9): the generic check holds the
// arms to each other, but both answer with a bare bool, so agreement between them says the two
// strategies reached the same verdict on the same corridor and nothing more than that. This pins
// the baseline's verdict to one worked out from the corridor's layout without either arm.
public sealed partial class CatAndMouseIIBenchmarksTests
{
    private const int SmallestCorridorLength = 4;

    // At the smallest corridor the harness lays out 'F', '.', 'C', 'M': the cat sits on the
    // midpoint cell at index 2 and the mouse on the far end at index 3, so the two start adjacent
    // and the mouse - which moves first - can neither reach the food at index 0 in one jump nor
    // step onto the cat's cell without being captured there. Either way the cat's reply captures
    // it, so the mouse cannot force a win from this corridor and both arms must report that.
    private const bool MouseCanForceWinFromSmallestCorridor = false;

    [Fact]
    public void CanMouseWinByExhaustiveRecursion_MouseAdjacentToTheCat_ReportsTheMouseCannotForceAWin() =>
        Assert.Equal(MouseCanForceWinFromSmallestCorridor, BuildHarness().CanMouseWinByExhaustiveRecursion());

    private static CatAndMouseIIBenchmarks BuildHarness()
    {
        var harness = new CatAndMouseIIBenchmarks { CorridorLength = SmallestCorridorLength };
        harness.Setup();

        return harness;
    }
}
