using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.RevealCardsInIncreasingOrder;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are RevealCardsInIncreasingOrderSolution's, the same methods
// RevealCardsInIncreasingOrderTests proves correct. The deck is generated once in
// [GlobalSetup]; each arm still does its own sort, because the sort is part of the
// strategy and both pay the same O(n log n) for it - the difference measured here is
// List<int>.RemoveAt(0)'s O(n) front removal against Queue<int>'s O(1) amortized one.
[MemoryDiagnoser]
public class RevealCardsInIncreasingOrderBenchmarks
{
    private const int RandomSeed = 950; // LC problem number
    private const int CardValueUpperBound = 1_000_000;

    [Params(200, 5_000)]
    public int Length;

    private int[] _deck = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _deck = Enumerable.Range(0, Length).Select(_ => random.Next(1, CardValueUpperBound)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int[] ListRemoveAtSimulation() =>
        RevealCardsInIncreasingOrderSolution.DeckRevealedIncreasingByListRemoveAt(_deck);

    [Benchmark]
    public int[] QueueSimulation() =>
        RevealCardsInIncreasingOrderSolution.DeckRevealedIncreasingByQueueRotation(_deck);
}
