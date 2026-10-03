using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for IteratorForCombinationBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for one question - enumerate every length-c combination of the first n
// letters - and each returns every combination its iterator produced, in order. That the arms
// agree is BenchmarkArmsTests' check; what this pins is independent of both arms: there are
// C(n, c) combinations, and LC 1286 hands them out in lexicographical order.
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

        Assert.Equal(expected, BuildHarness().BitmaskEnumeration().Count);
        Assert.Equal(expected, BuildHarness().BacktrackComposed().Count);
    }

    [Fact]
    public void BitmaskEnumeration_HalfLengthCombinations_DrainsInLexicographicalOrder() =>
        AssertStrictlyAscending(BuildHarness().BitmaskEnumeration());

    [Fact]
    public void BacktrackComposed_HalfLengthCombinations_DrainsInLexicographicalOrder() =>
        AssertStrictlyAscending(BuildHarness().BacktrackComposed());

    private static void AssertStrictlyAscending(List<string> combinations) =>
        Assert.All(
            combinations.Zip(combinations.Skip(1)),
            pair => Assert.True(string.CompareOrdinal(pair.First, pair.Second) < 0, $"{pair.First} before {pair.Second}"));

    private static IteratorForCombinationBenchmarks BuildHarness()
    {
        var harness = new IteratorForCombinationBenchmarks { CharacterCount = SmallestCharacterCount };
        harness.Setup();

        return harness;
    }

    // Pascal's triangle rather than a multiplicative formula, so the expected count is derived
    // here instead of restated from whatever the arms happen to return.
    private static int BinomialCoefficient(int setSize, int chosenCount)
    {
        var row = new int[setSize + 1];
        row[0] = 1;

        for (var step = 0; step < setSize; step++)
        {
            for (var column = step + 1; column > 0; column--)
            {
                row[column] += row[column - 1];
            }
        }

        return row[chosenCount];
    }
}
