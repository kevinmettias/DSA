using System.Collections.Immutable;

namespace DSAExperimentation.Domain.Language.English.NumberNames;

// English's words for numbers, each a named value carrying its number. Every number below twenty
// has a word of its own, the tens from twenty to ninety have theirs, and then come a hundred and a
// word for each power of a thousand, thousand through quintillion, the last a long reaches. Those
// powers are the short scale, the one English uses today; the long scale and Indian English's lakh
// and crore name the groups of a number's digits differently, and each is a scale of its own
// (INumberScale) that EnglishNumberNames composes words by. The words are lowercase, as written
// within a sentence; a notation that capitalizes them is its user's. Each list is sorted by number
// as it is built, so the order the words are written in below cannot misplace one.
/// <summary>
/// English's number words: zero to nineteen, the tens, a hundred, and the short scale's powers of a
/// thousand.
/// </summary>
public static class EnglishNumberWords
{
    /// <summary>Zero, 0.</summary>
    public static NumberWord Zero { get; } = new(0, "zero");

    /// <summary>One, 1.</summary>
    public static NumberWord One { get; } = new(1, "one");

    /// <summary>Two, 2.</summary>
    public static NumberWord Two { get; } = new(2, "two");

    /// <summary>Three, 3.</summary>
    public static NumberWord Three { get; } = new(3, "three");

    /// <summary>Four, 4.</summary>
    public static NumberWord Four { get; } = new(4, "four");

    /// <summary>Five, 5.</summary>
    public static NumberWord Five { get; } = new(5, "five");

    /// <summary>Six, 6.</summary>
    public static NumberWord Six { get; } = new(6, "six");

    /// <summary>Seven, 7.</summary>
    public static NumberWord Seven { get; } = new(7, "seven");

    /// <summary>Eight, 8.</summary>
    public static NumberWord Eight { get; } = new(8, "eight");

    /// <summary>Nine, 9.</summary>
    public static NumberWord Nine { get; } = new(9, "nine");

    /// <summary>Ten, 10.</summary>
    public static NumberWord Ten { get; } = new(10, "ten");

    /// <summary>Eleven, 11.</summary>
    public static NumberWord Eleven { get; } = new(11, "eleven");

    /// <summary>Twelve, 12.</summary>
    public static NumberWord Twelve { get; } = new(12, "twelve");

    /// <summary>Thirteen, 13.</summary>
    public static NumberWord Thirteen { get; } = new(13, "thirteen");

    /// <summary>Fourteen, 14.</summary>
    public static NumberWord Fourteen { get; } = new(14, "fourteen");

    /// <summary>Fifteen, 15.</summary>
    public static NumberWord Fifteen { get; } = new(15, "fifteen");

    /// <summary>Sixteen, 16.</summary>
    public static NumberWord Sixteen { get; } = new(16, "sixteen");

    /// <summary>Seventeen, 17.</summary>
    public static NumberWord Seventeen { get; } = new(17, "seventeen");

    /// <summary>Eighteen, 18.</summary>
    public static NumberWord Eighteen { get; } = new(18, "eighteen");

    /// <summary>Nineteen, 19.</summary>
    public static NumberWord Nineteen { get; } = new(19, "nineteen");

    /// <summary>Twenty, 20.</summary>
    public static NumberWord Twenty { get; } = new(20, "twenty");

    /// <summary>Thirty, 30.</summary>
    public static NumberWord Thirty { get; } = new(30, "thirty");

    /// <summary>Forty, 40.</summary>
    public static NumberWord Forty { get; } = new(40, "forty");

    /// <summary>Fifty, 50.</summary>
    public static NumberWord Fifty { get; } = new(50, "fifty");

    /// <summary>Sixty, 60.</summary>
    public static NumberWord Sixty { get; } = new(60, "sixty");

    /// <summary>Seventy, 70.</summary>
    public static NumberWord Seventy { get; } = new(70, "seventy");

    /// <summary>Eighty, 80.</summary>
    public static NumberWord Eighty { get; } = new(80, "eighty");

    /// <summary>Ninety, 90.</summary>
    public static NumberWord Ninety { get; } = new(90, "ninety");

    /// <summary>A hundred, 100.</summary>
    public static NumberWord Hundred { get; } = new(100, "hundred");

    /// <summary>A thousand, 10^3.</summary>
    public static NumberWord Thousand { get; } = new(1_000, "thousand");

    /// <summary>A million, 10^6.</summary>
    public static NumberWord Million { get; } = new(1_000_000, "million");

    /// <summary>A billion on the short scale, 10^9.</summary>
    public static NumberWord Billion { get; } = new(1_000_000_000, "billion");

    /// <summary>A trillion on the short scale, 10^12.</summary>
    public static NumberWord Trillion { get; } = new(1_000_000_000_000, "trillion");

    /// <summary>A quadrillion on the short scale, 10^15.</summary>
    public static NumberWord Quadrillion { get; } = new(1_000_000_000_000_000, "quadrillion");

    /// <summary>A quintillion on the short scale, 10^18.</summary>
    public static NumberWord Quintillion { get; } = new(1_000_000_000_000_000_000, "quintillion");

    /// <summary>
    /// Zero to nineteen, the numbers with a word of their own, each at the index of its number.
    /// </summary>
    public static ImmutableArray<NumberWord> OwnWords { get; } = SortedByNumber(
        Zero, One, Two, Three, Four, Five, Six, Seven, Eight, Nine,
        Ten, Eleven, Twelve, Thirteen, Fourteen, Fifteen, Sixteen, Seventeen, Eighteen, Nineteen);

    /// <summary>
    /// The tens a larger number is composed from, twenty to ninety, in increasing order.
    /// </summary>
    public static ImmutableArray<NumberWord> Tens { get; } = SortedByNumber(
        Twenty, Thirty, Forty, Fifty, Sixty, Seventy, Eighty, Ninety);

    /// <summary>
    /// The short scale's word for each power of a thousand, thousand to quintillion, in increasing
    /// order.
    /// </summary>
    public static ImmutableArray<NumberWord> ShortScale { get; } = SortedByNumber(
        Thousand, Million, Billion, Trillion, Quadrillion, Quintillion);

    private static ImmutableArray<NumberWord> SortedByNumber(params NumberWord[] words)
        => [.. words.OrderBy(word => word.Value)];
}
