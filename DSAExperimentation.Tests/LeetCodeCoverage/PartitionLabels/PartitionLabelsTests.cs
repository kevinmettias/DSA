using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Tests.LeetCodeCoverage.PartitionLabels;

// LeetCode 763. Partition Labels: one HashMap<char,int> pass (TwoSumTests' exact
// shape) to record each letter's last occurrence, then a greedy scan that closes a
// partition the moment the running index catches up to the furthest last-occurrence
// seen among the letters in it so far.
public sealed partial class PartitionLabelsTests
{
    [Fact]
    public void PartitionSizes_ClassicExample_ReturnsExpectedPartitionSizes()
    {
        var sizes = PartitionIntoLabelSizes("ababcbacadefegdehijhklij");

        Assert.Equal([9, 7, 8], sizes);
    }

    [Fact]
    public void PartitionSizes_EachLetterDistinct_ReturnsSizeOnePerLetter()
    {
        var sizes = PartitionIntoLabelSizes("abc");

        Assert.Equal([1, 1, 1], sizes);
    }

    [Fact]
    public void PartitionSizes_EveryLetterRepeatsThroughoutTheString_ReturnsOneWholeStringPartition()
    {
        var sizes = PartitionIntoLabelSizes("abab");

        Assert.Equal([4], sizes);
    }

    private static List<int> PartitionIntoLabelSizes(string s)
    {
        var lastIndex = new HashMap<char, int>();

        for (var i = 0; i < s.Length; i++)
        {
            lastIndex.Set(s[i], i);
        }

        var sizes = new List<int>();
        var start = 0;
        var end = 0;

        for (var i = 0; i < s.Length; i++)
        {
            lastIndex.TryGetValue(s[i], out var furthest);
            end = Math.Max(end, furthest);

            if (i != end)
            {
                continue;
            }

            sizes.Add(end - start + 1);
            start = i + 1;
        }

        return sizes;
    }
}
