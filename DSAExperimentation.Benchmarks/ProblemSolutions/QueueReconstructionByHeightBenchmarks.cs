using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.DynamicArray;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Queue Reconstruction by Height (LC 406): both strategies run the same
// sort-tallest-first-then-insert-at-k algorithm - this problem's insight (why
// descending height makes the insert index always valid) doesn't leave room
// for a differently-shaped naive alternative the way HIndex/
// RussianDollEnvelopes do. SortThenListInsert uses BCL Array.Sort and
// List<T>.Insert; SortThenDynamicArrayInsert instead composes this repo's own
// MergeSort.Sort<Element,TSequence> and DynamicArray<Element>.Insert, the same
// pair QueueReconstructionByHeightTests.cs exercises for correctness.
[MemoryDiagnoser]
public class QueueReconstructionByHeightBenchmarks
{
    private const int RandomSeed = 406;

    [Params(200, 2_000)]
    public int Length;

    private (int Height, int K)[] _people = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _people = Enumerable.Range(0, Length)
            .Select(_ =>
            {
                var height = random.Next(1, Length);
                return (Height: height, K: random.Next(0, height));
            })
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public int SortThenListInsert()
    {
        var items = ((int Height, int K)[])_people.Clone();
        Array.Sort(items, (a, b) => a.Height != b.Height ? b.Height.CompareTo(a.Height) : a.K.CompareTo(b.K));

        var queue = new List<(int Height, int K)>();

        foreach (var person in items)
        {
            queue.Insert(person.K, person);
        }

        return queue.Count;
    }

    [Benchmark]
    public int SortThenDynamicArrayInsert()
    {
        var items = ((int Height, int K)[])_people.Clone();
        var byHeightDescendingThenKAscending = Comparer<(int Height, int K)>.Create(
            (a, b) => a.Height != b.Height ? b.Height.CompareTo(a.Height) : a.K.CompareTo(b.K));

        MergeSort.Sort<(int Height, int K), ArrayIndexedSequence<(int Height, int K)>>(
            new ArrayIndexedSequence<(int Height, int K)>(items), byHeightDescendingThenKAscending);

        var queue = new DynamicArray<(int Height, int K)>();

        foreach (var person in items)
        {
            queue.Insert(person.K, person);
        }

        return queue.Count;
    }
}
