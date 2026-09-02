using BenchmarkDotNet.Attributes;
using RepoQueue = DSAExperimentation.DataStructures.Queue.Queue<int>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Reveal Cards In Increasing Order (LC 950): the naive simulation most people reach for
// first - a List<int> of indices with RemoveAt(0) standing in for "pop the front" - is
// O(n) per removal, O(n^2) total, vs. this repo's own Queue<int> (Deque-backed, O(1)
// amortized push/pop at both ends) driving the identical reveal/move-to-bottom
// simulation in O(n). Both pay the same O(n log n) sort up front, so the asymmetry shows
// up entirely in the simulation loop itself.
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
    public int[] ListRemoveAtSimulation()
    {
        var sorted = SortedDeck();
        var indices = BuildIndexList(_deck.Length);
        var result = new int[_deck.Length];

        foreach (var value in sorted)
        {
            RevealAndRotate(indices, result, value);
        }

        return result;
    }

    [Benchmark]
    public int[] QueueSimulation()
    {
        var sorted = SortedDeck();
        var indices = BuildIndexQueue(_deck.Length);
        var result = new int[_deck.Length];

        foreach (var value in sorted)
        {
            RevealAndRotate(indices, result, value);
        }

        return result;
    }

    private int[] SortedDeck()
    {
        var sorted = (int[])_deck.Clone();
        Array.Sort(sorted);
        return sorted;
    }

    private static List<int> BuildIndexList(int length)
    {
        var indices = new List<int>(length);

        for (var i = 0; i < length; i++)
        {
            indices.Add(i);
        }

        return indices;
    }

    private static void RevealAndRotate(List<int> indices, int[] result, int value)
    {
        var revealIndex = indices[0];
        indices.RemoveAt(0);
        result[revealIndex] = value;

        if (indices.Count > 0)
        {
            var moveToBottom = indices[0];
            indices.RemoveAt(0);
            indices.Add(moveToBottom);
        }
    }

    private static RepoQueue BuildIndexQueue(int length)
    {
        var indices = new RepoQueue();

        for (var i = 0; i < length; i++)
        {
            indices.Enqueue(i);
        }

        return indices;
    }

    private static void RevealAndRotate(RepoQueue indices, int[] result, int value)
    {
        indices.TryDequeue(out var revealIndex);
        result[revealIndex] = value;

        if (indices.TryDequeue(out var moveToBottom))
        {
            indices.Enqueue(moveToBottom);
        }
    }
}
