using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MinimumAbsoluteDifferenceBetweenElementsWithConstraintBenchmarks (ARCHITECTURE
// 17.9): its two arms are competing strategies for the same question - the O(n^2) scan of every admissible
// pair against the O(n log n) BinarySearchTree<int> sliding window queried by FindClosest.TryFind - so a
// harness whose arms disagree is timing two different problems. Both arms return the minimum absolute
// difference as an int, so they are compared directly, and both only read the value array and the index
// gap, so one harness is safe to read twice in either order. Setup draws the values from one fixed seed and
// derives the gap from Length, so the same Length must rebuild the same array and the same gap.
public sealed partial class MinimumAbsoluteDifferenceBetweenElementsWithConstraintBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().BruteForcePairScan(), BuildHarness().BruteForcePairScan());

    [Fact]
    public void BruteForcePairScan_SeededValuesAndIndexGap_AgreesWithBstSlidingWindow()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BstSlidingWindow(), harness.BruteForcePairScan());
    }

    [Fact]
    public void BstSlidingWindow_SeededValuesAndIndexGap_AgreesWithBruteForcePairScan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForcePairScan(), harness.BstSlidingWindow());
    }

    private static MinimumAbsoluteDifferenceBetweenElementsWithConstraintBenchmarks BuildHarness()
    {
        var harness = new MinimumAbsoluteDifferenceBetweenElementsWithConstraintBenchmarks
        {
            Length = SmallestLength,
        };

        harness.Setup();

        return harness;
    }
}
