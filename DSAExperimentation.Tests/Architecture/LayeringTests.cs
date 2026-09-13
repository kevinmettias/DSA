using System.Text.RegularExpressions;

namespace DSAExperimentation.Tests.Architecture;

// Enforces ARCHITECTURE.md section 17.2's tier order, which is otherwise only a
// convention the file tree implies: a tier may depend downward and never upward.
//
// This is the guard a project-per-tier split would have bought from the compiler.
// It is deliberately a test instead, for two reasons the split could not answer:
// every type here is `internal` and several designs depend on single-assembly
// encapsulation (see IVisitGuard's own doc comment), and one upward edge is
// genuinely intended - IntervalSet composes BinarySearch rather than duplicating
// its bisection loop. A ProjectReference cannot say "this one edge, for this
// reason"; the allow-list below can.
public sealed class LayeringTests
{
    // Lowest tier first. A file in tier i may reference tiers 0..i only.
    private static readonly string[] Tiers = ["DataStructures", "Algorithms", "Domain", "LeetCode"];

    // Upward edges that are deliberate, each with the reason it is allowed.
    private static readonly Dictionary<string, string> AllowedInversions = new()
    {
        ["DataStructures/IntervalSet/IntervalSet.cs"] =
            "IntervalSet keeps its intervals sorted and locates candidates with BinarySearch.LowerBound "
            + "through a private IRandomAccessSequence view, rather than carrying a second bisection loop. "
            + "Documented in the type's own doc comment.",
    };

    private static readonly Regex Reference = new(
        @"^using(?:\s+\w+\s*=)?\s+DSAExperimentation\.(\w+)", RegexOptions.Multiline);

    public static TheoryData<string, string> SourceRoots =>
        new() { { "DSAExperimentation", "" }, { "DSAExperimentation.LeetCode", "LeetCode" } };

    [Theory]
    [MemberData(nameof(SourceRoots))]
    public void EveryFile_ReferencesItsOwnTierOrALowerOne(string projectDirectory, string fixedTier)
    {
        var root = RepositoryFiles.Root();
        var offences = new List<string>();

        foreach (var file in RepositoryFiles.SourceFilesIn(Path.Combine(root, projectDirectory)))
        {
            var relative = RepositoryFiles.PathFromRoot(root, file);
            var tier = fixedTier.Length > 0 ? fixedTier : TierOf(relative, projectDirectory);

            if (tier is not null)
            {
                offences.AddRange(InversionsIn(file, relative, tier));
            }
        }

        Assert.True(offences.Count == 0, string.Join(Environment.NewLine, offences));
    }

    [Fact]
    public void AllowedInversions_AreAllStillPresent()
    {
        // A stale entry would silently widen the rule; delete it once the edge is gone.
        var root = RepositoryFiles.Root();

        foreach (var path in AllowedInversions.Keys)
        {
            var full = Path.Combine(root, "DSAExperimentation", path.Replace('/', Path.DirectorySeparatorChar));

            Assert.True(File.Exists(full), $"{path} is allow-listed as a deliberate tier inversion but no longer exists.");
        }
    }

    [Fact]
    public void LeetCodeTier_LivesInItsOwnProject()
    {
        // The one boundary that IS compiler-enforced: nothing in the framework
        // project may depend on a LeetCode solution, because it cannot see one.
        var root = RepositoryFiles.Root();

        Assert.False(
            Directory.Exists(Path.Combine(root, "DSAExperimentation", "LeetCode")),
            "LeetCode solutions belong in DSAExperimentation.LeetCode, not the framework project.");
    }

    private static IEnumerable<string> InversionsIn(string file, string relative, string tier)
    {
        if (AllowedInversions.ContainsKey(TrimProject(relative)))
        {
            yield break;
        }

        var rank = Array.IndexOf(Tiers, tier);

        foreach (Match match in Reference.Matches(File.ReadAllText(file)))
        {
            var referencedRank = Array.IndexOf(Tiers, match.Groups[1].Value);

            if (referencedRank > rank)
            {
                yield return $"{relative}: {tier} references {match.Groups[1].Value}, a higher tier.";
            }
        }
    }

    private static string TrimProject(string relative)
    {
        var slash = relative.IndexOf('/');

        return slash < 0 ? relative : relative[(slash + 1)..];
    }

    private static string? TierOf(string relative, string projectDirectory)
    {
        var segments = relative[(projectDirectory.Length + 1)..].Split('/');

        return segments.Length > 1 && Tiers.Contains(segments[0]) ? segments[0] : null;
    }
}
