using System.Text.Json;

namespace DSAExperimentation.LeetCode.Catalog.Sync.Dataset;

// LeetCodeDataset (huggingface.co/datasets/newfacade/LeetCodeDataset, Apache-2.0): about 2,900
// questions with roughly a hundred generated cases each, split into a train and a test file that
// together cover the questions once. Rows are read one line at a time - the train file is about
// 90 MB - keeping only the slug and the cases.
internal static class LeetCodeDataset
{
    private const string HttpScheme = "http";

    // Long enough to fetch the 90 MB train file over a slow connection.
    private static readonly TimeSpan DownloadTimeout = TimeSpan.FromMinutes(10);

    public static IReadOnlyList<string> DefaultSources { get; } =
    [
        "https://huggingface.co/datasets/newfacade/LeetCodeDataset/resolve/main/LeetCodeDataset-train.jsonl",
        "https://huggingface.co/datasets/newfacade/LeetCodeDataset/resolve/main/LeetCodeDataset-test.jsonl",
    ];

    // Each source is a local path or an http(s) URL; the first row for a slug wins.
    public static async Task<IReadOnlyDictionary<string, DatasetRowDto>> ReadRowsAsync(
        IReadOnlyList<string> sources, CancellationToken cancellationToken)
    {
        var rows = new Dictionary<string, DatasetRowDto>(StringComparer.Ordinal);
        using var client = new HttpClient { Timeout = DownloadTimeout };

        foreach (var source in sources)
        {
            await using var stream = await OpenAsync(client, source, cancellationToken);
            await ReadInto(rows, stream, cancellationToken);
        }

        return rows;
    }

    private static async Task<Stream> OpenAsync(HttpClient client, string source, CancellationToken cancellationToken)
    {
        if (!source.StartsWith(HttpScheme, StringComparison.OrdinalIgnoreCase))
        {
            return File.OpenRead(source);
        }

        return await client.GetStreamAsync(source, cancellationToken);
    }

    private static async Task ReadInto(Dictionary<string, DatasetRowDto> rows, Stream stream, CancellationToken cancellationToken)
    {
        using var reader = new StreamReader(stream);

        while (await reader.ReadLineAsync(cancellationToken) is { } line)
        {
            if (line.Length == 0)
            {
                continue;
            }

            var row = RowOf(line);
            rows.TryAdd(row.TitleSlug, row);
        }
    }

    // presumption: allow -- every non-empty line of the dataset's JSONL is one row object.
    private static DatasetRowDto RowOf(string line) => JsonSerializer.Deserialize<DatasetRowDto>(line, WireJson.Options)!;
}
