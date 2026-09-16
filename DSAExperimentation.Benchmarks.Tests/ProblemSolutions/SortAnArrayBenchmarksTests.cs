using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SortAnArrayBenchmarks (ARCHITECTURE 17.9): both arms are
// SortAnArraySolution's sorts of the same randomized input, each on its own copy, so a harness
// whose arms disagree is timing two different problems. Setup is a pure function of Length and
// its own fixed seed, so the same Length must rebuild the same values.
//
// The arms return the sorted array itself, so the order-sensitive rendering compares the whole
// answer rather than a count of it. Agreement alone would still hold if both arms returned their
// input untouched, so each arm's result is also asserted ascending - the property the problem
// asks for that agreement cannot witness on its own.
public sealed partial class SortAnArrayBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().InsertionSort()),
            AnswerText.Of(BuildHarness().InsertionSort()));

    [Fact]
    public void InsertionSort_TwoHundredValues_AgreesWithMergeSortAscending()
    {
        var harness = BuildHarness();

        Assert.True(IsAscending(harness.InsertionSort()));
        Assert.Equal(AnswerText.Of(harness.MergeSortAscending()), AnswerText.Of(harness.InsertionSort()));
    }

    [Fact]
    public void MergeSortAscending_TwoHundredValues_AgreesWithInsertionSort()
    {
        var harness = BuildHarness();

        Assert.True(IsAscending(harness.MergeSortAscending()));
        Assert.Equal(AnswerText.Of(harness.InsertionSort()), AnswerText.Of(harness.MergeSortAscending()));
    }

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
