# LeetCode coverage build-out — agent runbook

This is a standing job, not a one-off. Paste the block below to any Claude Code
agent/session (this one or a fresh one — nothing here depends on conversation
history) to make progress on it.

## Prompt to give an agent

> Continue the LeetCode coverage build-out for the DSA repo at `F:\repos\DSA`.
> Repeatedly call `Workflow({ name: "leetcode-coverage", args: { batchSize: 12, clusterSize: 3 } })`
> (falls back to `Workflow({ scriptPath: "F:\repos\DSA\.claude\workflows\leetcode-coverage.js" })`
> if it isn't registered by name yet) until a call returns `{ done: true }`. Each
> call is self-contained — it reads `.claude/leetcode-coverage/manifest.json`,
> picks the next batch of `status: "pending"` problems, implements a test +
> benchmark for each by composing this repo's existing primitives, verifies
> with a real build/test pass, and records the outcome back into the manifest
> — so no state needs to carry over between calls, sessions, or agents. Between
> runs: spot-check a few of the newly created files for quality (don't just
> trust the green build), and if `blocked` count is nonzero, leave those alone
> — they're logged with a reason (real missing primitive, or a failed
> build/test) for a dedicated follow-up pass, not something to force through.
> Every few runs, `git status`/`git diff` the new files and ask the user before
> committing. This job skips per-item Nomos gate checks for throughput, so
> periodically run this repo's Nomos gate tool over the newly added
> directories and fix real findings in a batch, the way prior phases did.

## State

- Manifest: `.claude/leetcode-coverage/manifest.json` — 1,127 problems parsed
  from `C:\Users\kmett\Downloads\leetcode questions.txt`. Each entry:
  `id`, `title`, `difficulty`, `acceptance`, `status`
  (`pending`/`done`/`blocked`/`skipped`), `skipReason`, `testPath`,
  `benchmarkPath`, `primitivesUsed`, `notes`.
- 13 already `done` (prior session's curated sample: TwoSum, ValidParentheses,
  MinStack, MergeTwoSortedLists, NetworkDelayTime, CourseSchedule,
  RedundantConnection, KthLargestElement, LowestCommonAncestorOfBst,
  MergeIntervals, Subsets, ClimbingStairs, ImplementTrie). 5 of those 13 have a
  benchmark already; the other 8 are noted as needing a benchmark backfill for
  policy consistency (every test should get one).
- 4 marked `skipped` (192/193/194/195 — LeetCode's Shell category, no
  algorithmic content).
- Everything else starts `pending`.

## Checking progress

```
jq '[.problems[] | .status] | group_by(.) | map({(.[0]): length}) | add' .claude/leetcode-coverage/manifest.json
```

## Workflow script

`.claude/workflows/leetcode-coverage.js` — 4 phases per run: Select (jq-filter
the manifest for the next batch), Implement (one agent per cluster of
`clusterSize` problems — writes the test + benchmark), Verify (real
`dotnet build`/`dotnet test` on the touched projects), Record (writes results
back to the manifest; a batch that fails Verify gets marked `blocked` instead
of `done`, so it's retried after a human looks at it, not silently lost).
