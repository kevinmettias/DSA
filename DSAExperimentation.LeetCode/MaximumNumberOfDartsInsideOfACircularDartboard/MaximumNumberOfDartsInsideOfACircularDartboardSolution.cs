using DSAExperimentation.DataStructures.DynamicArray;

namespace DSAExperimentation.LeetCode.MaximumNumberOfDartsInsideOfACircularDartboard;

// LeetCode 1453. Maximum Number of Darts Inside of a Circular Dartboard: place a
// circle of the given radius anywhere on the plane and report how many darts it can
// cover.
//
// An optimal circle either covers a single dart or can be slid until at least two
// darts sit on its boundary, so every center worth trying is either a dart itself or
// one of the (up to) two positions where a radius-r circle passes through a pair of
// darts no more than 2r apart. Scoring each candidate against every dart answers the
// problem in O(n^3) without the naive every-subset search.
//
// That candidate-center technique is the established solution, so there is no simpler
// correct brute force to contrast it against - checking only the darts themselves as
// centers is provably wrong (LeetCode's second example's optimal circle is centred on
// neither input dart). What the two strategies below differ in is therefore the
// candidate buffer they grow: a plain BCL List, or this repo's own DynamicArray in the
// same growable-scratch-buffer role CircleAndRectangleOverlapping (LC 1401) gives it.
internal static class MaximumNumberOfDartsInsideOfACircularDartboardSolution
{
    // Two darts more than a diameter apart can never share a circle of this radius.
    private const double MaxPairDistanceSquaredFactor = 4.0;
    private const double Half = 2.0;

    // Boundary darts land exactly on the radius, so the containment test is inclusive
    // up to floating-point drift in the intersection arithmetic.
    private const double DistanceComparisonEpsilon = 1e-6;

    // A circle can always be placed over one dart, so no answer is ever below 1.
    private const int LoneDartCount = 1;

    // The baseline: the same candidate-center sweep, collecting into a plain BCL
    // List<(double, double)> and scanning it afterwards. Deliberately written without
    // this repo's primitives - it is the arm the composed variant has to justify
    // itself against.
    public static int MaxDartsByListCandidates(int[][] darts, int radius)
    {
        var candidates = new List<(double X, double Y)>();
        CollectCandidates(darts, radius, new ListCandidateBuffer(candidates));

        var best = LoneDartCount;

        foreach (var candidate in candidates)
        {
            var count = CountWithin(darts, candidate, radius);
            best = Math.Max(best, count);
        }

        return best;
    }

    // The composed variant: identical geometry, with this repo's own DynamicArray as
    // the growable candidate buffer and its index-based Get driving the scoring pass.
    public static int MaxDartsByDynamicArrayCandidates(int[][] darts, int radius)
    {
        var candidates = new DynamicArray<(double X, double Y)>();
        CollectCandidates(darts, radius, new DynamicArrayCandidateBuffer(candidates));

        var best = LoneDartCount;

        for (var c = 0; c < candidates.Count; c++)
        {
            var count = CountWithin(darts, candidates.Get(c), radius);
            best = Math.Max(best, count);
        }

        return best;
    }

    // Where the candidate centers go. Naming the sink puts the container in the sweep's
    // contract as well as in the two strategies below: CollectCandidates produces centers
    // for a buffer, rather than calling something that takes a pair of doubles.
    private interface ICandidateBuffer
    {
        void Collect((double X, double Y) center);
    }

    // Every center worth scoring, handed to whichever buffer the calling strategy
    // grows: each dart itself, plus the circle positions every close-enough pair of
    // darts admits. Shared so the only thing the two strategies differ in is the
    // container, which is the whole point of the comparison.
    private static void CollectCandidates(int[][] darts, int radius, ICandidateBuffer buffer)
    {
        for (var i = 0; i < darts.Length; i++)
        {
            buffer.Collect((darts[i][0], darts[i][1]));

            for (var j = i + 1; j < darts.Length; j++)
            {
                if (BoundaryCenters(darts[i], darts[j], radius) is { } centers)
                {
                    buffer.Collect(centers.First);
                    buffer.Collect(centers.Second);
                }
            }
        }
    }

    // The BCL list arm's buffer, holding the same List the scoring pass below walks.
    private sealed class ListCandidateBuffer(List<(double X, double Y)> candidates) : ICandidateBuffer
    {
        public void Collect((double X, double Y) center) => candidates.Add(center);
    }

    // The composed arm's buffer, holding this repo's DynamicArray that the scoring
    // pass below indexes into.
    private sealed class DynamicArrayCandidateBuffer(DynamicArray<(double X, double Y)> candidates)
        : ICandidateBuffer
    {
        public void Collect((double X, double Y) center) => candidates.Add(center);
    }

    // The two centers of a radius-r circle through both darts: the chord's midpoint
    // offset along the perpendicular by the height the circle needs to reach both
    // ends. Null when the darts are further apart than a diameter, since no such
    // circle exists then.
    private static ((double X, double Y) First, (double X, double Y) Second)? BoundaryCenters(
        int[] a, int[] b, int radius)
    {
        var dx = (double)(b[0] - a[0]);
        var dy = (double)(b[1] - a[1]);
        var distanceSquared = (dx * dx) + (dy * dy);

        if (distanceSquared > MaxPairDistanceSquaredFactor * radius * radius)
        {
            return null;
        }

        var midX = (a[0] + b[0]) / Half;
        var midY = (a[1] + b[1]) / Half;
        var distance = Math.Sqrt(distanceSquared);
        var halfChord = distance / Half;
        var heightSquared = Math.Max(0.0, ((double)radius * radius) - (halfChord * halfChord));
        var height = Math.Sqrt(heightSquared);
        var offsetX = -dy / distance * height;
        var offsetY = dx / distance * height;

        return ((midX + offsetX, midY + offsetY), (midX - offsetX, midY - offsetY));
    }

    private static int CountWithin(int[][] darts, (double X, double Y) center, int radius)
    {
        var count = 0;

        foreach (var dart in darts)
        {
            var dx = dart[0] - center.X;
            var dy = dart[1] - center.Y;

            if ((dx * dx) + (dy * dy) <= ((double)radius * radius) + DistanceComparisonEpsilon)
            {
                count++;
            }
        }

        return count;
    }
}
