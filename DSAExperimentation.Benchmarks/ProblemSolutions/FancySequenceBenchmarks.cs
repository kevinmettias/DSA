using BenchmarkDotNet.Attributes;
using DSAExperimentation.Domain.Modular;
using DSAExperimentation.LeetCode.FancySequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are FancySequenceSolution's, the same factories
// FancySequenceTests proves correct - re-scanning and rewriting every element on each
// addAll/multAll (baseline, the naive approach the problem is designed to make too
// slow) vs. this repo's own LazySegmentTree applying the same affine transform as one
// O(log n) lazy range-update (FallingSquaresBenchmarks' own LazySegmentTree-vs-rescan
// precedent, here with an affine op instead of range-assign-max). [GlobalSetup] builds
// the fixed workload - Length values to append, then Length alternating
// addAll/multAll operations across the whole live prefix - so workload construction is
// charged to setup and only the replay is measured. Every index is read back
// afterwards, so both strategies pay their full workload instead of an early exit
// making the rescan look artificially competitive.
[MemoryDiagnoser]
public class FancySequenceBenchmarks
{
    private const int RandomSeed = 1622; // LC problem number
    private const int AppendedValueUpperBoundExclusive = 100;
    private const int AlternatingParityModulus = 2;
    private const int MinOperationAmount = 2;
    private const int MaxOperationAmountExclusive = 5;

    private int[] _appendValues = [];

    private (bool IsMultiply, int Amount)[] _operations = [];
    [Params(200, 2_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _appendValues = Enumerable.Range(0, Length).Select(_ => random.Next(1, AppendedValueUpperBoundExclusive)).ToArray();
        _operations = Enumerable.Range(0, Length)
            .Select(i => (IsMultiply: i % AlternatingParityModulus == 0, Amount: random.Next(MinOperationAmount, MaxOperationAmountExclusive)))
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public long ArrayRescan() => Replay(FancySequenceSolution.CreateByArrayRescan(Length));

    [Benchmark]
    public long LazySegmentTreeAffine() => Replay(FancySequenceSolution.CreateByLazySegmentTreeAffine(Length));

    private long Replay(FancySequenceSolution.IFancySequence fancy)
    {
        AppendAll(fancy);
        ReplayOperations(fancy);

        var sum = 0L;

        for (var i = 0; i < Length; i++)
        {
            sum = (sum + fancy.GetIndex(i)) % ModularArithmetic.Modulo;
        }

        return sum;
    }

    // The fixed append workload, in order.
    private void AppendAll(FancySequenceSolution.IFancySequence fancy)
    {
        foreach (var value in _appendValues)
        {
            fancy.Append(value);
        }
    }

    // The fixed addAll/multAll workload, in order.
    private void ReplayOperations(FancySequenceSolution.IFancySequence fancy)
    {
        foreach (var (isMultiply, amount) in _operations)
        {
            if (isMultiply)
            {
                fancy.MultAll(amount);
            }
            else
            {
                fancy.AddAll(amount);
            }
        }
    }
}
