using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SearchInsertPositionBenchmarks (ARCHITECTURE 17.9): both arms report the same
// insertion position for the same value, so a harness whose arms disagree is timing two different
// problems. Setup builds Length even values by the class's own step and aims one past the largest of
// them, so the target falls strictly beyond every element and the answer is fixed independently of
// either strategy: the whole array precedes it, so its insertion position is exactly Length. Both
// arms only read the array, so one harness instance is safe to call twice in either order.
public sealed partial class SearchInsertPositionBenchmarksTests
{
    private const int SmallestLength = 200;

    // The target sits past every element, so every element precedes it.
    private const int ExpectedInsertIndex = SmallestLength;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload()
    {
        Assert.Equal(BuildHarness().LinearScan(), BuildHarness().LinearScan());
        Assert.Equal(ExpectedInsertIndex, BuildHarness().LinearScan());
    }

    [Fact]
    public void LinearScan_EvenValuesPastTheTarget_AgreesWithBinarySearchLowerBound()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BinarySearchLowerBound(), harness.LinearScan());
    }

    [Fact]
    public void BinarySearchLowerBound_EvenValuesPastTheTarget_AgreesWithLinearScan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.LinearScan(), harness.BinarySearchLowerBound());
    }

    private static SearchInsertPositionBenchmarks BuildHarness()
    {
        var harness = new SearchInsertPositionBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
