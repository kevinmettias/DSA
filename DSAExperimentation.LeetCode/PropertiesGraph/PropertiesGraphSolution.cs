using DSAExperimentation.DataStructures.DisjointSet;
using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.LeetCode.PropertiesGraph;

// LeetCode 3493. Properties Graph: an undirected edge joins i and j whenever
// intersect(properties[i], properties[j]) - the count of DISTINCT values common
// to both rows - is >= `minimumIntersectionCount`. Return the number of connected
// components.
//
// Both strategies test every pair (n <= 100 per LC's own constraints, so O(n^2 *
// m) is already the intended order) and differ only in what computes intersect()
// and what tracks components - the same "which primitive would you reach for"
// contrast CountConnectedComponentsInLCMGraphSolution's two arms draw, just with
// both axes (set membership, union-find) varied together here instead of one.
internal static class PropertiesGraphSolution
{
    // Baseline: intersect() via a BCL HashSet per row, union-find via a
    // hand-rolled int[] parent array with path compression - deliberately no
    // repo primitive, the "what you'd write without this repo" arm the composed
    // strategy below has to beat.
    public static int NumberOfComponentsByBruteForce(int[][] properties, int minimumIntersectionCount)
    {
        var n = properties.Length;
        var parent = new int[n];

        for (var i = 0; i < n; i++)
        {
            parent[i] = i;
        }

        var rows = BuildHashSets(properties);

        UnionIntersectingRows(rows, minimumIntersectionCount, parent);

        return CountRoots(parent);
    }

    // One BCL HashSet per row, seeded from that row's own values.
    private static HashSet<int>[] BuildHashSets(int[][] properties)
    {
        var rows = new HashSet<int>[properties.Length];

        for (var i = 0; i < properties.Length; i++)
        {
            rows[i] = [.. properties[i]];
        }

        return rows;
    }

    // Unions every pair of rows whose distinct-value intersection reaches
    // `minimumIntersectionCount`.
    private static void UnionIntersectingRows(HashSet<int>[] rows, int minimumIntersectionCount, int[] parent)
    {
        for (var i = 0; i < rows.Length; i++)
        {
            for (var j = i + 1; j < rows.Length; j++)
            {
                if (IntersectCount(rows[i], rows[j]) >= minimumIntersectionCount)
                {
                    Union(parent, i, j);
                }
            }
        }
    }

    // The answer: how many distinct components the forest holds, i.e. how many
    // distinct roots it has.
    private static int CountRoots(int[] parent)
    {
        var roots = new HashSet<int>();

        for (var i = 0; i < parent.Length; i++)
        {
            var root = Find(parent, i);
            roots.Add(root);
        }

        return roots.Count;
    }

    private static int IntersectCount(HashSet<int> firstValues, HashSet<int> secondValues)
    {
        var count = 0;

        foreach (var value in firstValues)
        {
            if (secondValues.Contains(value))
            {
                count++;
            }
        }

        return count;
    }

    private static int Find(int[] parent, int element)
    {
        while (parent[element] != element)
        {
            parent[element] = parent[parent[element]];
            element = parent[element];
        }

        return element;
    }

    private static void Union(int[] parent, int firstElement, int secondElement)
    {
        var firstRoot = Find(parent, firstElement);
        var secondRoot = Find(parent, secondElement);

        if (firstRoot != secondRoot)
        {
            parent[firstRoot] = secondRoot;
        }
    }

    // Composed: this repo's own Set<int> for both jobs it already has - TryAdd
    // (via the bulk-seeding constructor) for per-row dedup, Has for O(1)-average
    // membership - and DisjointSet for union-find, the same forest
    // CountConnectedComponentsInLCMGraphSolution's own composed arm reaches for.
    // Set<T> has no enumerator (Trie.cs's own doc comment notes Trie<TValue>
    // keeps a comparable surface deliberately narrow), so each row's distinct
    // values are also kept as a plain array alongside its Set - Distinct's own
    // Set<int> is what produces that array, TryAdd rejecting every repeat.
    public static int NumberOfComponentsByDisjointSet(int[][] properties, int minimumIntersectionCount)
    {
        var n = properties.Length;
        var forest = new DisjointSet(n);
        var rowSets = new Set<int>[n];
        var distinctRows = new int[n][];

        BuildRowSets(properties, rowSets, distinctRows);

        UnionIntersectingRows(distinctRows, rowSets, minimumIntersectionCount, forest);

        return CountRoots(forest);
    }

    // One repo Set per row for membership, plus the same row's distinct values as a
    // plain array (Set has no enumerator to walk).
    private static void BuildRowSets(int[][] properties, Set<int>[] rowSets, int[][] distinctRows)
    {
        for (var i = 0; i < properties.Length; i++)
        {
            rowSets[i] = new Set<int>(properties[i]);
            distinctRows[i] = Distinct(properties[i]);
        }
    }

    private static void UnionIntersectingRows(
        int[][] distinctRows, Set<int>[] rowSets, int minimumIntersectionCount, DisjointSet forest)
    {
        for (var i = 0; i < distinctRows.Length; i++)
        {
            for (var j = i + 1; j < distinctRows.Length; j++)
            {
                if (IntersectCount(distinctRows[i], rowSets[j]) >= minimumIntersectionCount)
                {
                    forest.Union(i, j);
                }
            }
        }
    }

    private static int CountRoots(DisjointSet forest)
    {
        var roots = new HashSet<int>();

        for (var i = 0; i < forest.Count; i++)
        {
            var root = forest.Find(i);
            roots.Add(root);
        }

        return roots.Count;
    }

    private static int IntersectCount(int[] distinctValues, Set<int> other)
    {
        var count = 0;

        foreach (var value in distinctValues)
        {
            if (other.Has(value))
            {
                count++;
            }
        }

        return count;
    }

    private static int[] Distinct(int[] row)
    {
        var seen = new Set<int>();
        var distinct = new List<int>(row.Length);

        foreach (var value in row)
        {
            if (seen.TryAdd(value))
            {
                distinct.Add(value);
            }
        }

        return [.. distinct];
    }
}
