using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ClimbingStairsBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question, so a harness whose arms disagree is timing two different
// problems. This class carries no generated workload - the parameter IS the input - so the only
// property left to pin is that the arms still answer identically across the parameter range.
public sealed partial class ClimbingStairsBenchmarksTests
{
    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(5)]
    [InlineData(10)]
    [InlineData(25)]
    [InlineData(40)]
    public void MemoizedRecurrence_AgreesWithIterativeRollingTotals(int stepCount)
    {
        var harness = new ClimbingStairsBenchmarks { StepCount = stepCount };

        Assert.Equal(harness.IterativeRollingTotals(), harness.MemoizedRecurrence());
    }
}
