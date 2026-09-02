using DSAExperimentation.DataStructures.DisjointSet;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CheckingExistenceOfEdgeLengthLimitedPaths;

// LeetCode 1697. Checking Existence of Edge Length Limited Paths: the standard
// offline Kruskal-style sweep, using this repo's own DisjointSet - the same
// primitive FindCriticalAndPseudoCriticalEdgesInMinimumSpanningTreeTests/
// GraphConnectivityWithThresholdTests already compose. Sort edges by weight and
// queries by limit, both ascending, then walk queries in limit order: before
// answering a query, union every edge whose weight is still strictly below that
// query's limit (edgeIndex only ever advances, so the whole edge list is swept
// once across every query, not once per query).
public sealed partial class CheckingExistenceOfEdgeLengthLimitedPathsTests
{
    [Fact]
    public void DistanceLimitedPathsExist_ClassicExample_MatchesExpectedResults()
    {
        int[][] edgeList = [[0, 1, 2], [1, 2, 4], [2, 0, 8], [1, 0, 16]];
        int[][] queries = [[0, 1, 2], [0, 2, 5]];

        var actual = DistanceLimitedPathsExist(3, edgeList, queries);
        Assert.Equal([false, true], actual);
    }

    [Fact]
    public void DistanceLimitedPathsExist_SecondExample_MatchesExpectedResults()
    {
        int[][] edgeList = [[0, 1, 10], [1, 2, 5], [2, 3, 9], [3, 4, 13]];
        int[][] queries = [[0, 4, 14], [1, 4, 13]];

        var actual = DistanceLimitedPathsExist(5, edgeList, queries);
        Assert.Equal([true, false], actual);
    }

    private static bool[] DistanceLimitedPathsExist(int n, int[][] edgeList, int[][] queries)
    {
        var edgesByWeight = edgeList.OrderBy(edge => edge[2]).ToArray();
        var queryOrder = Enumerable.Range(0, queries.Length).OrderBy(i => queries[i][2]).ToArray();

        var components = new DisjointSet(n);
        var results = new bool[queries.Length];
        var edgeIndex = 0;

        foreach (var queryIndex in queryOrder)
        {
            var query = queries[queryIndex];

            while (edgeIndex < edgesByWeight.Length && edgesByWeight[edgeIndex][2] < query[2])
            {
                components.Union(edgesByWeight[edgeIndex][0], edgesByWeight[edgeIndex][1]);
                edgeIndex++;
            }

            results[queryIndex] = components.IsConnected(query[0], query[1]);
        }

        return results;
    }
}
