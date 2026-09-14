using DSAExperimentation.DataStructures.SegmentTree;

namespace DSAExperimentation.LeetCode.LongestSubstringOfOneRepeatingCharacter;

// LeetCode 2213. Longest Substring of One Repeating Character: apply each
// queryCharacters[i] at queryIndices[i] in turn and, after every one of those point
// updates, report the length of the longest substring of a single repeated
// character in the whole string.
//
// The two strategies differ only in what they keep between updates: nothing (rescan
// the buffer end to end) versus this repo's own SegmentTree<Element,TOperation>
// carrying a RunSegment per node, so a point update costs O(log n) and the answer
// is read straight off the root.
internal static class LongestSubstringOfOneRepeatingCharacterSolution
{
    // The textbook answer: a plain char[] mutated in place and rescanned from the
    // start after each update, O(n) per query. Deliberately BCL-only - it is the arm
    // the segment-tree strategy below has to justify itself against.
    public static int[] LongestRepeatingByLinearRescan(string s, string queryCharacters, int[] queryIndices)
    {
        var chars = s.ToCharArray();
        var lengths = new int[queryCharacters.Length];

        for (var i = 0; i < queryCharacters.Length; i++)
        {
            chars[queryIndices[i]] = queryCharacters[i];
            lengths[i] = LongestRun(chars);
        }

        return lengths;
    }

    private static int LongestRun(char[] chars)
    {
        var best = 0;
        var i = 0;

        while (i < chars.Length)
        {
            var j = i;

            while (j < chars.Length && chars[j] == chars[i])
            {
                j++;
            }

            best = Math.Max(best, j - i);
            i = j;
        }

        return best;
    }

    // This repo's own SegmentTree keyed by index, each node a RunSegment merged by
    // RunAggregate. Update is O(log n); the query spans exactly the tree's own root
    // range, which SegmentTree.Query answers from the root node without descending.
    public static int[] LongestRepeatingBySegmentTree(string s, string queryCharacters, int[] queryIndices)
    {
        var tree = new SegmentTree<RunSegment, RunAggregate>(BuildLeaves(s));
        var lengths = new int[queryCharacters.Length];

        for (var i = 0; i < queryCharacters.Length; i++)
        {
            tree.Update(queryIndices[i], RunSegment.Leaf(queryCharacters[i]));
            lengths[i] = tree.Query(0, s.Length - 1).MaxLen;
        }

        return lengths;
    }

    private static RunSegment[] BuildLeaves(string s)
    {
        var leaves = new RunSegment[s.Length];

        for (var i = 0; i < s.Length; i++)
        {
            leaves[i] = RunSegment.Leaf(s[i]);
        }

        return leaves;
    }
}
