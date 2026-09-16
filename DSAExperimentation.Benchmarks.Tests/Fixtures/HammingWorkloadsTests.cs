using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Graph.Hamming;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for HammingWorkloads (ARCHITECTURE 17.7). The reading depends on a chain of
// single-character mutations over the DNA alphabet, starting from one fixed value and ending at a
// value a genuine shortest transformation sequence exists for, so every strategy does real BFS work
// instead of failing fast.
public sealed partial class HammingWorkloadsTests
{
    private const int ValueCount = 64;
    private const int ValueLength = 8; // LC 433's and LC 127's shared gene/word length
    private const int Seed = 433; // LC problem number
    private const int MutationCount = 1;
    private const int ChainStart = 0;

    [Fact]
    public void BuildChain_ValueCount_ReturnsOneValuePerStepAndPinsTheChainEnds()
    {
        var (values, first, last) =
            HammingWorkloads.BuildChain(ValueCount, ValueLength, StandardAlphabets.Dna, Seed);

        Assert.Equal(ValueCount, values.Length);
        Assert.Equal(values[ChainStart], first);
        Assert.Equal(values[^MutationCount], last);
    }

    [Fact]
    public void BuildChain_EveryValue_StaysOnTheAlphabetAtTheRequestedLength()
    {
        var (values, _, _) = HammingWorkloads.BuildChain(ValueCount, ValueLength, StandardAlphabets.Dna, Seed);

        Assert.All(values, value => Assert.Equal(ValueLength, value.Length));
        Assert.All(
            values,
            value => Assert.All(value, character => Assert.Contains(character, StandardAlphabets.Dna.Characters)));
    }

    // One position mutated per step is the chain's whole claim: consecutive values differ in exactly
    // one position, which is what makes the step a single-character mutation and keeps a shortest
    // transformation sequence between the two ends a real one.
    [Fact]
    public void BuildChain_EveryConsecutivePair_DiffersInExactlyOnePosition()
    {
        var (values, _, _) = HammingWorkloads.BuildChain(ValueCount, ValueLength, StandardAlphabets.Dna, Seed);

        foreach (var step in Enumerable.Range(1, ValueCount - 1))
        {
            Assert.Equal(MutationCount, DifferingPositions(values[step - 1], values[step]));
        }
    }

    [Fact]
    public void BuildChain_SameSeed_ReturnsTheSameChain()
    {
        var (values, first, last) = HammingWorkloads.BuildChain(ValueCount, ValueLength, StandardAlphabets.Dna, Seed);
        var (repeatValues, repeatFirst, repeatLast) =
            HammingWorkloads.BuildChain(ValueCount, ValueLength, StandardAlphabets.Dna, Seed);

        Assert.Equal(values, repeatValues);
        Assert.Equal(first, repeatFirst);
        Assert.Equal(last, repeatLast);
    }

    private static int DifferingPositions(string left, string right) =>
        left.Where((character, position) => character != right[position]).Count();
}
