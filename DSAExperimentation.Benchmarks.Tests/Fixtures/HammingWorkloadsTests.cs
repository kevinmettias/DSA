using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Graph.Hamming;
using DSAExperimentation.Domain.Science.Dna;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for HammingWorkloads (ARCHITECTURE 17.7). The reading depends on a walk of
// single-character mutations over the DNA alphabet, starting from one fixed value and ending at a
// value a genuine shortest transformation sequence exists for, so every strategy does real BFS work
// instead of failing fast - with each word listed once, as LC 126/127 promise.
public sealed partial class HammingWorkloadsTests
{
    private const int ValueCount = 64;
    private const int ValueLength = 8; // LC 433's and LC 127's shared gene/word length
    private const int Seed = 433; // LC problem number
    private const int MutationCount = 1;
    private const int ChainStart = 0;

    // A walk over DNA strings revisits a word now and then, so the list can be shorter than the
    // steps taken - never longer - and both chain ends are still in it.
    [Fact]
    public void BuildChain_ValueCount_ListsAtMostOneValuePerStepAndKeepsTheChainEnds()
    {
        var (values, first, last) =
            HammingWorkloads.BuildChain<DnaBases>(ValueCount, ValueLength, Seed);

        Assert.InRange(values.Length, MutationCount + 1, ValueCount);
        Assert.Equal(values[ChainStart], first);
        Assert.Contains(last, values);
    }

    [Fact]
    public void BuildChain_EveryValue_IsListedOnce() =>
        Assert.Equal(
            BuildChain().Length,
            BuildChain().Distinct().Count());

    [Fact]
    public void BuildChain_EveryValue_StaysOnTheAlphabetAtTheRequestedLength()
    {
        var (values, _, _) = HammingWorkloads.BuildChain<DnaBases>(ValueCount, ValueLength, Seed);

        Assert.All(values, value => Assert.Equal(ValueLength, value.Length));
        Assert.All(
            values,
            value => Assert.All(value, character => Assert.Contains(character, DnaBases.Letters)));
    }

    // Each word entered the list as one mutation of the word the walk stood on, whose first copy is
    // earlier in the list, so every word after the first is one position away from some earlier word:
    // that is what keeps a real transformation sequence from the first word to every other.
    [Fact]
    public void BuildChain_EveryLaterValue_IsOneMutationFromAnEarlierValue()
    {
        var values = BuildChain();

        foreach (var index in Enumerable.Range(1, values.Length - 1))
        {
            Assert.Contains(values[..index], earlier => DifferingPositions(earlier, values[index]) == MutationCount);
        }
    }

    // Each step draws from the alphabet with the replaced letter taken out, so over a long chain every
    // letter is written somewhere; a draw that kept the replaced letter in would never reach the last.
    [Fact]
    public void BuildChain_EveryLetterOfTheAlphabet_IsWrittenSomewhere() =>
        Assert.All(DnaBases.Letters, letter => Assert.Contains(BuildChain(), value => value.Contains(letter)));

    [Fact]
    public void BuildChain_SameSeed_ReturnsTheSameChain()
    {
        var (values, first, last) = HammingWorkloads.BuildChain<DnaBases>(ValueCount, ValueLength, Seed);
        var (repeatValues, repeatFirst, repeatLast) =
            HammingWorkloads.BuildChain<DnaBases>(ValueCount, ValueLength, Seed);

        Assert.Equal(values, repeatValues);
        Assert.Equal(first, repeatFirst);
        Assert.Equal(last, repeatLast);
    }

    private static string[] BuildChain() =>
        HammingWorkloads.BuildChain<DnaBases>(ValueCount, ValueLength, Seed).Values;

    private static int DifferingPositions(string left, string right) =>
        left.Where((character, position) => character != right[position]).Count();
}
