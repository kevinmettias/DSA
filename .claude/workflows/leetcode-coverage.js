export const meta = {
  name: 'leetcode-coverage',
  description: 'Batch-processes .claude/leetcode-coverage/manifest.json: composes existing DSA primitives into a test + benchmark for the next pending problems, verifies, records progress',
  phases: [
    { title: 'Select', detail: 'read manifest, pick next batch of pending problems' },
    { title: 'Implement', detail: 'one agent per cluster: design + test + benchmark' },
    { title: 'Verify', detail: 'build + run the touched test/benchmark projects' },
    { title: 'Record', detail: 'update manifest with results from this batch' },
  ],
}

const MANIFEST_PATH = '.claude/leetcode-coverage/manifest.json'
const UPDATE_SCRATCH_PATH = '.claude/leetcode-coverage/_pending-update.json'

const batchSize = (args && args.batchSize) || 12
const clusterSize = (args && args.clusterSize) || 3

const SELECT_SCHEMA = {
  type: 'object',
  properties: {
    batch: {
      type: 'array',
      items: {
        type: 'object',
        properties: {
          id: { type: 'integer' },
          title: { type: 'string' },
          difficulty: { type: 'string' },
        },
        required: ['id', 'title', 'difficulty'],
      },
    },
    totalPendingBeforeThisBatch: { type: 'integer' },
  },
  required: ['batch', 'totalPendingBeforeThisBatch'],
}

const IMPLEMENT_SCHEMA = {
  type: 'object',
  properties: {
    results: {
      type: 'array',
      items: {
        type: 'object',
        properties: {
          id: { type: 'integer' },
          status: { type: 'string', enum: ['done', 'blocked'] },
          testPath: { type: ['string', 'null'] },
          benchmarkPath: { type: ['string', 'null'] },
          primitivesUsed: { type: ['string', 'null'] },
          notes: { type: ['string', 'null'] },
        },
        required: ['id', 'status'],
      },
    },
  },
  required: ['results'],
}

const VERIFY_SCHEMA = {
  type: 'object',
  properties: {
    buildSucceeded: { type: 'boolean' },
    testsPassed: { type: 'boolean' },
    benchmarksCompiled: { type: 'boolean' },
    failureSummary: { type: ['string', 'null'] },
  },
  required: ['buildSucceeded', 'testsPassed', 'benchmarksCompiled'],
}

function buildImplementPrompt(cluster) {
  const list = cluster.map((p) => `- #${p.id} "${p.title}" (${p.difficulty})`).join('\n')
  return (
    `You are extending the DSA repo at the current working directory (F:\\repos\\DSA) - a generic C# ` +
    `algorithms/data-structures FRAMEWORK, not a LeetCode solutions repo (read ARCHITECTURE.md at the repo ` +
    `root first for the overall shape and naming conventions). Your job: for each of the following LeetCode ` +
    `problems, prove this repo's EXISTING primitives are sufficient to solve it, by writing (a) a test and ` +
    `(b) a BenchmarkDotNet benchmark. Do NOT invent new production primitives in DataStructures/ or ` +
    `Algorithms/ - if a problem genuinely cannot be solved by composing what already exists, mark it ` +
    `"blocked" with a note naming the missing primitive, do not build it ad hoc.\n\n` +
    `Problems in this cluster:\n${list}\n\n` +
    `Conventions to follow, read these two reference files first:\n` +
    `- DSAExperimentation.Tests/LeetCodeCoverage/TwoSum/TwoSumTests.cs is the reference shape for a coverage ` +
    `test: one folder per problem under DSAExperimentation.Tests/LeetCodeCoverage/<PascalCaseProblemName>/, ` +
    `a single <Name>Tests.cs (add a Fixtures/ subfolder only if the problem genuinely needs custom node/graph ` +
    `types, the way CourseSchedule/ does), using xUnit, composing 1-2 existing production primitives from ` +
    `DSAExperimentation/DataStructures or DSAExperimentation/Algorithms - browse those two folders first to ` +
    `find what already exists before assuming something is missing.\n` +
    `- DSAExperimentation.Benchmarks/ProblemSolutions/TwoSumBenchmarks.cs is the reference shape for a ` +
    `benchmark: a BenchmarkDotNet class under DSAExperimentation.Benchmarks/ProblemSolutions/<Name>Benchmarks.cs ` +
    `comparing a naive/brute-force baseline against the repo-primitive-based approach as separate [Benchmark] ` +
    `methods (if 3+ genuinely distinct algorithms apply, e.g. multiple shortest-path strategies, benchmark all ` +
    `of them, following ShortestPathAlgorithmBenchmarks.cs's shape instead) on a couple of [Params] input sizes, ` +
    `reusing/extending DSAExperimentation.Benchmarks/Fixtures where sensible.\n\n` +
    `Naming: PascalCase derived from the problem title (e.g. "House Robber II" -> HouseRobberII, keep roman ` +
    `numerals as-is). If the title starts with a digit (e.g. "3Sum", "132 Pattern"), spell out a natural C# ` +
    `identifier instead (ThreeSum, OneThreeTwoPattern) - use judgment, it just needs to be a valid, readable C# ` +
    `identifier. Check DSAExperimentation.Tests/LeetCodeCoverage/ first for a folder that already covers this ` +
    `exact problem BY CONTENT (not just by name) and report it as already done if so, without duplicating it.\n\n` +
    `Keep new code simple and idiomatic for this codebase (short, no unnecessary abstraction) - closely match ` +
    `the two reference files' style and structure; you do not need to run the full Nomos gate tool per problem ` +
    `at this scale, a separate periodic pass will handle gate compliance later.\n\n` +
    `GIT SAFETY: this repo commonly has other uncommitted work in progress from concurrent sessions/agents. ` +
    `Never run a git command that discards changes (git checkout -- <path>, git restore, git reset --hard, git ` +
    `clean) on ANY file, manifest.json included - doing so silently destroys someone else's uncommitted work ` +
    `with no error. If manifest.json or any other file looks unexpected mid-task, that is normal (another agent ` +
    `is editing concurrently) - just re-read the current file and proceed; only git status/diff/log are safe to ` +
    `run.\n\n` +
    `After writing all files for this cluster, return one result object per problem id: status "done" (with the ` +
    `real testPath/benchmarkPath - as REPO-ROOT-RELATIVE paths using forward slashes, e.g. ` +
    `"DSAExperimentation.Tests/LeetCodeCoverage/TwoSum/TwoSumTests.cs", matching the existing manifest entries' ` +
    `style, NOT an absolute Windows path - and a short primitivesUsed note) or "blocked" (with a note on exactly ` +
    `what's missing and why it's a genuine primitive gap, not just effort).`
  )
}

phase('Select')
const selection = await agent(
  `In the repo at the current working directory, read the JSON file ${MANIFEST_PATH} using a Bash one-liner ` +
  `with jq (do NOT try to read and reason over the whole 300+KB file by hand) - the file is a JSON object with ` +
  `a top-level "problems" array, each item having id/title/difficulty/status. Run:\n` +
  `  jq '[.problems[] | select(.status=="pending")] | length' ${MANIFEST_PATH}\n` +
  `to get the total pending count, then:\n` +
  `  jq --argjson n ${batchSize} '[.problems[] | select(.status=="pending")] | sort_by(.id) | .[0:$n] | map({id,title,difficulty})' ${MANIFEST_PATH}\n` +
  `to get the next batch. Report the batch array and the total pending count (from the first command, BEFORE ` +
  `removing this batch). Do not modify the file.`,
  { phase: 'Select', schema: SELECT_SCHEMA },
)

log(
  `Selected ${selection.batch.length} problems (${selection.totalPendingBeforeThisBatch} pending total before this batch).`,
)

if (selection.batch.length === 0) {
  return { done: true, message: 'No pending problems remain in the manifest.' }
}

const clusters = []
for (let i = 0; i < selection.batch.length; i += clusterSize) {
  clusters.push(selection.batch.slice(i, i + clusterSize))
}

phase('Implement')
const implementResults = await pipeline(clusters, (cluster) =>
  agent(buildImplementPrompt(cluster), {
    phase: 'Implement',
    schema: IMPLEMENT_SCHEMA,
    label: `impl:${cluster[0].id}-${cluster[cluster.length - 1].id}`,
  }),
)

const allResults = implementResults.filter(Boolean).flatMap((r) => r.results)
log(
  `Implemented ${allResults.filter((r) => r.status === 'done').length}/${selection.batch.length}, ` +
    `blocked ${allResults.filter((r) => r.status === 'blocked').length}.`,
)

phase('Verify')
const verifyReport = await agent(
  `In the repo at the current working directory (no top-level .sln - build/test per-project). Run, from the ` +
  `repo root:\n` +
  `  dotnet build DSAExperimentation.Tests/DSAExperimentation.Tests.csproj -c Release\n` +
  `  dotnet test DSAExperimentation.Tests/DSAExperimentation.Tests.csproj --filter FullyQualifiedName~LeetCodeCoverage -c Release --no-build\n` +
  `  dotnet build DSAExperimentation.Benchmarks/DSAExperimentation.Benchmarks.csproj -c Release\n` +
  `(the last one just confirms the new benchmark classes compile - do NOT actually run BenchmarkDotNet jobs, ` +
  `that's too slow for this pass). Report whether the test-project build succeeded, whether the ` +
  `LeetCodeCoverage-filtered tests passed, whether the benchmarks project built, and if anything failed, a ` +
  `concise summary of the first few real failures (file/line/message) - not the full raw log.`,
  { phase: 'Verify', schema: VERIFY_SCHEMA },
)

log(
  `Verify: build=${verifyReport.buildSucceeded} tests=${verifyReport.testsPassed} ` +
    `benchmarksCompiled=${verifyReport.benchmarksCompiled}`,
)

const batchIsClean = verifyReport.buildSucceeded && verifyReport.testsPassed && verifyReport.benchmarksCompiled
const finalResults = batchIsClean
  ? allResults
  : allResults.map((r) => ({
      ...r,
      status: 'blocked',
      notes:
        `Batch build/test failed, reverted to blocked for retry: ${verifyReport.failureSummary || 'see verify phase log'}. ` +
        `Original note: ${r.notes || '(none)'}`,
    }))

phase('Record')
const recordSummary = await agent(
  `In the repo at the current working directory. Write EXACTLY this JSON array, verbatim, to a new file at ` +
  `${UPDATE_SCRATCH_PATH} using your file-write tool:\n\n${JSON.stringify(finalResults, null, 2)}\n\n` +
  `Then run this Python one-liner via Bash to apply it to ${MANIFEST_PATH} (adjust only if python3 is not on ` +
  `PATH - node is also available):\n\n` +
  `python3 -c "` +
  `import json; ` +
  `m=json.load(open('${MANIFEST_PATH}', encoding='utf-8')); ` +
  `u={x['id']: x for x in json.load(open('${UPDATE_SCRATCH_PATH}', encoding='utf-8'))}; ` +
  `[p.update({k:v for k,v in u[p['id']].items() if k!='id'}) for p in m['problems'] if p['id'] in u]; ` +
  `json.dump(m, open('${MANIFEST_PATH}','w',encoding='utf-8'), indent=2, ensure_ascii=False); ` +
  `print('updated', len(u), 'entries')` +
  `"\n\n` +
  `Then delete ${UPDATE_SCRATCH_PATH}. Return the "updated N entries" line.`,
  { phase: 'Record' },
)

log(recordSummary)

return {
  batchRequested: selection.batch.length,
  implemented: finalResults.filter((r) => r.status === 'done').length,
  blocked: finalResults.filter((r) => r.status === 'blocked').length,
  totalPendingBeforeThisBatch: selection.totalPendingBeforeThisBatch,
  verify: verifyReport,
  recordSummary,
}
