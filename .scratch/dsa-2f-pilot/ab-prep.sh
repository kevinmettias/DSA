#!/usr/bin/env bash
# Put both A/B worktrees at HEAD exactly, then overlay this step's working-tree files on "after"; a
# listed path missing from the working tree is a deletion. Usage: ab-prep.sh "<file list>"
set -eu
REPO=/f/repos/DSA
AB=/f/repos/DSA-ab-2f
files=$1
cd "$REPO"
head=$(git rev-parse HEAD)
for side in before after; do
  # Force both sides to HEAD exactly: drop the previous step's copied files, tracked and new alike
  # (ignored build output stays, so builds remain incremental).
  if [ -d "$AB/$side" ]; then git -C "$AB/$side" checkout -q -f --detach "$head"; git -C "$AB/$side" clean -fdq
  else git worktree add -q --detach "$AB/$side" "$head"; fi
  [ "$(git -C "$AB/$side" rev-parse HEAD)" = "$head" ] || { echo "$side is not at $head" >&2; exit 1; }
done
for f in $files; do
  if [ -e "$REPO/$f" ]; then mkdir -p "$AB/after/$(dirname "$f")"; cp "$REPO/$f" "$AB/after/$f"
  else rm -f "$AB/after/$f"; fi
done
echo "before = $head, after = before + $(echo $files | wc -w) path(s)"
