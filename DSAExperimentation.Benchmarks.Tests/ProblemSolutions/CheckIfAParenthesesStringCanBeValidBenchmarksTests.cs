using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CheckIfAParenthesesStringCanBeValidBenchmarks (ARCHITECTURE 17.9): the
// generic check holds the arms to each other, but both answer with a bare bool, so agreement says
// only that the two strategies reached the same verdict on the same bracket string and lock mask.
// This pins the baseline's verdict to one worked out from Setup's layout without either arm.
public sealed partial class CheckIfAParenthesesStringCanBeValidBenchmarksTests
{
    private const int SmallestLength = 200;

    // Setup builds a locked '(' at index 0, a locked ')' at index 199, and 198 free positions
    // between them. The two locked characters already carry the one '(' and one ')' a 200-character
    // balanced string needs at its ends, so the free middle only has to supply 99 of each - which it
    // can, in order: 99 more '(' from index 1 and then 99 ')' from index 100 leaves every prefix
    // balance non-negative and the last one at zero. The free middle can therefore be rebooked into
    // a valid bracket string, and both arms must say so.
    private const bool CanBeBalancedByTheFreeMiddle = true;

    [Fact]
    public void CanBeValidByReachableOpenCountDp_LockedEndsFreeMiddle_ReturnsTrue() =>
        Assert.Equal(CanBeBalancedByTheFreeMiddle, BuildHarness().CanBeValidByReachableOpenCountDp());

    private static CheckIfAParenthesesStringCanBeValidBenchmarks BuildHarness()
    {
        var harness = new CheckIfAParenthesesStringCanBeValidBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
