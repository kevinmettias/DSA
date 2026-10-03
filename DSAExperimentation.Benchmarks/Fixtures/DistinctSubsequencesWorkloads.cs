namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 115: a target of random English letters with no
// letter repeated back to back, and a source that is that target with three of its
// letters doubled in place. LC 115 promises the count fits in a 32-bit int, and this
// shape pins it exactly, however long the strings are. Source and target spell the
// same runs of equal letters, every target run is one letter long, and an occurrence
// of the target takes exactly one letter from each source run - taking two from one
// run would need two equal neighbours in the target - so the count is the product of
// the source's run lengths, 2 * 2 * 2 = 8. Nothing about the shape prunes either
// recurrence: the rolling row still fills the whole source-by-target table, and the
// memoized recursion still visits every pair whose source prefix holds its target
// prefix - roughly half of it, since the two strings nearly coincide.
internal static class DistinctSubsequencesWorkloads
{
    public const int DoubledLetterCount = 3;

    private const string Letters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

    public static (string Source, string Target) Build(int sourceLength, Random random)
    {
        var target = new char[sourceLength - DoubledLetterCount];

        for (var i = 0; i < target.Length; i++)
        {
            // A draw from every letter but the last stands in for the last when it repeats the
            // previous letter, which keeps each draw uniform over the letters that do not.
            var letter = Letters[random.Next(Letters.Length - 1)];
            target[i] = i > 0 && letter == target[i - 1] ? Letters[^1] : letter;
        }

        var doubled = SeededSequences.ShuffledZeroTo(target.Length, random).Take(DoubledLetterCount).ToHashSet();
        var source = new List<char>(sourceLength);

        for (var i = 0; i < target.Length; i++)
        {
            source.Add(target[i]);

            if (doubled.Contains(i))
            {
                source.Add(target[i]);
            }
        }

        return (new string([.. source]), new string(target));
    }
}
