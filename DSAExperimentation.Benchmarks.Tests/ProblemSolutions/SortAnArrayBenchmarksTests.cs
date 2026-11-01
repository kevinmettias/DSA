using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SortAnArrayBenchmarks (ARCHITECTURE 17.9): the generic arm check holds
// MergeSortAscending to the baseline's answer, but agreement alone would still hold if both arms
// returned their input untouched, so the baseline's result is asserted ascending - the property the
// problem asks for that agreement cannot witness on its own.
public sealed partial class SortAnArrayBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void InsertionSort_TwoHundredValues_SortsAscending() =>
        Assert.True(IsAscending(BuildHarness().InsertionSort()));

    private static SortAnArrayBenchmarks BuildHarness()
    {
        var harness = new SortAnArrayBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }

    private static bool IsAscending(int[] values)
    {
        for (var i = 1; i < values.Length; i++)
        {
            if (values[i - 1] > values[i])
            {
                return false;
            }
        }

        return true;
    }
}
