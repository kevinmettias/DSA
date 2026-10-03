using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.XOfAKindInADeckOfCards;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are XOfAKindInADeckOfCardsSolution's, the same methods
// XOfAKindInADeckOfCardsSolutionTests proves correct - the BCL's Dictionary tally vs. this
// repo's own HashMap<int, int>, the "same algorithm, BCL structures vs. repo
// structures" contrast HandOfStraightsBenchmarks already draws. DeckSize stops at
// LC 914's 10^4 cards.
public class XOfAKindInADeckOfCardsBenchmarks
{
    private const int GroupSize = 4;
    private const int RandomSeed = 914; private int[] _deck = [];

    // LC problem number

    [Params(400, 10_000)]
    public int DeckSize { get; set; }

    [GlobalSetup]
    public void Setup() =>
        _deck = XOfAKindInADeckOfCardsWorkloads.BuildPartitionableDeck(DeckSize, GroupSize, RandomSeed);

    [Benchmark(Baseline = true)]
    public bool HasGroupsSizeXByDictionaryCount() =>
        XOfAKindInADeckOfCardsSolution.HasGroupsSizeXByDictionaryCount(_deck);

    [Benchmark]
    public bool HasGroupsSizeXByHashMapCount() =>
        XOfAKindInADeckOfCardsSolution.HasGroupsSizeXByHashMapCount(_deck);
}
