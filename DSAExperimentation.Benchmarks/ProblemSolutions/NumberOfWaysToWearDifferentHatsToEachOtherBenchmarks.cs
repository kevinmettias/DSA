using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Number of Ways to Wear Different Hats to Each Other (LC 1434): the textbook
// unmemoized bitmask recursion over (hat, peopleAssignedMask) - the same
// (hat, mask) pair re-explored from scratch down every branch, since many
// different hat orderings reach the identical state - vs. the same recursion
// routed through this repo's own Memoizer, using the tuple-state shape its own
// doc comment names as the intended use case. Every person likes a growing
// number of random hats from a small shared pool so the branching factor (and
// therefore state-revisit count) actually grows with PeopleCount, forcing a
// real gap between the two strategies - the same "force the real worst case"
// convention CanIWinBenchmarks already uses.
[MemoryDiagnoser]
public class NumberOfWaysToWearDifferentHatsToEachOtherBenchmarks
{
    private const int Modulo = 1_000_000_007;
    private const int MaxHat = 8;
    private const int LikedHatsPerPerson = 3;

    [Params(5, 7)]
    public int PeopleCount;

    private List<int>[] _hatToPeople = null!;
    private int _fullMask;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1434);
        _hatToPeople = new List<int>[MaxHat + 1];
        for (var hat = 1; hat <= MaxHat; hat++)
        {
            _hatToPeople[hat] = [];
        }

        for (var person = 0; person < PeopleCount; person++)
        {
            var liked = Enumerable.Range(1, MaxHat).OrderBy(_ => random.Next()).Take(LikedHatsPerPerson);
            foreach (var hat in liked)
            {
                _hatToPeople[hat].Add(person);
            }
        }

        _fullMask = (1 << PeopleCount) - 1;
    }

    [Benchmark(Baseline = true)]
    public long BruteForceRecursion() => CountWays(MaxHat, _fullMask);

    private long CountWays(int hat, int mask)
    {
        if (mask == 0)
        {
            return 1;
        }

        if (hat == 0)
        {
            return 0;
        }

        var total = CountWays(hat - 1, mask);

        foreach (var person in _hatToPeople[hat])
        {
            var bit = 1 << person;
            if ((mask & bit) != 0)
            {
                total = (total + CountWays(hat - 1, mask & ~bit)) % Modulo;
            }
        }

        return total;
    }

    [Benchmark]
    public long MemoizedRecursion() => Memoizer.Memoize<(int Hat, int Mask), long>((MaxHat, _fullMask), (state, ways) =>
    {
        var (hat, mask) = state;

        if (mask == 0)
        {
            return 1L;
        }

        if (hat == 0)
        {
            return 0L;
        }

        var total = ways((hat - 1, mask));

        foreach (var person in _hatToPeople[hat])
        {
            var bit = 1 << person;
            if ((mask & bit) != 0)
            {
                total = (total + ways((hat - 1, mask & ~bit))) % Modulo;
            }
        }

        return total;
    });
}
