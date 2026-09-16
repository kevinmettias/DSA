using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for IteratorForCombinationBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for one question - enumerate every length-c combination of the first n
// letters - so a harness whose arms disagree is timing two different problems.
//
// WEAK BY CONSTRUCTION: both arms are drained down to an int because the iterator type is
// internal and a public [Benchmark] cannot return one, so what the arms report is the NUMBER of
// combinations they produced, not which combinations. That number is C(n, c) for either strategy
// whatever order it emits in, so agreement here witnesses "both walked a full combination space
// of the right size" and nothing about the two orders matching. Asserting the honest thing; the
// proxy is in the benchmark's return type, which a harness test may not change.
public sealed partial class IteratorForCombinationBenchmarksTests
{
    private const int SmallestCharacterCount = 10;

    // Setup takes half the alphabet as the combination length, so the expected count is the
    // central binomial coefficient C(n, n/2) - 252 at n = 10.
    private const int SmallestCombinationLength = SmallestCharacterCount / 2;

    [Fact]
    public void Setup_SmallestCharacterCount_DrainsTheCentralBinomialCount()
    {
        var expected = BinomialCoefficient(SmallestCharacterCount, SmallestCombinationLength);

        Assert.Equal(expected, BuildHarness().BitmaskEnumeration());
        Assert.Equal(expected, BuildHarness().BacktrackComposed());
    }

    [Fact]
    public void BitmaskEnumeration_HalfLengthCombinations_AgreesWithBacktrackComposed()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BacktrackComposed(), harness.BitmaskEnumeration());
    }

    [Fact]
    public void BacktrackComposed_HalfLengthCombinations_AgreesWithBitmaskEnumeration()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BitmaskEnumeration(), harness.BacktrackComposed());
    }

    private static IteratorForCombinationBenchmarks BuildHarness()
    {
        var harness = new IteratorForCombinationBenchmarks { CharacterCount = SmallestCharacterCount };
        harness.Setup();

        return harness;
    }

    // Pascal's triangle rather than a multiplicative formula, so the expected count is derived
    // here instead of restated from whatever the arms happen to return.
    private static int BinomialCoefficient(int n, int k)
    {
        var row = new int[n + 1];
        row[0] = 1;

        for (var step = 0; step < n; step++)
        {
            for (var column = step + 1; column > 0; column--)
            {
                row[column] += row[column - 1];
            }
        }

        return row[k];
    }
}
