using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Design Movie Rental System (LC 1912): search()'s "5 cheapest unrented shops for a
// movie" is the operation worth benchmarking. NaiveSortOnQuery represents a system
// that keeps every (shop, movie, price) entry in one flat list and answers each
// search() by filtering the whole list down to the queried movie and sorting that
// subset from scratch, O(m log m) where m is how many shops carry the queried movie.
// BstMaintainedSorted instead composes this repo's own
// BinarySearchTree<(int,int)>/InOrderTraversal - the same composition
// DesignMovieRentalSystemTests uses - built once up front the way a real system would
// maintain it incrementally across rent/drop calls, so each search() is a plain
// in-order walk of an already-sorted tree with no per-query sort, O(m).
[MemoryDiagnoser]
public class DesignMovieRentalSystemBenchmarks
{
    private const int ResultCap = 5;
    private const int OtherMoviesCount = 19;
    private const int ShopsPerOtherMovie = 20;
    private const int TargetMovie = 0;
    private const int MaxPriceExclusive = 10_000;

    // LC problem number, reused as the deterministic seed for reproducible benchmarks.
    private const int RandomSeed = 1912;

    [Params(50, 500)]
    public int ShopsForTargetMovie;

    private List<(int Shop, int Movie, int Price)> _entries = null!;
    private BinaryTreeNode<(int Price, int Shop)>? _targetTreeRoot;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _entries = new List<(int Shop, int Movie, int Price)>();

        AddTargetMovieEntries(random);
        AddOtherMovieEntries(random);

        _targetTreeRoot = BuildTargetMovieTree();
    }

    private void AddTargetMovieEntries(Random random)
    {
        for (var shop = 0; shop < ShopsForTargetMovie; shop++)
        {
            _entries.Add((shop, TargetMovie, random.Next(1, MaxPriceExclusive)));
        }
    }

    private void AddOtherMovieEntries(Random random)
    {
        for (var movie = 1; movie <= OtherMoviesCount; movie++)
        {
            for (var shop = 0; shop < ShopsPerOtherMovie; shop++)
            {
                _entries.Add((shop, movie, random.Next(1, MaxPriceExclusive)));
            }
        }
    }

    private BinaryTreeNode<(int Price, int Shop)>? BuildTargetMovieTree()
    {
        var tree = new BinarySearchTree<(int Price, int Shop)>();

        foreach (var (shop, movie, price) in _entries)
        {
            if (movie == TargetMovie)
            {
                tree.Insert((price, shop));
            }
        }

        return tree.Root;
    }

    [Benchmark(Baseline = true)]
    public List<int> NaiveSortOnQuery()
        => _entries
            .Where(e => e.Movie == TargetMovie)
            .OrderBy(e => e.Price)
            .ThenBy(e => e.Shop)
            .Take(ResultCap)
            .Select(e => e.Shop)
            .ToList();

    [Benchmark]
    public List<int> BstMaintainedSorted()
    {
        CollectState.Values.Value = new List<int>();
        CollectState.Cap.Value = ResultCap;
        InOrderTraversal.Walk<(int Price, int Shop), CollectHooks>(_targetTreeRoot);
        return CollectState.Values.Value;
    }

    private readonly struct CollectHooks : IInOrderHooks<(int Price, int Shop)>
    {
        public static void Visit(BinaryTreeNode<(int Price, int Shop)> node, int depth)
        {
            var values = CollectState.Values.Value!;

            if (values.Count < CollectState.Cap.Value)
            {
                values.Add(node.Value.Shop);
            }
        }
    }

    private static class CollectState
    {
        public static readonly AsyncLocal<List<int>> Values = new();
        public static readonly AsyncLocal<int> Cap = new();
    }
}
