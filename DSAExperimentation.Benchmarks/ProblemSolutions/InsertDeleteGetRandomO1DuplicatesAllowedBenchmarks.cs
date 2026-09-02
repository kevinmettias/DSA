using BenchmarkDotNet.Attributes;
using static DSAExperimentation.LeetCode.InsertDeleteGetRandomO1DuplicatesAllowed.InsertDeleteGetRandomO1DuplicatesAllowedSolution;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are InsertDeleteGetRandomO1DuplicatesAllowedSolution's, the
// same classes InsertDeleteGetRandomO1DuplicatesAllowedTests proves correct. A Design
// problem's whole point is a sequence of mutating calls against one instance, so there
// is no separate "prepare input" step to hoist into [GlobalSetup] beyond the
// insert/removal order arrays themselves - [GlobalSetup] builds those (so shuffling
// isn't charged to the measured method) and each [Benchmark] arm constructs its own
// instance and replays the same insert-then-remove script, returning the surviving
// Count so the JIT can't eliminate the replay as dead code. _insertOrder is drawn from
// a narrow value range so most values collide with earlier ones, forcing real
// duplicate chains for Remove to walk past in the ListScan arm instead of an
// artificially duplicate-free input.
[MemoryDiagnoser]
public class InsertDeleteGetRandomO1DuplicatesAllowedBenchmarks
{
    // A small, fixed value range - not scaled with Count - so the average duplicate
    // cluster size grows linearly with Count instead of staying constant, which is
    // what makes the ListScan arm's per-removal IndexOf scan genuinely O(Count) (and
    // the whole removal phase O(Count^2)) rather than an accidentally-cheap O(1)-ish
    // scan.
    private const int DistinctValues = 10;

    [Params(200, 20_000)]
    public int Count;

    private int[] _insertOrder = null!;
    private int[] _removalOrder = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);

        _insertOrder = Enumerable.Range(0, Count).Select(_ => random.Next(DistinctValues)).ToArray();
        _removalOrder = (int[])_insertOrder.Clone();
        Shuffle(_removalOrder, random);
    }

    [Benchmark(Baseline = true)]
    public int ListScan() => Replay(new RandomizedCollectionByListScan());

    [Benchmark]
    public int LinkedOccurrences() => Replay(new RandomizedCollectionByLinkedOccurrences());

    private int Replay(IRandomizedCollection collection)
    {
        foreach (var value in _insertOrder)
        {
            collection.Insert(value);
        }

        foreach (var value in _removalOrder)
        {
            collection.Remove(value);
        }

        return collection.Count;
    }

    private static void Shuffle(int[] items, Random random)
    {
        for (var i = items.Length - 1; i > 0; i--)
        {
            var j = random.Next(i + 1);
            (items[i], items[j]) = (items[j], items[i]);
        }
    }
}
