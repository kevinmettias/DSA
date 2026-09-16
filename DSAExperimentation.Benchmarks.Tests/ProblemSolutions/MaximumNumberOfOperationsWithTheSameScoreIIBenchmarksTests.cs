using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MaximumNumberOfOperationsWithTheSameScoreIIBenchmarks (ARCHITECTURE 17.9):
// both arms are MaximumNumberOfOperationsWithTheSameScoreIISolution's competing strategies for one
// question - the brute-force interval DP against the memoized two-pointer search over the same
// three moves - so a harness whose arms disagree is timing two different problems. Both answer with
// a single operation count, compared directly.
public sealed partial class MaximumNumberOfOperationsWithTheSameScoreIIBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameValueArray()
    {
        var first = BuildHarness();
        var second = BuildHarness();

        // The value array is private, so the rebuild is pinned through the count it produces: the
        // same Length must draw the same seeded values and search them identically.
        Assert.Equal(first.BruteForceDp(), second.BruteForceDp());
        Assert.Equal(first.MemoizedTwoPointer(), second.MemoizedTwoPointer());
    }

    [Fact]
    public void BruteForceDp_SeededValueArray_AgreesWithMemoizedTwoPointer()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MemoizedTwoPointer(), harness.BruteForceDp());
    }

    [Fact]
    public void MemoizedTwoPointer_SeededValueArray_AgreesWithBruteForceDp()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForceDp(), harness.MemoizedTwoPointer());
    }

    private static MaximumNumberOfOperationsWithTheSameScoreIIBenchmarks BuildHarness()
    {
        var harness = new MaximumNumberOfOperationsWithTheSameScoreIIBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
