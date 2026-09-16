using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for TypeOfTriangleBenchmarks (ARCHITECTURE 17.9): its two arms are
// TypeOfTriangleSolution's competing strategies for the same question - three direct pairwise
// comparisons against copying the sides into an indexed sequence and MergeSorting it - so a harness
// whose arms disagree is answering two different questions.
//
// LC 3024 fixes nums.Length at 3, so the class carries no [Params] and no [GlobalSetup] to call: its
// one workload is the fixed [3, 4, 5] side triple, and a harness is nothing more than a new
// instance. Those three distinct sides settle LC 3024's answer as "scalene", and that literal is
// asserted alongside the arms' agreement so agreement cannot hold on a shared wrong answer.
public sealed partial class TypeOfTriangleBenchmarksTests
{
    // The fixed side triple's decisive classification: 3, 4 and 5 are all different.
    private const string ExpectedClassification = "scalene";

    [Fact]
    public void DirectComparison_ThreeFourFiveSides_AgreesWithTheSiblingArm()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(ExpectedClassification), AnswerText.Of(harness.DirectComparison()));
        Assert.Equal(AnswerText.Of(harness.MergeSort()), AnswerText.Of(harness.DirectComparison()));
    }

    [Fact]
    public void MergeSort_ThreeFourFiveSides_AgreesWithTheSiblingArm()
    {
        var harness = BuildHarness();

        Assert.Equal(AnswerText.Of(ExpectedClassification), AnswerText.Of(harness.MergeSort()));
        Assert.Equal(AnswerText.Of(harness.DirectComparison()), AnswerText.Of(harness.MergeSort()));
    }

    private static TypeOfTriangleBenchmarks BuildHarness() => new();
}
