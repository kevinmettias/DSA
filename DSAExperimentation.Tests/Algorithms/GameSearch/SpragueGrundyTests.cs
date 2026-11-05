using DSAExperimentation.Algorithms.GameSearch;
using DSAExperimentation.Tests.Algorithms.GameSearch.Fixtures;

namespace DSAExperimentation.Tests.Algorithms.GameSearch;

// The two Grundy sequences everything else is built from: a subtraction game taking one to three is
// periodic with period four, and a Nim heap's number is its size.
public sealed partial class SpragueGrundyTests
{
    private const int MostPerTurn = 3;
    private const int LargestPile = 40;

    [Fact]
    public void Number_TakeAway_IsThePileModuloFour() =>
        Assert.All(
            Enumerable.Range(0, LargestPile + 1),
            pile => Assert.Equal(pile % (MostPerTurn + 1), SpragueGrundy.Number(new TakeAwayGame(MostPerTurn), pile)));

    [Fact]
    public void Number_NimHeap_IsItsSize() =>
        Assert.All(Enumerable.Range(0, LargestPile + 1), size => Assert.Equal(size, SpragueGrundy.Number(new NimHeap(), size)));
}
