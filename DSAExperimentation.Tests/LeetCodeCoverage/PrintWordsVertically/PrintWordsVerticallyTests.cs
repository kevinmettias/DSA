using DSAExperimentation.DataStructures.DynamicArray;

namespace DSAExperimentation.Tests.LeetCodeCoverage.PrintWordsVertically;

// LeetCode 1324. Print Words Vertically: build each output row (one per
// character column) in this repo's own DynamicArray<char> - the same growable
// char buffer StreamOfCharactersTests uses to stand in for a running stream -
// then trim trailing spaces by popping from the tail via RemoveAt(Count - 1).
// DynamicArray earns its place over a plain List<char> here specifically
// because trimming needs both indexed Get (to read the last char) and O(1)
// removal from the end, the same "grow at the back, trim from the back" shape
// as a stack, but keyed by column index rather than push order.
public sealed partial class PrintWordsVerticallyTests
{
    [Fact]
    public void PrintVertically_ClassicExample_ReturnsColumnsWithNoTrailingSpaces()
    {
        var result = PrintVertically("HOW ARE YOU");

        Assert.Equal(["HAY", "ORO", "WEU"], result);
    }

    [Fact]
    public void PrintVertically_UnevenWordLengths_TrimsTrailingSpacesButKeepsInterior()
    {
        var result = PrintVertically("TO BE OR NOT TO BE");

        Assert.Equal(["TBONTB", "OEROOE", "   T"], result);
    }

    private static IList<string> PrintVertically(string s)
    {
        var words = s.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var maxLength = words.Max(word => word.Length);
        var result = new List<string>();

        for (var column = 0; column < maxLength; column++)
        {
            var buffer = new DynamicArray<char>();

            foreach (var word in words)
            {
                buffer.Add(column < word.Length ? word[column] : ' ');
            }

            while (buffer.Count > 0 && buffer.Get(buffer.Count - 1) == ' ')
            {
                buffer.RemoveAt(buffer.Count - 1);
            }

            var chars = new char[buffer.Count];
            for (var i = 0; i < buffer.Count; i++)
            {
                chars[i] = buffer.Get(i);
            }

            result.Add(new string(chars));
        }

        return result;
    }
}
