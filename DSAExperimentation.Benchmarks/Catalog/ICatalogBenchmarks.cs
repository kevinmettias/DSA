using DSAExperimentation.LeetCode.Catalog;

namespace DSAExperimentation.Benchmarks.Catalog;

// What a generated catalog benchmark class tells its tests beside its arms: the solution it times, and,
// for each arm after [GlobalSetup], the cases that arm's answers line up with, in order. Both are
// implemented explicitly, so BenchmarkDotNet sees only the arms.
internal interface ICatalogBenchmarks
{
    Type Solution { get; }

    IReadOnlyList<LeetCodeCase> CasesOf(string arm);
}
