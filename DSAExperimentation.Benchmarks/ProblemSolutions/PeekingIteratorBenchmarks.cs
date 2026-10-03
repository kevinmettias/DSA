using DSAExperimentation.LeetCode.PeekingIterator;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are PeekingIteratorSolution's, the same factories
// PeekingIteratorSolutionTests proves correct. Both drive the same peek-then-next
// pattern to full exhaustion, returning every Peek and Next answer in call order.
//
// The values are 1..Length, as LC 284's are at least 1. Length stops at 333: the drain
// makes a HasNext, a Peek and a Next per element plus one final HasNext, 1,000 calls in
// all, LC 284's cap.
public class PeekingIteratorBenchmarks
{
    // One Peek and one Next per element.
    private const int AnswersPerElement = 2;

    private int[] _values = [];

    private int[] _answers = [];

    [Params(200, 333)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _values = Enumerable.Range(1, Length).ToArray();
        _answers = new int[_values.Length * AnswersPerElement];
    }

    [Benchmark(Baseline = true)]
    public int[] IndexTracked() => Drain(PeekingIteratorSolution.CreateByIndexTracked(_values));

    [Benchmark]
    public int[] QueuePrimitive() => Drain(PeekingIteratorSolution.CreateByQueuePrimitive(_values));

    private int[] Drain(IPeekingIterator iterator)
    {
        var answered = 0;

        while (iterator.HasNext())
        {
            _answers[answered++] = iterator.Peek();
            _answers[answered++] = iterator.Next();
        }

        return _answers;
    }
}
