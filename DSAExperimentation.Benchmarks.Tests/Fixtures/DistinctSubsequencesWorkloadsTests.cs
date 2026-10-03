using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for DistinctSubsequencesWorkloads (ARCHITECTURE 17.7). LC 115's benchmark pins
// its count at exactly eight, and that rests on the shape the fixture's comment describes: a target
// with no letter repeated back to back, and a source spelling the same runs with exactly three of
// them doubled. Both halves of that are asserted here through the strings' runs, at LeetCode's
// 1,000-character cap.
public sealed partial class DistinctSubsequencesWorkloadsTests
{
    private const int SourceLength = 1_000;
    private const int Seed = 115; // LC problem number
    private const int DoubledRunLength = 2;

    [Fact]
    public void Build_SourceLength_ReturnsASourceThreeLettersLongerThanTheTarget()
    {
        var (source, target) = Build();

        Assert.Equal(SourceLength, source.Length);
        Assert.Equal(SourceLength - DistinctSubsequencesWorkloads.DoubledLetterCount, target.Length);
        Assert.All(source + target, letter => Assert.True(char.IsAsciiLetter(letter)));
    }

    [Fact]
    public void Build_Target_NeverRepeatsALetterBackToBack() =>
        Assert.All(RunsOf(Build().Target), run => Assert.Equal(1, run.Length));

    [Fact]
    public void Build_Source_SpellsTheTargetsRunsWithExactlyThreeDoubled()
    {
        var (source, target) = Build();
        var sourceRuns = RunsOf(source);

        Assert.Equal(RunsOf(target).Select(run => run.Letter), sourceRuns.Select(run => run.Letter));
        Assert.Equal(DistinctSubsequencesWorkloads.DoubledLetterCount, sourceRuns.Count(run => run.Length == DoubledRunLength));
        Assert.All(sourceRuns, run => Assert.InRange(run.Length, 1, DoubledRunLength));
    }

    [Fact]
    public void Build_SameSeed_ReturnsTheSameStrings() =>
        Assert.Equal(Build(), Build());

    private static (string Source, string Target) Build() =>
        DistinctSubsequencesWorkloads.Build(SourceLength, new Random(Seed));

    private static List<(char Letter, int Length)> RunsOf(string text)
    {
        var runs = new List<(char Letter, int Length)>();

        foreach (var letter in text)
        {
            if (runs.Count > 0 && runs[^1].Letter == letter)
            {
                runs[^1] = (letter, runs[^1].Length + 1);
            }
            else
            {
                runs.Add((letter, 1));
            }
        }

        return runs;
    }
}
