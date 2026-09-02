using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.LazySegmentTree;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Handling Sum Queries After Update (LC 2569): re-scanning a mutable bit array on
// every flip and every "add p per current one" query (baseline - the naive
// approach the problem is designed to make too slow) vs. this repo's own
// LazySegmentTree<int,bool,FlipCountOperation> answering both the range flip and
// the current-one-count as O(log n) operations (HandlingSumQueriesAfterUpdateTests'
// own approach, the same "compose the generic lazy engine over a new algebra" move
// FancySequenceBenchmarks' AffineOperation already makes). Query kinds cycle
// through flip/add/read so both strategies pay a representative mixed workload
// instead of one query kind dominating.
[MemoryDiagnoser]
public class HandlingSumQueriesAfterUpdateBenchmarks
{
    private const int RandomSeed = 2569; // LeetCode problem number
    private const int MaxNums2ValueExclusive = 1_000;
    private const int QueryKindModulus = 3;

    [Params(200, 5_000)]
    public int Length;

    private int[] _nums1 = null!;
    private int[] _nums2 = null!;
    private int[][] _queries = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _nums1 = Enumerable.Range(0, Length).Select(_ => random.Next(0, 2)).ToArray();
        _nums2 = Enumerable.Range(0, Length).Select(_ => random.Next(0, MaxNums2ValueExclusive)).ToArray();
        _queries = Enumerable.Range(0, Length).Select(i => BuildQuery(random, i)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public long ArrayRescan()
    {
        var bits = (int[])_nums1.Clone();
        var sum = _nums2.Sum(value => (long)value);
        var lastAnswer = 0L;

        foreach (var query in _queries)
        {
            switch (query[0])
            {
                case 1:
                    for (var i = query[1]; i <= query[2]; i++)
                    {
                        bits[i] ^= 1;
                    }
                    break;
                case 2:
                    sum += (long)query[1] * bits.Sum();
                    break;
                default:
                    lastAnswer = sum;
                    break;
            }
        }

        return lastAnswer;
    }

    [Benchmark]
    public long LazySegmentTreeFlip()
    {
        var ones = new LazySegmentTree<int, bool, FlipCountOperation>(_nums1);
        var sum = _nums2.Sum(value => (long)value);
        var lastAnswer = 0L;

        foreach (var query in _queries)
        {
            switch (query[0])
            {
                case 1:
                    ones.UpdateRange(query[1], query[2], true);
                    break;
                case 2:
                    sum += (long)query[1] * ones.Query(0, Length - 1);
                    break;
                default:
                    lastAnswer = sum;
                    break;
            }
        }

        return lastAnswer;
    }

    private int[] BuildQuery(Random random, int index)
    {
        var kind = index % QueryKindModulus;

        if (kind == 0)
        {
            var left = random.Next(0, Length);
            var right = random.Next(left, Length);
            return [1, left, right];
        }

        return kind == 1 ? [2, random.Next(1, MaxNums2ValueExclusive), 0] : [3, 0, 0];
    }

    private readonly struct FlipCountOperation : IRangeUpdateOperation<int, bool>
    {
        public static int Identity => 0;

        public static bool NoUpdate => false;

        public static int Combine(int left, int right) => left + right;

        public static bool ComposeUpdate(bool outer, bool inner) => outer ^ inner;

        public static int ApplyUpdate(int aggregate, bool update, int rangeLength) => update ? rangeLength - aggregate : aggregate;
    }
}
