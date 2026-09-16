using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for NumberOfValidWordsForEachPuzzleWorkloads's nested WordSource.
//
// WordSource is a private nested type, so no test class can name it in code. Coverage is attributed
// by TYPE, so the nested unit needs a class named after it, and the only surface that runs
// WordSource.Next is the workload's BuildWords, which asks it for one entry per slot. These tests
// therefore drive BuildWords and assert what WordSource owns: the generator's own comment says each
// source carries its kind's size rule, and a word's rule is a *random* length in [3, 8], drawn from
// the Random the two sources share - not the fixed width its sibling uses.
//
// That is the part the builder-level tests leave open. They check a length band, which a Next that
// took one constant length would also satisfy; the second test pins the variation itself, so a
// source that stopped drawing its length cannot pass both.
public sealed partial class WordSourceTests
{
    private const int WordCount = 200;
    private const int WordSeed = 1178; // LC problem number, reused as the workload seed
    private const int MinWordLength = 3;
    private const int WordLengthUpperBound = 9; // exclusive; word length ranges [3, 8]
    private const int LowestDistinctLengthCount = 2;

    [Fact]
    public void Next_EveryEntry_HasDistinctLettersWithinTheDocumentedLengthBand() =>
        Assert.All(BuildWords(), word =>
        {
            Assert.InRange(word.Length, MinWordLength, WordLengthUpperBound - 1);
            Assert.Equal(word.Length, word.Distinct().Count());
        });

    [Fact]
    public void Next_TwoHundredEntries_DrawAVaryingLengthRatherThanAFixedOne()
    {
        var distinctLengths = BuildWords().Select(word => word.Length).Distinct().Count();

        Assert.InRange(distinctLengths, LowestDistinctLengthCount, WordLengthUpperBound - MinWordLength);
    }

    private static string[] BuildWords() =>
        NumberOfValidWordsForEachPuzzleWorkloads.BuildWords(WordCount, new Random(WordSeed));
}
