using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for EditDistanceBenchmarks (ARCHITECTURE 17.9): an expected distance known from
// the workload's shape. Setup builds a run of Length 'a's against the same run one character shorter
// followed by a 'b', which differ in exactly one trailing character, so LC 72's distance between
// them is one substitution; that is what pins the workload to the documented shape.
public sealed partial class EditDistanceBenchmarksTests
{
    private const int SmallestLength = 20;
    private const int ExpectedDistanceForOneTrailingSubstitution = 1;

    [Fact]
    public void Tabulation_RunAgainstItsOneCharacterLongerSibling_DistanceIsOne() =>
        Assert.Equal(ExpectedDistanceForOneTrailingSubstitution, BuildHarness().Tabulation());

    private static EditDistanceBenchmarks BuildHarness()
    {
        var harness = new EditDistanceBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
