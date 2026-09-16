using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.QueueReconstructionByHeight;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are QueueReconstructionByHeightSolution's, the same
// methods QueueReconstructionByHeightTests proves correct. Each arm takes the
// (Height, K) pairs [GlobalSetup] already prepared, so decoding LeetCode's
// int[][] shape is not charged to the measured method - the hoisted overload
// QueueReconstructionByHeightSolution exposes for exactly that. Both arms
// return the reconstructed queue itself (LeetCode's actual answer), not just
// its count.
[MemoryDiagnoser]
public class QueueReconstructionByHeightBenchmarks
{
    private const int RandomSeed = 406;

    private (int Height, int K)[] _people = [];

    [Params(200, 2_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        var heights = Enumerable.Range(0, Length).Select(_ => random.Next(1, Length)).ToArray();

        _people = BuildPeople(random, heights);
    }

    // LC 406's contract is that k counts the people at least as tall as this one standing in front
    // of them, so no k can exceed the number of people at or above that height - exactly the bound
    // both arms' sort-then-Insert(k) relies on. The earlier generator drew k uniformly from
    // [0, height), which carries no such bound: at the smallest Length the tallest person's k was
    // typically near a hundred while only a handful of people shared that height, so every arm
    // threw ArgumentOutOfRangeException on its first insertion and the benchmark measured nothing
    // at all.
    //
    // The pairs are therefore read off a valid arrangement: standing the people in a shuffled
    // non-increasing-height order makes each person's k exactly their position in it, which is by
    // construction the number of people at or above their height standing in front of them.
    private static (int Height, int K)[] BuildPeople(Random random, int[] heights)
    {
        var arrangement = heights
            .Select((height, index) => (height, index))
            .OrderBy(_ => random.Next())
            .OrderByDescending(person => person.height)
            .ToArray();

        var people = new (int Height, int K)[heights.Length];

        for (var position = 0; position < arrangement.Length; position++)
        {
            people[arrangement[position].index] = (arrangement[position].height, position);
        }

        return people;
    }

    [Benchmark(Baseline = true)]
    public int[][] ArraySortListInsert() =>
        QueueReconstructionByHeightSolution.ReconstructQueueByArraySortListInsert(_people);

    [Benchmark]
    public int[][] MergeSortDynamicArrayInsert() =>
        QueueReconstructionByHeightSolution.ReconstructQueueByMergeSortDynamicArrayInsert(_people);
}
