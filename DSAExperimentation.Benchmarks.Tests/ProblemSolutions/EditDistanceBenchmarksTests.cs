using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for EditDistanceBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - the BCL 2D table filled bottom-up against the identical
// suffix-pair recurrence driven top-down through this repo's Memoizer - so a harness whose arms
// disagree is timing two different problems. Setup builds a run of Length 'a's against the same run
// one character shorter followed by a 'b', which differ in exactly one trailing character, so LC
// 72's distance between them is one substitution; that is what pins the workload to the documented
// shape, and it is rebuilt identically on every harness built from the same Length.
public sealed partial class EditDistanceBenchmarksTests
{
    private const int SmallestLength = 20;
    private const int ExpectedDistanceForOneTrailingSubstitution = 1;

    [Fact]
    public void Setup_RunAgainstItsOneCharacterLongerSibling_DistanceIsOneAndRebuildsTheSameWorkload()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedDistanceForOneTrailingSubstitution, harness.Tabulation());
        Assert.Equal(harness.Tabulation(), BuildHarness().Tabulation());
    }

    [Fact]
    public void Tabulation_LettersDifferingAtTheLastPosition_AgreesWithMemoizedRecurrence()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MemoizedRecurrence(), harness.Tabulation());
    }

    [Fact]
    public void MemoizedRecurrence_LettersDifferingAtTheLastPosition_AgreesWithTabulation()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.Tabulation(), harness.MemoizedRecurrence());
    }

    private static EditDistanceBenchmarks BuildHarness()
    {
        var harness = new EditDistanceBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
