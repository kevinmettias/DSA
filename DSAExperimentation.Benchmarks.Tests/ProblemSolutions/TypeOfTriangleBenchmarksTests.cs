using DSAExperimentation.Benchmarks.ProblemSolutions;
using DSAExperimentation.LeetCode.Conventions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for TypeOfTriangleBenchmarks (ARCHITECTURE 17.9): LC 3024 fixes nums.Length at 3,
// so the class carries no [Params] and no [GlobalSetup] to call: its one workload is the fixed
// [3, 4, 5] side triple, and a harness is nothing more than a new instance. Those three distinct
// sides settle LC 3024's answer as "scalene", and that literal is asserted of the baseline so the
// generic arm check's agreement cannot hold on a shared wrong answer.
public sealed partial class TypeOfTriangleBenchmarksTests
{
    // The fixed side triple's decisive classification: 3, 4 and 5 are all different.
    private const string ExpectedClassification = "scalene";

    [Fact]
    public void DirectComparison_ThreeFourFiveSides_ClassifiesAsScalene() =>
        Assert.Equal(AnswerGraphText.Of(ExpectedClassification), AnswerGraphText.Of(BuildHarness().DirectComparison()));

    private static TypeOfTriangleBenchmarks BuildHarness() => new();
}
