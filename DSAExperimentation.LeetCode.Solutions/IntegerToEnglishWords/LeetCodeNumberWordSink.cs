using System.Text;
using DSAExperimentation.Domain.Language;
using DSAExperimentation.Domain.Language.English.NumberNames;
using DSAExperimentation.Domain.Language.English.NumberNames.Composition;
using DSAExperimentation.Domain.Mathematics.Numerals;
using Vocabulary =
    DSAExperimentation.LeetCode.Solutions.IntegerToEnglishWords.IntegerToEnglishWordsVocabulary;

namespace DSAExperimentation.LeetCode.Solutions.IntegerToEnglishWords;

// Where the composer arm writes English's words in LC 273's notation: each word capitalized, one
// space between words, and no hyphen in "Twenty One". The composer hands over each word with the
// number it names, and the word's spelling is looked up by that number in a table built once from
// the vocabulary's own tables, so the composer arm spells every word as the two arms beside it do.
internal struct LeetCodeNumberWordSink(StringBuilder text) : INumberWordSink
{
    private static readonly Dictionary<long, string> Spellings = SpellingsByNumber();

    public readonly StringBuilder Text => text;

    public readonly void Append(NumberWord word)
    {
        if (text.Length > 0)
        {
            text.Append(Vocabulary.WordSeparator);
        }

        text.Append(Spellings[word.Value]);
    }

    // Zero to nineteen, the tens, a hundred and the short scale's words, each by its number. The
    // vocabulary's scales are indexed by group, ones first, so the short scale's k-th word is its
    // (k + 1)-th.
    private static Dictionary<long, string> SpellingsByNumber()
    {
        var spellings = new Dictionary<long, string>();

        for (var number = 0; number < Vocabulary.Below20.Length; number++)
        {
            spellings[number] = Vocabulary.Below20[number];
        }

        foreach (var tens in EnglishNumberWords.Tens)
        {
            spellings[tens.Value] = Vocabulary.Tens[tens.Value / DecimalDigits.Size];
        }

        spellings[EnglishNumberWords.Hundred.Value] = Vocabulary.HundredWord;

        for (var group = 0; group < EnglishNumberWords.ShortScale.Length; group++)
        {
            spellings[EnglishNumberWords.ShortScale[group].Value] = Vocabulary.Scales[group + 1];
        }

        return spellings;
    }
}
