using DSAExperimentation.LeetCode.RevealCardsInIncreasingOrder;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are RevealCardsInIncreasingOrderSolution's, the same methods
// RevealCardsInIncreasingOrderSolutionTests proves correct. The deck is generated once in
// [GlobalSetup]; each arm still does its own sort, because the sort is part of the
// strategy and both pay the same O(n log n) for it - the difference measured here is
// List<int>.RemoveAt(0)'s O(n) front removal against Queue<int>'s O(1) amortized one.
// Length stops at LC 950's 1,000 cards, and since LC 950's values are unique, a value
// drawn a second time is drawn again rather than dealt twice.
public class RevealCardsInIncreasingOrderBenchmarks
{
    private const int RandomSeed = 950; // LC problem number
    private const int CardValueUpperBound = 1_000_000;

    private int[] _deck = [];

    [Params(200, 1_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        var dealt = new HashSet<int>();
        var deck = new List<int>(Length);

        while (deck.Count < Length)
        {
            var card = random.Next(1, CardValueUpperBound);

            if (dealt.Add(card))
            {
                deck.Add(card);
            }
        }

        _deck = [.. deck];
    }

    [Benchmark(Baseline = true)]
    public int[] ListRemoveAtSimulation() =>
        RevealCardsInIncreasingOrderSolution.DeckRevealedIncreasingByListRemoveAt(_deck);

    [Benchmark]
    public int[] QueueSimulation() =>
        RevealCardsInIncreasingOrderSolution.DeckRevealedIncreasingByQueueRotation(_deck);
}
