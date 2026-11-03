using DSAExperimentation.LeetCode.Catalog.Sync;

// Refreshes DSAExperimentation.LeetCode.Catalog/Snapshot/ from its three sources:
//
//   questions [slug...]   leetcode.com: every enrolled question, and statistics; a slug named enrols it
//   ratings               zerotrac: contest ratings of the enrolled questions
//   dataset [source...]   LeetCodeDataset: generated cases for the enrolled questions
//   weekly                questions, then ratings - what .github/workflows/leetcode-catalog-sync.yml runs
//
// Run with `dotnet run --project DSAExperimentation.LeetCode.Catalog.Sync -- <verb>`.
const string WeeklyVerb = "weekly";
const string Usage = "usage: questions [slug...] | ratings | dataset [jsonl path or URL...] | weekly";
const int UsageExitCode = 2;

using var cancellation = new CancellationTokenSource();
Console.CancelKeyPress += (_, keyPress) =>
{
    keyPress.Cancel = true;
    cancellation.Cancel();
};

var verb = args.FirstOrDefault() ?? string.Empty;
var operands = args.Skip(1).ToList();

switch (verb)
{
    case QuestionsCommand.Verb:
        return await QuestionsCommand.RunAsync(operands, Console.Out, Console.Error, cancellation.Token);
    case RatingsCommand.Verb:
        return await RatingsCommand.RunAsync(Console.Out, cancellation.Token);
    case DatasetCommand.Verb:
        return await DatasetCommand.RunAsync(operands, Console.Out, cancellation.Token);
    case WeeklyVerb:
        var questions = await QuestionsCommand.RunAsync([], Console.Out, Console.Error, cancellation.Token);
        var ratings = await RatingsCommand.RunAsync(Console.Out, cancellation.Token);
        return Math.Max(questions, ratings);
    default:
        Console.Error.WriteLine(Usage);
        return UsageExitCode;
}
