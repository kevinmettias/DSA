using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.NumberOfWaysToWearDifferentHatsToEachOther;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are NumberOfWaysToWearDifferentHatsToEachOtherSolution's,
// the same methods NumberOfWaysToWearDifferentHatsToEachOtherTests proves correct.
// Each is handed the prepared HatPreferences its hoisted overload takes, so
// inverting hats-per-person into people-per-hat is charged to [GlobalSetup] rather
// than to the recursion being measured.
[MemoryDiagnoser]
public class NumberOfWaysToWearDifferentHatsToEachOtherBenchmarks
{
    private const int HatPoolSize = 8;
    private const int LikedHatsPerPerson = 3;

    // LC problem number, reused as the deterministic preference seed.
    private const int RandomSeed = 1434;

    private HatPreferences _preferences = null!;

    [Params(5, 7)]
    public int PeopleCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var likedHats = HatWorkloads.BuildLikedHats(PeopleCount, HatPoolSize, LikedHatsPerPerson, seed: RandomSeed);

        _preferences = HatPreferences.Build(likedHats, HatPoolSize);
    }

    [Benchmark(Baseline = true)]
    public int BruteForceRecursion() =>
        NumberOfWaysToWearDifferentHatsToEachOtherSolution.NumberWaysByBruteForceRecursion(_preferences);

    [Benchmark]
    public int MemoizedRecursion() =>
        NumberOfWaysToWearDifferentHatsToEachOtherSolution.NumberWaysByMemoizedBitmask(_preferences);
}
