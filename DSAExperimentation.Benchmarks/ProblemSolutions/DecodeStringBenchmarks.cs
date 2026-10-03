using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.DecodeString;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are DecodeStringSolution's, the same methods
// DecodeStringSolutionTests proves correct. LC 394 caps the encoded string at 30
// characters, so the larger length is that cap: six "2[ab]" tiles.
public class DecodeStringBenchmarks
{
    private string _encoded = "";

    [Params(10, 30)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup() => _encoded = DecodeStringWorkloads.BuildEncoded(Length);

    [Benchmark(Baseline = true)]
    public string RecursiveDescent() => DecodeStringSolution.DecodeByRecursiveDescent(_encoded);

    [Benchmark]
    public string StackScan() => DecodeStringSolution.DecodeByStackScan(_encoded);
}
