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
public sealed partial class LayeringTests
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

    // The one project laid out in tier folders. DSAExperimentation.LeetCode is the top
    // tier in a project of its own, so nothing it could reference sits above it - a row
    // for it here could never fail, and the boundary it does have is the compiler's.
    private const string FrameworkProject = "DSAExperimentation";

    [Fact]
    public void EveryFile_ReferencesItsOwnTierOrALowerOne()
    {
        var offences = Inversions();
        var report = string.Join(Environment.NewLine, offences);

        Assert.True(offences.Count == 0, report);
    }

    // A sweep that found no tiered file would report no inversions while checking nothing.
    [Fact]
    public void TierSweep_OverTheFrameworkProject_FindsFilesInEveryLowerTier()
    {
        var root = RepositoryFiles.Root();
        var tiersFound = RepositoryFiles.SourceFilesIn(Path.Combine(root, FrameworkProject))
            .Select(file => TierOf(RepositoryFiles.PathFromRoot(root, file)))
            .OfType<string>()
            .ToHashSet();

        Assert.Equal(Tiers[..^1], Tiers.Where(tiersFound.Contains));
    }

    private static List<string> Inversions()
    {
        var root = RepositoryFiles.Root();
        var offences = new List<string>();

        foreach (var file in RepositoryFiles.SourceFilesIn(Path.Combine(root, FrameworkProject)))
        {
            var relative = RepositoryFiles.PathFromRoot(root, file);
            var tier = TierOf(relative);

            if (tier is not null)
            {
                offences.AddRange(InversionsIn(new TieredFile(file, relative, tier)));
            }
        }

        return offences;
    }

    [Fact]
    public void AllowedInversions_AreAllStillPresent()
    {
        // A stale entry would silently widen the rule; delete it once the edge is gone.
        var root = RepositoryFiles.Root();

        foreach (var path in AllowedInversions.Keys)
        {
            var relativePath = path.Replace('/', Path.DirectorySeparatorChar);
            var full = Path.Combine(root, "DSAExperimentation", relativePath);

            Assert.True(File.Exists(full), $"{path} is allow-listed as a deliberate tier inversion but no longer exists.");
        }
    }

    [Fact]
    public void LeetCodeTier_LivesInItsOwnProject()
    {
        // The one boundary that IS compiler-enforced: nothing in the framework
        // project may depend on a LeetCode solution, because it cannot see one.
        var root = RepositoryFiles.Root();
        var frameworkLeetCode = Path.Combine(root, "DSAExperimentation", "LeetCode");

        Assert.False(
            Directory.Exists(frameworkLeetCode),
            "LeetCode solutions belong in DSAExperimentation.LeetCode, not the framework project.");
    }

    // The tier folder a framework file's path names directly, or null when it names none.
    private static string? TierOf(string relative)
    {
        var segments = relative[(FrameworkProject.Length + 1)..].Split('/');

        if (segments.Length <= 1 || !Tiers.Contains(segments[0]))
        {
            return null;
        }

        return segments[0];
    }

    private static IEnumerable<string> InversionsIn(TieredFile file)
    {
        if (AllowedInversions.ContainsKey(TrimProject(file.RelativePath)))
        {
            yield break;
        }

        var rank = Array.IndexOf(Tiers, file.Tier);

        foreach (Match match in Reference.Matches(File.ReadAllText(file.FullPath)))
        {
            var referencedRank = Array.IndexOf(Tiers, match.Groups[1].Value);

            if (referencedRank > rank)
            {
                yield return $"{file.RelativePath}: {file.Tier} references {match.Groups[1].Value}, a higher tier.";
            }
        }
    }

    private static string TrimProject(string relative)
    {
        var slash = relative.IndexOf('/');

        if (slash < 0)
        {
            return relative;
        }

        return relative[(slash + 1)..];
    }

    // One source file under a tier project, named three ways: the absolute path to read, the
    // root-relative path a finding states it by, and the tier its location puts it in. Three
    // adjacent strings would let a call site transpose them and still compile.
    public readonly record struct TieredFile(string FullPath, string RelativePath, string Tier);
}
