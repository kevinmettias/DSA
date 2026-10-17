using System.Text.RegularExpressions;

namespace DSAExperimentation.Tests.Architecture;

// How SectionCitationTests reads a citation of an ARCHITECTURE.md section and a numbered heading of
// it, with each piece of the citation pattern under its own name so it can be read on its own.
internal static class SectionCitationGrammar
{
    // A section number: dotted digits with at most one letter suffix (17.6a) that starts no word.
    public const string Number = @"\d+(?:\.\d+)*(?:[a-z](?![A-Za-z]))?";

    // The one form a list may continue with when its next number carries no mark of its own; the
    // dot is what keeps a count after a comma from reading as a section.
    public const string DottedNumber = @"\d+\.\d+(?:\.\d+)*(?:[a-z](?![A-Za-z]))?";

    // A comment that wraps onto its next line between the name and the number.
    public const string CommentWrap = @"[ \t]*\r?\n[ \t]*(?://+|\*)[ \t]*";

    public const string SectionWord = $@"\b[Ss]ections?(?:{CommentWrap}|[ \t]+)";

    public const string DocumentName =
        $@"\bARCHITECTURE(?:\.md)?(?:'s)?(?:{CommentWrap}|[ \t]*)(?:(?:§|#|Sec\.?|[Ss]ections?)[ \t]*)?";

    public const string ListContinuation =
        $@"(?:[ \t]*(?:/|,|&|-|–|\band\b|\bor\b)[ \t]*(?:(?:§|#)[ \t]*(?<number>{Number})|(?<number>{DottedNumber})))*";

    public static readonly Regex Citation =
        new($@"(?:§[ \t]*|{SectionWord}|{DocumentName})(?<number>{Number}){ListContinuation}");

    public static readonly Regex Heading =
        new(@"^#{1,6}[ \t]+(?<number>\d+(?:\.\d+)*[a-z]?)[.\s]", RegexOptions.Multiline);
}
