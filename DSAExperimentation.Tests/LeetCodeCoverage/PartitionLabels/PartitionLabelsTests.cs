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

        var scanner = new PartitionScanner();

        for (var i = 0; i < s.Length; i++)
        {
            lastIndex.TryGetValue(s[i], out var furthest);
            scanner.Advance(i, furthest);
        }

        return scanner.Sizes;
    }

    private sealed class PartitionScanner
    {
        private int _start;
        private int _end;

        public List<int> Sizes { get; } = [];

        public void Advance(int i, int furthest)
        {
            _end = Math.Max(_end, furthest);

            if (i != _end)
            {
                return;
            }

            Sizes.Add(_end - _start + 1);
            _start = i + 1;
        }
    }
}
