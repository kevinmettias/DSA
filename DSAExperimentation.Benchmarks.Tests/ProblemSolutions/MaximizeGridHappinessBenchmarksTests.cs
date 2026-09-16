using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MaximizeGridHappinessBenchmarks (ARCHITECTURE 17.9): both arms are competing
// strategies for one question - the best grid happiness placing the fixed introvert and extrovert
// counts into the layout - so a harness whose arms disagree is timing two different problems. Setup
// builds the GridLayout from the row count and the class's fixed column count, so the same row count
// must rebuild the same layout; the arms then read that one layout and mutate nothing.
public sealed partial class MaximizeGridHappinessBenchmarksTests
{
    private const int SmallestRowCount = 3;

    [Fact]
    public void Setup_SameRowCount_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().UnmemoizedRecursion(), BuildHarness().UnmemoizedRecursion());

    [Fact]
    public void UnmemoizedRecursion_AgreesWithMemoizedRecursion()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.UnmemoizedRecursion(), harness.MemoizedRecursion());
    }

    [Fact]
    public void MemoizedRecursion_AgreesWithUnmemoizedRecursion()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MemoizedRecursion(), harness.UnmemoizedRecursion());
    }

    private static MaximizeGridHappinessBenchmarks BuildHarness()
    {
        var harness = new MaximizeGridHappinessBenchmarks { Rows = SmallestRowCount };
        harness.Setup();

        return harness;
    }
}
