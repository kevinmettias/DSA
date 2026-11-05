using DSAExperimentation.Domain.Language.English.Phonology;
using DSAExperimentation.Domain.Language.Phonetics;

namespace DSAExperimentation.Tests.Domain.Language.Phonetics;

// Read through Received Pronunciation's inventory: a run of symbols that is one phoneme reads as
// it, so a diphthong, a long vowel and an affricate each come back whole, and a symbol the accent
// has no phoneme for is refused rather than guessed at.
public sealed partial class PhonemicTranscriptionTests
{
    public static TheoryData<string, string[]> Words => new()
    {
        { "ʃɪp", ["ʃ", "ɪ", "p"] },
        { "maɪ", ["m", "aɪ"] },
        { "fliːs", ["f", "l", "iː", "s"] },
        { "t͡ʃɜːt͡ʃ", ["t͡ʃ", "ɜː", "t͡ʃ"] },
        { "θɪŋk", ["θ", "ɪ", "ŋ", "k"] },
        { "jes", ["j", "e", "s"] },
        { string.Empty, [] },
    };

    // x is the fricative of Scottish "loch", no phoneme of this accent; a is only the start of a
    // diphthong in it; and the slashes around a transcription are the caller's to strip.
    public static TheoryData<string> NoPhonemes => new() { "lɒx", "a", "/ʃɪp/" };

    [Theory]
    [MemberData(nameof(Words))]
    public void Segment_Transcription_ReadsTheLongestPhonemeAtEachPlace(string transcription, string[] expected) =>
        Assert.Equal(expected, PhonemicTranscription<ReceivedPronunciation>.Segment(transcription));

    [Theory]
    [MemberData(nameof(NoPhonemes))]
    public void Segment_SymbolNoPhonemeOfTheInventory_Throws(string transcription) =>
        Assert.Throws<ArgumentException>(() => PhonemicTranscription<ReceivedPronunciation>.Segment(transcription));
}
