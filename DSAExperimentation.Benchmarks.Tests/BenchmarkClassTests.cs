using BenchmarkDotNet.Attributes;

namespace DSAExperimentation.Benchmarks.Tests;

// BenchmarkArmsTests compares arms only as well as BenchmarkClass picks the workload they are
// compared on. For arms sized per arm - an exponential baseline that stops early beside a
// polynomial arm that runs on - that is the smallest size they all run, and a class whose arms
// share no size must fail loudly rather than be compared across two workloads.
public sealed partial class BenchmarkClassTests
{
    [Fact]
    public void Run_ArmsSizedPerArm_CallsEachWithTheSmallestSizeTheyShare()
    {
        var benchmark = BenchmarkClass.Of(typeof(PerArmSizes));

        var answers = benchmark.Arms.Select(arm => benchmark.Run(benchmark.Prepare(arm), arm)).ToList();

        Assert.Equal([5, 5], answers);
    }

    [Fact]
    public void ArgumentsOf_ArgumentsSourceArm_ReadsTheSourceMember() =>
        Assert.Equal([5, 9, 200], BenchmarkClass.Of(typeof(PerArmSizes)).ArgumentsOf(typeof(PerArmSizes).GetMethod(nameof(PerArmSizes.Fast))!));

    [Fact]
    public void Run_ArmsSharingNoSize_RefusesToCompareThem()
    {
        var benchmark = BenchmarkClass.Of(typeof(DisjointSizes));
        var arm = benchmark.Arms[0];

        Assert.Throws<InvalidOperationException>(() => benchmark.Run(benchmark.Prepare(arm), arm));
    }

    public sealed class PerArmSizes
    {
        public static IEnumerable<int> FastSizes => [5, 9, 200];

        [Benchmark(Baseline = true)]
        [Arguments(5)]
        [Arguments(9)]
        public int Slow(int size) => size;

        [Benchmark]
        [ArgumentsSource(nameof(FastSizes))]
        public int Fast(int size) => size;
    }

    public sealed class DisjointSizes
    {
        [Benchmark(Baseline = true)]
        [Arguments(5)]
        public int Slow(int size) => size;

        [Benchmark]
        [Arguments(9)]
        public int Fast(int size) => size;
    }
}
