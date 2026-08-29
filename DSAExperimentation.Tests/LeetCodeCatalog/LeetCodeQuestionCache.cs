using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DSAExperimentation.Tests.LeetCodeCatalog;

// One JSON file per question under Fixtures/, keyed by title slug - checked into
// source control so every normal test run reads a fixed, offline snapshot instead
// of calling the live API (see LeetCodeApiClient.cs's own doc comment for why
// that call stays out of the ordinary `dotnet test` path entirely). Locates
// Fixtures/ via [CallerFilePath] rather than AppContext.BaseDirectory
// specifically so this resolves correctly from the SOURCE tree regardless of
// build output layout (bin/Debug/net10.0/... has no Fixtures/ of its own) - no
// csproj CopyToOutputDirectory wiring needed.
internal static class LeetCodeQuestionCache
{
    private const string FixturesFolderName = "Fixtures";
    private const string FixtureFileSearchPattern = "*.json";

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() },
    };

    private static readonly string FixturesDirectory = ResolveFixturesDirectory();

    public static bool TryLoad(string titleSlug, out LeetCodeQuestion question)
    {
        var path = PathFor(titleSlug);

        if (!File.Exists(path))
        {
            // presumption: allow -- question is only meaningful when this returns
            // true, the standard TryGetValue/TryParse out-parameter contract this
            // mirrors.
            question = null!;
            return false;
        }

        question = DeserializeQuestion(File.ReadAllText(path));
        return true;
    }

    public static void Save(LeetCodeQuestion question)
    {
        Directory.CreateDirectory(FixturesDirectory);
        var json = JsonSerializer.Serialize(question, SerializerOptions);
        File.WriteAllText(PathFor(question.TitleSlug), json);
    }

    public static IReadOnlyList<string> ListCachedTitleSlugs()
        => Directory.Exists(FixturesDirectory) ? FixtureFileNames() : NoCachedSlugs();

    private static List<string> NoCachedSlugs() => [];

    private static List<string> FixtureFileNames()
        => Directory.GetFiles(FixturesDirectory, FixtureFileSearchPattern)
            .Select(FileNameWithoutExtension)
            .ToList();

    // presumption: allow -- path came from Directory.GetFiles matching
    // FixtureFileSearchPattern, so it always names a real file with a ".json"
    // extension, never a bare root path with none.
    private static string FileNameWithoutExtension(string path) => Path.GetFileNameWithoutExtension(path)!;

    // presumption: allow -- the JSON on disk here is always something this same
    // class's own Save wrote, in the shape LeetCodeQuestion declares.
    private static LeetCodeQuestion DeserializeQuestion(string json)
        => JsonSerializer.Deserialize<LeetCodeQuestion>(json, SerializerOptions)!;

    private static string PathFor(string titleSlug) => Path.Combine(FixturesDirectory, $"{titleSlug}.json");

    // presumption: allow -- sourceFilePath is supplied by the compiler via
    // [CallerFilePath], always an absolute path to this very file, which always
    // has a parent directory.
    private static string ResolveFixturesDirectory([CallerFilePath] string sourceFilePath = "")
        => Path.Combine(Path.GetDirectoryName(sourceFilePath)!, FixturesFolderName);
}
