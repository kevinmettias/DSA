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

    public static TheoryData<SourceRoot> SourceRoots =>
        new()
        {
            { new SourceRoot(ProjectDirectory: "DSAExperimentation", FixedTier: "") },
            { new SourceRoot(ProjectDirectory: "DSAExperimentation.LeetCode", FixedTier: "LeetCode") },
        };

    [Theory]
    [MemberData(nameof(SourceRoots))]
    public void EveryFile_ReferencesItsOwnTierOrALowerOne(SourceRoot sourceRoot)
    {
        var offences = InversionsUnder(sourceRoot);
        var report = string.Join(Environment.NewLine, offences);

        Assert.True(offences.Count == 0, report);
    }

    private static List<string> InversionsUnder(SourceRoot sourceRoot)
    {
        var root = RepositoryFiles.Root();
        var offences = new List<string>();
        var projectDirectory = Path.Combine(root, sourceRoot.ProjectDirectory);

        foreach (var file in RepositoryFiles.SourceFilesIn(projectDirectory))
        {
            var relative = RepositoryFiles.PathFromRoot(root, file);
            var tier = TierUnder(sourceRoot, relative);

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

    // The tier a file sits in: the source root's own fixed tier where it declares one (a
    // project whose whole body is a single tier), otherwise the tier folder its path
    // names directly, and null when the path names no such folder.
    private static string? TierUnder(SourceRoot sourceRoot, string relative)
    {
        if (sourceRoot.FixedTier.Length > 0)
        {
            return sourceRoot.FixedTier;
        }

        var segments = relative[(sourceRoot.ProjectDirectory.Length + 1)..].Split('/');

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

    // One source root the sweep walks: the project directory, and the tier its files are all
    // in where the whole project is a single tier (empty for a project laid out in tier
    // folders). The two positions are both `string` and mean different things, so each is
    // named rather than left interchangeable.
    public readonly record struct SourceRoot(string ProjectDirectory, string FixedTier);

    // One source file under a tier project, named three ways: the absolute path to read, the
    // root-relative path a finding states it by, and the tier its location puts it in. Three
    // adjacent strings would let a call site transpose them and still compile.
    public readonly record struct TieredFile(string FullPath, string RelativePath, string Tier);
}
