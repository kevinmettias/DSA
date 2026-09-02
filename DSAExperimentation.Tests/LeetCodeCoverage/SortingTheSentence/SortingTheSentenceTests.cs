using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SortingTheSentence;

// LeetCode 1859. Sorting the Sentence: each word carries its original 1-9 position as
// a trailing digit. This repo's own MergeSort.Sort<Element,TSequence> sorts the words
// in place by that trailing digit via a custom comparer over an ArrayIndexedSequence -
// the same custom-comparer-over-ArrayIndexedSequence shape RelativeSortArrayTests.cs
// already exercises - then the digit is stripped and the words are joined back together.
public sealed class SortingTheSentenceTests
{
    [Fact]
    public void SortSentence_LeetCodeExample_ReconstructsOriginalOrder()
    {
        var s = "is2 sentence4 This1 a3";

        var sentence = SortSentence(s);

        Assert.Equal("This is a sentence", sentence);
    }

    [Fact]
    public void SortSentence_SecondLeetCodeExample_ReconstructsOriginalOrder()
    {
        var s = "Myself2 Me1 I4 and3";

        var sentence = SortSentence(s);

        Assert.Equal("Me Myself and I", sentence);
    }

    [Fact]
    public void SortSentence_SingleWord_ReturnsWordUnchanged()
    {
        var s = "Hello1";

        var sentence = SortSentence(s);

        Assert.Equal("Hello", sentence);
    }

    private static string SortSentence(string s)
    {
        var words = s.Split(' ');
        var byPosition = Comparer<string>.Create((a, b) => a[^1].CompareTo(b[^1]));

        MergeSort.Sort<string, ArrayIndexedSequence<string>>(new ArrayIndexedSequence<string>(words), byPosition);

        return string.Join(' ', words.Select(word => word[..^1]));
    }
}
