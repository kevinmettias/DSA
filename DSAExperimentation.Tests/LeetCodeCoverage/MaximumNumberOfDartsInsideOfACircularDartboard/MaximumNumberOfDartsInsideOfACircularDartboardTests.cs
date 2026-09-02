using DSAExperimentation.DataStructures.DynamicArray;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximumNumberOfDartsInsideOfACircularDartboard;

// LeetCode 1453. Maximum Number of Darts Inside of a Circular Dartboard: the
// optimal circle of radius r either contains a single dart (count 1) or has at
// least two darts on its boundary, so every candidate center comes from a single
// dart or from the (up to) two points where a radius-r circle passes through a
// pair of darts within distance 2r. Candidate centers are collected into this
// repo's own DynamicArray<(double,double)> - the same "growable scratch buffer"
// role it plays for CircleAndRectangleOverlappingTests' (LC 1401) lattice scan -
// then each candidate is scored with that same problem's closed-form squared-
// distance-vs-radius-squared check, just repeated per candidate instead of once.
// Avoids the naive O(2^n) every-subset search.
public sealed partial class MaximumNumberOfDartsInsideOfACircularDartboardTests
{
    [Fact]
    public void MaxDarts_ClassicExampleOne_ReturnsFour()
    {
        int[][] darts = [[-2, 0], [2, 0], [0, 1], [0, -1]];

        var actual = MaxDarts(darts, radius: 2);
        Assert.Equal(4, actual);
    }

    [Fact]
    public void MaxDarts_ClassicExampleTwo_ReturnsFive()
    {
        int[][] darts = [[-3, 0], [3, 0], [2, 6], [5, 4], [0, 9], [7, 8]];

        var actual = MaxDarts(darts, radius: 5);
        Assert.Equal(5, actual);
    }

    [Fact]
    public void MaxDarts_SingleDart_ReturnsOne()
    {
        int[][] darts = [[3, 7]];

        var actual = MaxDarts(darts, radius: 10);
        Assert.Equal(1, actual);
    }

    [Fact]
    public void MaxDarts_NoTwoDartsFitTogether_ReturnsOne()
    {
        int[][] darts = [[0, 0], [100, 100], [-100, -100]];

        var actual = MaxDarts(darts, radius: 1);
        Assert.Equal(1, actual);
    }

    private static int MaxDarts(int[][] darts, int radius)
    {
        var candidates = BuildCandidateCenters(darts, radius);

        return FindBestCandidateCount(darts, candidates, radius);
    }

    private static DynamicArray<(double X, double Y)> BuildCandidateCenters(int[][] darts, int radius)
    {
        var candidates = new DynamicArray<(double X, double Y)>();

        for (var i = 0; i < darts.Length; i++)
        {
            var pointI = ((double)darts[i][0], (double)darts[i][1]);
            candidates.Add(pointI);

            for (var j = i + 1; j < darts.Length; j++)
            {
                var pointJ = ((double)darts[j][0], (double)darts[j][1]);
                AddBoundaryCandidates(candidates, pointI, pointJ, radius);
            }
        }

        return candidates;
    }

    private static int FindBestCandidateCount(int[][] darts, DynamicArray<(double X, double Y)> candidates, int radius)
    {
        var best = 1;

        for (var c = 0; c < candidates.Count; c++)
        {
            var (centerX, centerY) = candidates.Get(c);
            var count = CountDartsNearCenter(darts, centerX, centerY, radius);

            best = Math.Max(best, count);
        }

        return best;
    }

    private static int CountDartsNearCenter(int[][] darts, double centerX, double centerY, int radius)
    {
        var count = 0;

        foreach (var dart in darts)
        {
            var dx = dart[0] - centerX;
            var dy = dart[1] - centerY;

            if ((dx * dx) + (dy * dy) <= ((double)radius * radius) + 1e-6)
            {
                count++;
            }
        }

        return count;
    }

    private static void AddBoundaryCandidates(DynamicArray<(double X, double Y)> candidates, (double X, double Y) a, (double X, double Y) b, int radius)
    {
        var dx = b.X - a.X;
        var dy = b.Y - a.Y;
        var distanceSquared = (dx * dx) + (dy * dy);

        if (distanceSquared > 4.0 * radius * radius)
        {
            return;
        }

        var midX = (a.X + b.X) / 2.0;
        var midY = (a.Y + b.Y) / 2.0;
        var distance = Math.Sqrt(distanceSquared);
        var halfChord = distance / 2.0;
        var heightSquared = Math.Max(0.0, ((double)radius * radius) - (halfChord * halfChord));
        var height = Math.Sqrt(heightSquared);
        var offsetX = -dy / distance * height;
        var offsetY = dx / distance * height;

        candidates.Add((midX + offsetX, midY + offsetY));
        candidates.Add((midX - offsetX, midY - offsetY));
    }
}
