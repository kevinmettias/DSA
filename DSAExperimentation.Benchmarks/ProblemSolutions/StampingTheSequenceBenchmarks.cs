using DSAExperimentation.LeetCode.StampingTheSequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are StampingTheSequenceSolution's, the same methods
// StampingTheSequenceSolutionTests proves correct. The target is the stamp repeated, so the
// reverse simulation discovers one stamp per repeat and the only thing separating
// the arms is how each discovered index is put back into forward order:
// List<int>.Insert(0, i) is O(k) per insertion and O(m^2) over m discovered stamps,
// while Stack<int> records in O(1) and unwinds once at the end. Repeats stops at 250,
// whose 1,000 characters are LC 936's longest target, so m never passes 250: the
// stack arm's higher per-call constant cost through DynamicArray is set against a
// quadratic term that has little room to grow inside the bound.
public class StampingTheSequenceBenchmarks
{
    private const string Stamp = "abcd";

    private string _target = string.Empty;

    [Params(200, 250)]
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
