using System.Text.RegularExpressions;

namespace DSAExperimentation.Tests.Architecture;

// Per-call state belongs to the call. A hook or algebra that needs a runtime value carries it
// as a field of the struct the engine is handed, and the engine hands the value back
// (ARCHITECTURE section 12.4). An AsyncLocal, ThreadLocal or [ThreadStatic] slot in the library
// or the solutions would be that value smuggled past the contract instead - state a reader
// cannot see from the call site, and exactly what the hook and algebra contracts became
// instance members to retire. Comments and strings are blanked first, so prose that names the
// mechanism is not mistaken for code that uses it. Benchmarks and tests are harnesses and are
// not scanned.
public sealed partial class AmbientStateTests
{
    private static readonly string[] ScannedProjects = ["DSAExperimentation", "DSAExperimentation.LeetCode"];

    private static readonly Regex AmbientSlot = new(@"\b(?:AsyncLocal|ThreadLocal)\s*<|\[\s*ThreadStatic\s*\]");

    public static TheoryData<string> SlotsDeclaredInCode =>
        new()
        {
            "private static readonly AsyncLocal<int> Slot = new();",
            "private static readonly ThreadLocal<int> Slot = new();",
            "[ThreadStatic] private static int slot;",
        };

    public static TheoryData<string> SlotsNamedOnlyInACommentOrString =>
        new() { "// the old hook kept this in an AsyncLocal<int>", "var message = \"no [ThreadStatic] here\";" };

    [Fact]
    public void LibraryAndSolutions_HoldNoAmbientPerCallState()
    {
        var root = RepositoryFiles.Root();
        var offences = ScannedProjects
            .SelectMany(project => RepositoryFiles.SourceFilesIn(Path.Combine(root, project)))
            .Where(file => HasAmbientSlot(File.ReadAllText(file)))
            .Select(file => RepositoryFiles.PathFromRoot(root, file))
            .ToList();

        Assert.True(offences.Count == 0, string.Join(Environment.NewLine, offences));
    }

    [Theory]
    [MemberData(nameof(SlotsDeclaredInCode))]
    public void HasAmbientSlot_SlotDeclaredInCode_IsFound(string source) => Assert.True(HasAmbientSlot(source));

    [Theory]
    [MemberData(nameof(SlotsNamedOnlyInACommentOrString))]
    public void HasAmbientSlot_SlotNamedOnlyInACommentOrString_IsNotFound(string source) =>
        Assert.False(HasAmbientSlot(source));

    private static bool HasAmbientSlot(string source) => AmbientSlot.IsMatch(RepositoryFiles.CodeOf(source));
}
