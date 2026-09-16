using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.AddBinary;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are AddBinarySolution's. All-ones operands force a carry
// out of every digit position, so the result always grows by one and neither
// strategy gets to stop early.
[MemoryDiagnoser]
public class AddBinaryBenchmarks
{
    private string _firstOperand = "";

    private string _secondOperand = "";
    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _firstOperand = new string('1', Length);
        _secondOperand = new string('1', Length);
    }

    [Benchmark(Baseline = true)]
    public string CharArrayReverse() => AddBinarySolution.AddByCharArrayReverse(_firstOperand, _secondOperand);

    [Benchmark]
    public string StackBits() => AddBinarySolution.AddByBitStack(_firstOperand, _secondOperand);
}
