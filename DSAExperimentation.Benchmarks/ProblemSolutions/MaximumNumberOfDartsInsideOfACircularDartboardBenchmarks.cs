using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.DynamicArray;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Maximum Number of Darts Inside of a Circular Dartboard (LC 1453): both variants
// run the same O(n^3) "candidate centers from every dart pair, count darts within
// r of each candidate" algorithm - the pairwise-circle-intersection technique is
// the established solution for this problem, so there is no simpler correct brute
// force to contrast it with (checking only the darts themselves as centers, as a
// faster "brute force" would, is provably wrong: ClassicExampleTwo's optimal circle
// centers on neither input dart). What IS compared is the candidate-buffer
// container: a plain BCL List<(double,double)> vs. this repo's own
// DynamicArray<(double,double)> (the same growable-buffer role it plays in
// CircleAndRectangleOverlappingBenchmarks' lattice scan, LC 1401).
[MemoryDiagnoser]
public class MaximumNumberOfDartsInsideOfACircularDartboardBenchmarks
{
    private const int Radius = 50;

    [Params(20, 60)]
    public int DartCount;

    private int[][] _darts = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1453);
        _darts = Enumerable.Range(0, DartCount)
            .Select(_ => new[] { random.Next(-100, 100), random.Next(-100, 100) })
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public int PairwiseCandidateCentersWithList()
    {
        var candidates = new List<(double X, double Y)>();
        CollectCandidates(candidates.Add);
        return BestCount(candidates);
    }

    [Benchmark]
    public int PairwiseCandidateCentersWithDynamicArray()
    {
        var candidates = new DynamicArray<(double X, double Y)>();
        CollectCandidates(c => candidates.Add(c));

        var best = 1;

        for (var c = 0; c < candidates.Count; c++)
        {
            best = Math.Max(best, CountWithin(candidates.Get(c)));
        }

        return best;
    }

    private void CollectCandidates(Action<(double X, double Y)> add)
    {
        for (var i = 0; i < _darts.Length; i++)
        {
            add((_darts[i][0], _darts[i][1]));

            for (var j = i + 1; j < _darts.Length; j++)
            {
                var dx = (double)(_darts[j][0] - _darts[i][0]);
                var dy = (double)(_darts[j][1] - _darts[i][1]);
                var distanceSquared = (dx * dx) + (dy * dy);

                if (distanceSquared > 4.0 * Radius * Radius)
                {
                    continue;
                }

                var midX = (_darts[i][0] + _darts[j][0]) / 2.0;
                var midY = (_darts[i][1] + _darts[j][1]) / 2.0;
                var distance = Math.Sqrt(distanceSquared);
                var halfChord = distance / 2.0;
                var height = Math.Sqrt(Math.Max(0.0, ((double)Radius * Radius) - (halfChord * halfChord)));
                var offsetX = -dy / distance * height;
                var offsetY = dx / distance * height;

                add((midX + offsetX, midY + offsetY));
                add((midX - offsetX, midY - offsetY));
            }
        }
    }

    private int BestCount(List<(double X, double Y)> candidates)
    {
        var best = 1;

        foreach (var candidate in candidates)
        {
            best = Math.Max(best, CountWithin(candidate));
        }

        return best;
    }

    private int CountWithin((double X, double Y) center)
    {
        var count = 0;

        foreach (var dart in _darts)
        {
            var dx = dart[0] - center.X;
            var dy = dart[1] - center.Y;

            if ((dx * dx) + (dy * dy) <= ((double)Radius * Radius) + 1e-6)
            {
                count++;
            }
        }

        return count;
    }
}
