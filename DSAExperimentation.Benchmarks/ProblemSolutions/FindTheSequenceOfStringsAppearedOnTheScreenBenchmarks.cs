using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.FindTheSequenceOfStringsAppearedOnTheScreen;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are FindTheSequenceOfStringsAppearedOnTheScreenSolution's.
// An all-'z' target forces every character to roll the full 25-step 'a'->'z'
// range, the worst case for both screen buffers.
[MemoryDiagnoser]
public class FindTheSequenceOfStringsAppearedOnTheScreenBenchmarks
{
    private string _target = "";

    [Params(100, 400)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup() => _target = new string('z', Length);

    [Benchmark(Baseline = true)]
    public List<string> StringBuilderWalk() =>
        FindTheSequenceOfStringsAppearedOnTheScreenSolution.StringSequenceByStringBuilder(_target);

    [Benchmark]
    public List<string> GrowableBufferWalk() =>
        FindTheSequenceOfStringsAppearedOnTheScreenSolution.StringSequenceByGrowableBuffer(_target);
}
