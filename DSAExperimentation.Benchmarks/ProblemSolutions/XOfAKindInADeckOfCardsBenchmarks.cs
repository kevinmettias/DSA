using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.XOfAKindInADeckOfCards;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are XOfAKindInADeckOfCardsSolution's, the same methods
// XOfAKindInADeckOfCardsTests proves correct - the BCL's Dictionary tally vs. this
// repo's own HashMap<int, int>, the "same algorithm, BCL structures vs. repo
// structures" contrast HandOfStraightsBenchmarks already draws.
[MemoryDiagnoser]
public class XOfAKindInADeckOfCardsBenchmarks
{
    private const int GroupSize = 4;
    private const int RandomSeed = 914; private int[] _deck = [];

    // LC problem number

    [Params(400, 20_000)]
    public int DeckSize { get; set; }

    [GlobalSetup]
    public void Setup() =>
        _deck = XOfAKindInADeckOfCardsWorkloads.BuildPartitionableDeck(DeckSize, GroupSize, RandomSeed);

    [Benchmark(Baseline = true)]
    public bool DictionaryCount() =>
        XOfAKindInADeckOfCardsSolution.HasGroupsSizeXByDictionaryCount(_deck);

    [Benchmark]
    public bool HashMapCount() =>
        XOfAKindInADeckOfCardsSolution.HasGroupsSizeXByHashMapCount(_deck);
}
