using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for UniquePathsBenchmarks (ARCHITECTURE 17.9). Arm agreement is
// BenchmarkArmsTests' job; this pins the count at both published sizes. A path across an n x n
// grid is n - 1 downs and n - 1 rights in any order, so there are C(2n - 2, n - 1) of them.
//
// The larger size is the class's claim to stay inside LC 62's guarantee that the answer fits:
// C(32, 16) is the count for 17, and at 18 the count, 2,333,606,220, would wrap in an int and both
// arms would agree on the wrapped value.
public sealed partial class UniquePathsBenchmarksTests
{
    private const int SmallestSize = 10;
    private const int LargestSize = 17;

    // C(18, 9): the paths across a 10 x 10 grid.
    private const int SmallestGridPathCount = 48_620;

    // C(32, 16): the paths across a 17 x 17 grid.
    private const int LargestGridPathCount = 601_080_390;

    [Fact]
    public void Combinatorics_SmallestSize_CountsEveryOrderOfDownsAndRights() =>
        Assert.Equal(SmallestGridPathCount, BuildHarness(SmallestSize).Combinatorics());

    [Fact]
    public void MemoizedRecurrence_SmallestSize_CountsEveryOrderOfDownsAndRights() =>
        Assert.Equal(SmallestGridPathCount, BuildHarness(SmallestSize).MemoizedRecurrence());

    [Fact]
    public void Combinatorics_LargestSize_StaysInsideTheIntTheProblemPromises() =>
        Assert.Equal(LargestGridPathCount, BuildHarness(LargestSize).Combinatorics());

    [Fact]
    public void MemoizedRecurrence_LargestSize_StaysInsideTheIntTheProblemPromises() =>
        Assert.Equal(LargestGridPathCount, BuildHarness(LargestSize).MemoizedRecurrence());

    private static UniquePathsBenchmarks BuildHarness(int size) => new() { Size = size };
}
