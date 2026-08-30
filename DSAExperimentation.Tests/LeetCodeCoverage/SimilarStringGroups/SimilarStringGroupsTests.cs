using DSAExperimentation.DataStructures.DisjointSet;
using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SimilarStringGroups;

// LeetCode 839. Similar String Groups: DisjointSet over word indices (the same
// "union whenever two elements are connected, count roots at the end" shape
// AccountsMergeTests/RedundantConnectionTests already use), unioned whenever two
// words are similar (equal, or differ in exactly two positions that are each
// other's swap - transitivity handles chains like tars~rats~arts even though tars
// and arts alone are not directly similar). The final group count is deduped via
// Set<int> instead of AccountsMergeTests' HashMap<int,bool> placeholder, since
// counting distinct roots is exactly Set's own stated purpose.
public sealed partial class SimilarStringGroupsTests
{
    [Fact]
    public void CountGroups_ClassicExample_GroupsTransitivelySimilarWordsTogether()
    {
        string[] strs = ["tars", "rats", "arts", "star"];

        Assert.Equal(2, CountGroups(strs));
    }

    [Fact]
    public void CountGroups_TwoSimilarWords_ReturnsOneGroup()
    {
        string[] strs = ["omv", "ovm"];

        Assert.Equal(1, CountGroups(strs));
    }

    [Fact]
    public void CountGroups_DuplicateWords_TreatsEqualStringsAsSimilar()
    {
        string[] strs = ["abc", "abc", "xyz"];

        Assert.Equal(2, CountGroups(strs));
    }

    private static int CountGroups(string[] strs)
    {
        var components = new DisjointSet(strs.Length);

        for (var i = 0; i < strs.Length; i++)
        {
            for (var j = i + 1; j < strs.Length; j++)
            {
                if (IsSimilar(strs[i], strs[j]))
                {
                    components.Union(i, j);
                }
            }
        }

        var roots = new Set<int>();

        for (var i = 0; i < strs.Length; i++)
        {
            roots.TryAdd(components.Find(i));
        }

        return roots.Count;
    }

    private static bool IsSimilar(string first, string second)
    {
        var mismatchCount = 0;
        var firstMismatch = -1;
        var secondMismatch = -1;

        for (var i = 0; i < first.Length; i++)
        {
            if (first[i] == second[i])
            {
                continue;
            }

            mismatchCount++;

            if (mismatchCount > 2)
            {
                return false;
            }

            if (mismatchCount == 1)
            {
                firstMismatch = i;
            }
            else
            {
                secondMismatch = i;
            }
        }

        return mismatchCount == 0
            || (mismatchCount == 2 && first[firstMismatch] == second[secondMismatch] && first[secondMismatch] == second[firstMismatch]);
    }
}
