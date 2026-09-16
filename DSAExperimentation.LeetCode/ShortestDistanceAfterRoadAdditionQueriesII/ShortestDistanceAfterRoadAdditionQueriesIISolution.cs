using DSAExperimentation.DataStructures.IntervalSet;

namespace DSAExperimentation.LeetCode.ShortestDistanceAfterRoadAdditionQueriesII;

// LeetCode 3244. Shortest Distance After Road Addition Queries II: cityCount cities sit
// on a chain 0 -> 1 -> ... -> cityCount-1; each query adds a one-way shortcut u -> v
// (v > u + 1) and asks for the shortest 0 -> cityCount-1 path length afterward.
// LC guarantees queries never
// partially overlap - for any two, one's interval is either nested inside or disjoint
// from the other's - which is exactly what makes this variant ("II") solvable faster
// than re-running BFS from scratch every time.
//
// Under that guarantee the shortest path is always "walk every city that has not yet
// been bypassed by some shortcut, in increasing order": whichever query most recently
// swallowed a run of interior cities is the query that supplies the direct edge between
// its two still-standing neighbors, and no earlier shortcut can have left a gap a later
// one crosses without fully containing it, because crossing is exactly what the
// guarantee rules out. So the answer after each query is just (cityCount - 1) minus
// however many interior cities have been bypassed so far - and "which cities have been
// bypassed" is precisely a merged set of disjoint closed ranges, which is this repo's
// own IntervalSet<int>.
internal static class ShortestDistanceAfterRoadAdditionQueriesIISolution
{
    // Baseline: keep growing one real adjacency list and re-run a fresh BFS from city 0
    // after every query, the way LC 3243's small-n sibling problem solves it directly.
    // Correct for any query pattern, nested/disjoint or not - it just does not exploit
    // the guarantee, so it redoes O(n) of search work per query instead of O(1)
    // amortized. Plain BCL Queue and arrays only.
    public static int[] ShortestDistancesByAdjacencyBfs(int cityCount, int[][] queries)
    {
        var adjacency = new List<int>[cityCount];

        for (var city = 0; city < cityCount; city++)
        {
            var hasNextCity = city + 1 < cityCount;

            adjacency[city] = hasNextCity ? AdjacencyToNextCity(city) : NoAdjacency();
        }

        var answers = new int[queries.Length];

        for (var q = 0; q < queries.Length; q++)
        {
            var (u, v) = (queries[q][0], queries[q][1]);
            adjacency[u].Add(v);
            answers[q] = ShortestPathLength(adjacency, cityCount);
        }

        return answers;
    }

    private static List<int> AdjacencyToNextCity(int city) => [city + 1];

    private static List<int> NoAdjacency() => [];

    private static int ShortestPathLength(List<int>[] adjacency, int cityCount)
    {
        var distance = new int[cityCount];
        Array.Fill(distance, -1);
        distance[0] = 0;

        var queue = new Queue<int>();
        queue.Enqueue(0);

        while (queue.Count > 0)
        {
            var city = queue.Dequeue();

            if (city == cityCount - 1)
            {
                return distance[city];
            }

            RelaxNeighbors(adjacency, city, distance, queue);
        }

        return -1;
    }

    // The BFS front's one step outward: every not-yet-seen neighbor of city joins the
    // queue one hop further out, and the ones already seen are left where they are.
    private static void RelaxNeighbors(
        List<int>[] adjacency,
        int city,
        int[] distance,
        Queue<int> queue)
    {
        foreach (var neighbor in adjacency[city])
        {
            if (distance[neighbor] != -1)
            {
                continue;
            }

            distance[neighbor] = distance[city] + 1;
            queue.Enqueue(neighbor);
        }
    }

    // Composed: every query (u, v) bypasses the closed range of interior cities
    // [u+1, v-1]. Feeding that range into an IntervalSet<int> gets its merging with
    // every previously-bypassed range for free, so the running total of bypassed
    // cities - and therefore the answer, (cityCount - 1) minus that total - only ever needs
    // this query's own new range, never the whole history.
    public static int[] ShortestDistancesByIntervalSet(int cityCount, int[][] queries)
    {
        var bypassed = new IntervalSet<int>();
        var answers = new int[queries.Length];

        for (var q = 0; q < queries.Length; q++)
        {
            var (u, v) = (queries[q][0], queries[q][1]);

            if (u + 1 <= v - 1)
            {
                bypassed.Add(u + 1, v - 1);
            }

            answers[q] = cityCount - 1 - CountBypassed(bypassed);
        }

        return answers;
    }

    private static int CountBypassed(IntervalSet<int> bypassed)
    {
        var total = 0;

        for (var i = 0; i < bypassed.Count; i++)
        {
            var (start, end) = bypassed.Get(i);
            total += end - start + 1;
        }

        return total;
    }
}
