#!/usr/bin/env bash
# Build and test the committed HEAD alone, in a mirror: the working tree carries peers' uncommitted
# edits that can hide a break at HEAD (7a0bbe30 renamed a benchmark arm whose HEAD caller a peer's
# draft had deleted). Usage: headcheck.sh
set -u
REPO=/f/repos/DSA
H=$REPO/.scratch/dsa-2f-pilot/headcheck
rm -rf "$H"; mkdir -p "$H"
cd "$REPO"
echo "HEAD = $(git log --oneline -1)"
git archive HEAD | tar -x -C "$H"
cd "$H"
dotnet build DSA.slnx 2>&1 | grep -E ' error |Error\(s\)' | sed 's#^.*headcheck[\\/]##' | sort -u | head -20
dotnet test DSA.slnx --no-build 2>&1 | grep -E '^(Passed!|Failed!)|^\s+Failed ' | head -20
cd "$REPO"; rm -rf "$H"
