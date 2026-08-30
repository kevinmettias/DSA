using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.LazySegmentTree;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Fancy Sequence (LC 1622): re-scanning and rewriting every element on each
// addAll/multAll (baseline - the naive approach the problem is designed to make
// too slow) vs. this repo's own LazySegmentTree<long,(Mult,Add),AffineOperation>
// applying the same affine transform as one O(log n) lazy range-update
// (FallingSquaresBenchmarks' own LazySegmentTree-vs-array-rescan precedent, here
// with an affine op instead of range-assign-max - see FancySequenceTests for
// AffineOperation's own derivation). Every appended value is followed by
// Length alternating addAll/multAll operations across the whole live prefix, then
// every index is read back, so both strategies pay their full workload instead of
// an early exit making the rescan look artificially competitive.
[MemoryDiagnoser]
public class FancySequenceBenchmarks
{
    private const long Modulo = 1_000_000_007;

    [Params(200, 2_000)]
    public int Length;

    private int[] _appendValues = null!;
    private (bool IsMultiply, int Amount)[] _operations = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1622);
        _appendValues = Enumerable.Range(0, Length).Select(_ => random.Next(1, 100)).ToArray();
        _operations = Enumerable.Range(0, Length)
            .Select(i => (IsMultiply: i % 2 == 0, Amount: random.Next(2, 5)))
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public long ArrayRescan()
    {
        var values = new List<long>(Length);

        foreach (var val in _appendValues)
        {
            values.Add(val);
        }

        foreach (var (isMultiply, amount) in _operations)
        {
            for (var i = 0; i < values.Count; i++)
            {
                values[i] = isMultiply ? values[i] * amount % Modulo : (values[i] + amount) % Modulo;
            }
        }

        var sum = 0L;
        foreach (var value in values)
        {
            sum = (sum + value) % Modulo;
        }

        return sum;
    }

    [Benchmark]
    public long LazySegmentTreeAffine()
    {
        var tree = new LazySegmentTree<long, (long Mult, long Add), AffineOperation>(new long[Length]);

        for (var i = 0; i < _appendValues.Length; i++)
        {
            tree.UpdateRange(i, i, (0L, _appendValues[i]));
        }

        foreach (var (isMultiply, amount) in _operations)
        {
            var update = isMultiply ? (Mult: (long)amount, Add: 0L) : (Mult: 1L, Add: (long)amount);
            tree.UpdateRange(0, Length - 1, update);
        }

        var sum = 0L;
        for (var i = 0; i < Length; i++)
        {
            sum = (sum + tree.Query(i, i)) % Modulo;
        }

        return sum;
    }

    private readonly struct AffineOperation : IRangeUpdateOperation<long, (long Mult, long Add)>
    {
        public static long Identity => 0L;

        public static (long Mult, long Add) NoUpdate => (1L, 0L);

        public static long Combine(long left, long right) => (left + right) % Modulo;

        public static (long Mult, long Add) ComposeUpdate((long Mult, long Add) outer, (long Mult, long Add) inner)
            => (outer.Mult * inner.Mult % Modulo, (outer.Mult * inner.Add + outer.Add) % Modulo);

        public static long ApplyUpdate(long aggregate, (long Mult, long Add) update, int rangeLength)
            => (update.Mult * aggregate + update.Add) % Modulo;
    }
}
