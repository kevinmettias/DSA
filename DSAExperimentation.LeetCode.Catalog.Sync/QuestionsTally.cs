namespace DSAExperimentation.LeetCode.Catalog.Sync;

// What a `questions` run gathers as it walks the enrolled questions - each question's fresh
// statistics, starting from the previous snapshot's so a question that fails keeps its old
// figures - and what it reports: a progress line every hundred questions, renamed slugs, failures,
// and a summary. Each question ends in one outcome - Rewritten, Unchanged or Failed - and the exit
// code says whether every question refreshed, so a partial refresh is never mistaken for a whole one.
internal sealed class QuestionsTally(
    int total, IReadOnlyDictionary<string, LeetCodeStatistics> previous, TextWriter output, TextWriter error)
{
    private const int ProgressInterval = 100;
    private const string ListSeparator = ", ";

    private readonly Dictionary<string, LeetCodeStatistics> _statistics = new(previous, StringComparer.Ordinal);
    private readonly List<string> _failures = [];
    private int _visited;
    private int _rewritten;

    public IReadOnlyDictionary<string, LeetCodeStatistics> Statistics => _statistics;

    // The question changed on LeetCode, and its file was rewritten.
    public void Rewritten(string slug, LeetCodeStatistics statistics)
    {
        _rewritten++;
        Unchanged(slug, statistics);
    }

    // The question was fetched; only its figures moved.
    public void Unchanged(string slug, LeetCodeStatistics statistics)
    {
        _statistics[slug] = statistics;
        Advance();
    }

    public void Failed(string slug, string reason)
    {
        _failures.Add(slug);
        error.WriteLine($"{slug}: {reason}");
        Advance();
    }

    // LeetCode occasionally renames a question's slug. The new slug gets its own file; the old one
    // is left for a person to retire, since deleting a question is not the sync tool's call.
    public void Renamed(string requested, string returned)
        => error.WriteLine($"{requested}: leetcode.com now calls it '{returned}'; its file was written under the new slug.");

    public int Finish()
    {
        output.WriteLine($"{total} questions fetched: {_rewritten} changed, {_failures.Count} failed.");

        if (_failures.Count == 0)
        {
            return 0;
        }

        error.WriteLine($"Failed: {string.Join(ListSeparator, _failures)}");
        return 1;
    }

    private void Advance()
    {
        _visited++;

        if (_visited % ProgressInterval == 0)
        {
            output.WriteLine($"{_visited}/{total} questions fetched, {_rewritten} changed, {_failures.Count} failed");
        }
    }
}
