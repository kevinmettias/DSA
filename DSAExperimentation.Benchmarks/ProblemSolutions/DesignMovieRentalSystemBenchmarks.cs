using BenchmarkDotNet.Attributes;
using static DSAExperimentation.LeetCode.DesignMovieRentalSystem.DesignMovieRentalSystemSolution;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are DesignMovieRentalSystemSolution's, the same classes
// DesignMovieRentalSystemTests proves correct. search() is the operation worth
// measuring - the sort-on-query system filters the whole catalogue down to the
// queried movie and sorts that subset from scratch, O(m log m), while the
// binary-search-tree system keeps each movie's copies ordered as they are inserted
// and answers with a plain in-order walk, O(m). Both systems are built in
// [GlobalSetup] from the same entry table, exactly as a real deployment would have
// built them before the first query, so construction is charged to setup rather
// than to the search being measured.
[MemoryDiagnoser]
public class DesignMovieRentalSystemBenchmarks
{
    private const int OtherMoviesCount = 19;
    private const int ShopsPerOtherMovie = 20;
    private const int TargetMovie = 0;
    private const int MaxPriceExclusive = 10_000;

    // LC problem number, reused as the deterministic seed for reproducible benchmarks.
    private const int RandomSeed = 1912;

    [Params(50, 500)]
    public int ShopsForTargetMovie;

    private MovieRentingSystemBySortOnQuery _sortOnQuery = null!;
    private MovieRentingSystemByBinarySearchTree _binarySearchTree = null!;

    [GlobalSetup]
    public void Setup()
    {
        var entries = BuildEntries();
        var shopCount = Math.Max(ShopsForTargetMovie, ShopsPerOtherMovie);

        _sortOnQuery = new MovieRentingSystemBySortOnQuery(shopCount, entries);
        _binarySearchTree = new MovieRentingSystemByBinarySearchTree(shopCount, entries);
    }

    [Benchmark(Baseline = true)]
    public List<int> SortOnQuery() => _sortOnQuery.Search(TargetMovie);

    [Benchmark]
    public List<int> BstMaintainedSorted() => _binarySearchTree.Search(TargetMovie);

    // The queried movie is stocked by ShopsForTargetMovie shops; nineteen other
    // movies pad the catalogue so the sort-on-query arm has entries to filter out
    // before it can sort.
    private int[][] BuildEntries()
    {
        var random = new Random(RandomSeed);
        var entries = new List<int[]>();

        for (var shop = 0; shop < ShopsForTargetMovie; shop++)
        {
            entries.Add([shop, TargetMovie, random.Next(1, MaxPriceExclusive)]);
        }

        for (var movie = 1; movie <= OtherMoviesCount; movie++)
        {
            for (var shop = 0; shop < ShopsPerOtherMovie; shop++)
            {
                entries.Add([shop, movie, random.Next(1, MaxPriceExclusive)]);
            }
        }

        return [.. entries];
    }
}
