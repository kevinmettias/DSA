using BenchmarkDotNet.Attributes;

using DSAExperimentation.LeetCode.Harness;

namespace DSAExperimentation.Benchmarks;

// THE benchmark harness: one [Benchmark] for every (problem, strategy, workload)
// any registration declared, in place of a hand-written class per problem.
//
// The arm is a string parameter rather than a rich object because BenchmarkDotNet
// has to print it, use it in result filenames and round-trip it through
// --filter - and because LeetCodeArm is internal, which a public [ParamsSource]
// property cannot expose. Resolution happens once in [GlobalSetup], so the
// measured call is a single delegate invocation over an already-built input: no
// dictionary lookup, no parsing, no input construction inside the timed region.
//
// Run one problem with BenchmarkDotNet's own filter rather than by editing this
// file, e.g. --filter "*LeetCodeProblemBenchmarks*two-sum*".
[MemoryDiagnoser]
public class LeetCodeProblemBenchmarks
{
    private const char ArmSeparator = '/';
    private const int ArmPartCount = 3;

    private Func<object?> _run = () => null;

    public static IEnumerable<string> Arms
        => LeetCodeProblemRegistry.WorkloadArms().Select(arm => arm.ToString());

    [ParamsSource(nameof(Arms))]
    public string Arm { get; set; } = string.Empty;

    [GlobalSetup]
    public void Setup()
    {
        var parts = Arm.Split(ArmSeparator);

        if (parts.Length != ArmPartCount)
        {
            throw new InvalidOperationException($"'{Arm}' is not a problem/strategy/workload arm.");
        }

        _run = LeetCodeProblemRegistry.Get(parts[0]).BindWorkload(parts[1], parts[2]);
    }

    [Benchmark]
    public object? Run() => _run();
}
