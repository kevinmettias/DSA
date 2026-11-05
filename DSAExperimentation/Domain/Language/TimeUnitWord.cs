namespace DSAExperimentation.Domain.Language;

// A unit of time and the word a language names it with: "minute" names the unit of 60 seconds. The
// unit is given by its length, in TimeSpan's ticks as Science/Time states it, since each of
// TimeUnit's units has a length no other has; that length is the one source a table of words is
// built from, so a table places each word at its own unit and cannot disagree with it.
/// <summary>A unit of time, by its length, and the word that names it.</summary>
/// <param name="Ticks">The length of the unit the word names, in TimeSpan's ticks.</param>
/// <param name="Word">The word, lowercase and singular, as it is written within a sentence.</param>
public readonly record struct TimeUnitWord(long Ticks, string Word);
