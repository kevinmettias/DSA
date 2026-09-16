using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.SortingTheSentence;

// LeetCode 1859. Sorting the Sentence: a shuffled sentence's words each carry
// their original 1-9 position as a trailing digit, and the sentence has to be put
// back together without them.
//
// MergeSort is this repo's own MergeSort.Sort<Element,TSequence> over an
// ArrayIndexedSequence with a comparer on the embedded position - the same
// custom-comparer-over-ArrayIndexedSequence shape RelativeSortArray uses - so the
// words are ordered in one O(n log n) pass and then stripped. PositionScan is the
// textbook answer: rescan the whole word array once per output slot for the
// smallest position not yet placed, O(n^2).
//
// The position is read as a whole trailing run of digits rather than as the single
// last character the 1-9 constraint guarantees. That agrees with LeetCode's answer
// on every legal input and lets the benchmark scale past nine words, the same
// "scale past the strict LeetCode bound to exercise real complexity" convention
// AddTwoNumbers and RelativeSortArray already use.
internal static class SortingTheSentenceSolution
{
    // LeetCode's own input shape: a single space-separated shuffled sentence.
    public static string SortSentenceByMergeSort(string sentence) => JoinInPositionOrder(sentence.Split(' '));

    // The prepared-input overload sorts a copy: MergeSort orders in place, and a
    // benchmark hands the same array to every iteration.
    public static string SortSentenceByMergeSort(string[] words) => JoinInPositionOrder((string[])words.Clone());

    private static string JoinInPositionOrder(string[] words)
    {
        var byPosition = Comparer<string>.Create((a, b) => PositionOf(a).CompareTo(PositionOf(b)));

        MergeSort.Sort<string, ArrayIndexedSequence<string>>(new ArrayIndexedSequence<string>(words), byPosition);

        return string.Join(' ', words.Select(StripPosition));
    }

    public static string SortSentenceByPositionScan(string sentence) => SortSentenceByPositionScan(sentence.Split(' '));

    public static string SortSentenceByPositionScan(string[] words)
    {
        var ordered = new string[words.Length];
        var placed = new bool[words.Length];

        for (var slot = 0; slot < words.Length; slot++)
        {
            var next = LowestUnplaced(words, placed);
            placed[next] = true;
            ordered[slot] = StripPosition(words[next]);
        }

        return string.Join(' ', ordered);
    }

    private static int LowestUnplaced(string[] words, bool[] placed)
    {
        var lowest = -1;

        for (var index = 0; index < words.Length; index++)
        {
            if (IsLowerUnplacedWord(words, placed, index, lowest))
            {
                lowest = index;
            }
        }

        return lowest;
    }

    // A word takes over as the lowest still-unplaced one when it is unplaced and
    // is either the first one seen or sits at a smaller position than the lowest
    // found so far.
    private static bool IsLowerUnplacedWord(string[] words, bool[] placed, int index, int lowest)
        => !placed[index] && (lowest < 0 || PositionOf(words[index]) < PositionOf(words[lowest]));

    private static int PositionOf(string word) => int.Parse(word.AsSpan(DigitsStart(word)));

    private static string StripPosition(string word) => word[..DigitsStart(word)];

    private static int DigitsStart(string word)
    {
        var digitsStart = word.Length;

        while (digitsStart > 0 && char.IsAsciiDigit(word[digitsStart - 1]))
        {
            digitsStart--;
        }

        return digitsStart;
    }
}
