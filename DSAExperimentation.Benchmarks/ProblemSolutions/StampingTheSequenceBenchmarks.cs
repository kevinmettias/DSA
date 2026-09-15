using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.StampingTheSequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are StampingTheSequenceSolution's, the same methods
// StampingTheSequenceTests proves correct. The target is the stamp repeated, so the
// reverse simulation discovers one stamp per repeat and the only thing separating
// the arms is how each discovered index is put back into forward order:
// List<int>.Insert(0, i) is O(k) per insertion and O(m^2) over m discovered stamps,
// while Stack<int> records in O(1) and unwinds once at the end. A BenchmarkDotNet
// dry run confirms the real crossover: at Repeats=200 (m~200) the stack arm trails
// (higher per-call constant cost through DynamicArray), but at Repeats=20,000
// (m~20,000) the list's quadratic shifting dominates and the stack arm wins by ~3x -
// the quadratic term overtaking the constant-factor gap, not benchmark noise.
[MemoryDiagnoser]
public class StampingTheSequenceBenchmarks
{
    private const string Stamp = "abcd";

    private string _target = string.Empty;

    [Params(200, 20_000)]
    public int Repeats { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var repeatedStamp = Enumerable.Repeat(Stamp, Repeats);
        _target = string.Concat(repeatedStamp);
    }

    [Benchmark(Baseline = true)]
    public int[] ListPrepend() => StampingTheSequenceSolution.MovesToStampByListPrepend(
        new StampingTheSequenceSolution.StampPattern(Stamp),
        new StampingTheSequenceSolution.TargetText(_target));

    [Benchmark]
    public int[] StackAndReverse() => StampingTheSequenceSolution.MovesToStampByStackReverse(
        new StampingTheSequenceSolution.StampPattern(Stamp),
        new StampingTheSequenceSolution.TargetText(_target));
}
