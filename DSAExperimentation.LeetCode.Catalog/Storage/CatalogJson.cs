using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text.Encodings.Web;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace DSAExperimentation.LeetCode.Catalog.Storage;

// How the catalog's snapshot is stored, shared by every kind of file in it:
//
//   Snapshot/Questions/<slug>.json          a LeetCodeQuestion (QuestionFiles)
//   Snapshot/Statistics.json                every question's LeetCodeStatistics, as of one date (StatisticsFile)
//   Snapshot/ContestRatings.json            the contest ratings of the questions that have one (ContestRatingsFile)
//   Snapshot/Cases/<source>/<slug>.json     the cases one source supplies for one question (CaseFiles)
//
// Snapshot/ is found from this file's own path ([CallerFilePath]), so a test, a benchmark and the
// sync tool all read and write the checked-in source tree whatever their build output layout.
//
// A write is skipped when the text would not change, so a refresh that finds nothing new leaves the
// tree clean, and one that does shows only the lines that moved.
internal static class CatalogJson
{
    private const string SnapshotFolderName = "Snapshot";
    private const string ParentFolder = "..";
    private const string JsonExtension = ".json";
    private const string JsonSearchPattern = "*.json";

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        Converters = { new JsonStringEnumConverter() },
    };

    public static string SnapshotDirectory { get; } = ResolveSnapshotDirectory();

    public static string PathOf(string folder, string titleSlug) => Path.Combine(folder, $"{titleSlug}{JsonExtension}");

    public static IReadOnlyList<string> SlugsIn(string folder)
    {
        if (!Directory.Exists(folder))
        {
            return [];
        }

        return Directory.GetFiles(folder, JsonSearchPattern)
            .Select(Path.GetFileNameWithoutExtension)
            .OfType<string>()
            .Order(StringComparer.Ordinal)
            .ToList();
    }

    public static bool TryRead<TDocument>(string path, [MaybeNullWhen(false)] out TDocument document)
    {
        if (!File.Exists(path))
        {
            document = default;
            return false;
        }

        document = Deserialize<TDocument>(File.ReadAllText(path));
        return true;
    }

    public static bool TryWrite<TDocument>(string path, TDocument document)
    {
        var json = JsonSerializer.Serialize(document, SerializerOptions) + Environment.NewLine;

        if (File.Exists(path) && File.ReadAllText(path) == json)
        {
            return false;
        }

        Directory.CreateDirectory(FolderOf(path));
        File.WriteAllText(path, json);
        return true;
    }

    // presumption: allow -- every path here is built by Path.Combine under SnapshotDirectory, so it
    // always has a parent folder.
    private static string FolderOf(string path) => Path.GetDirectoryName(path)!;

    // presumption: allow -- every file under Snapshot/ is one TryWrite produced, in the shape the
    // reader asks for; a file that deserializes to null would be a hand-edited "null".
    private static TDocument Deserialize<TDocument>(string json) => JsonSerializer.Deserialize<TDocument>(json, SerializerOptions)!;

    // Snapshot/ sits beside Storage/, the folder this file is in, at the project's root.
    // presumption: allow -- sourceFilePath is supplied by the compiler via [CallerFilePath], always
    // an absolute path to this very file, which always has a parent directory.
    private static string ResolveSnapshotDirectory([CallerFilePath] string sourceFilePath = "")
    {
        var storageFolder = Path.GetDirectoryName(sourceFilePath)!;
        var snapshotFolder = Path.Combine(storageFolder, ParentFolder, SnapshotFolderName);
        return Path.GetFullPath(snapshotFolder);
    }
}
